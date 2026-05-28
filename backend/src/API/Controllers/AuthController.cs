using Application.Features.Auth.Commands;
using Application.Features.Auth.Dtos;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var response = await _mediator.Send(command);

        if (!response.IsSuccessful || response.Data == null)
        {
            return StatusCode(response.StatusCode, response);
        }

        SetRefreshTokenCookie(response.Data.RefreshToken, response.Data.RefreshTokenExpiry);
        SetAuthMetaCookie(response.Data.Role, response.Data.AccessTokenExpiry);

        return Ok(response); // response.Data.RefreshToken is [JsonIgnore] so it won't be serialized in body
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token is missing." });
        }

        var command = new RefreshCommand(refreshToken);
        var response = await _mediator.Send(command);

        if (!response.IsSuccessful || response.Data == null)
        {
            return StatusCode(response.StatusCode, response);
        }

        SetRefreshTokenCookie(response.Data.RefreshToken, response.Data.RefreshTokenExpiry);
        SetAuthMetaCookie(response.Data.Role, response.Data.AccessTokenExpiry);

        return Ok(response);
    }

    [HttpPost("logout")]
    [AllowAnonymous] // Access token may be expired; identify session via the refresh token cookie
    public async Task<IActionResult> Logout()
    {
        // Try to revoke the refresh token if we can identify the user from the access token.
        // If the access token is missing or expired, we still clear the cookies (best-effort).
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            await _mediator.Send(new LogoutCommand(userId));
        }

        // Always clear both cookies
        Response.Cookies.Delete("refresh_token");
        Response.Cookies.Delete("auth_meta");

        return Ok(new { message = "Logged out successfully." });
    }

    private void SetRefreshTokenCookie(string token, DateTime expiry)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expiry
        };
        Response.Cookies.Append("refresh_token", token, cookieOptions);
    }

    /// <summary>
    /// Sets an HttpOnly auth_meta cookie so the Next.js middleware can read role/exp
    /// for routing decisions without that data being forgeable from JavaScript.
    /// </summary>
    private void SetAuthMetaCookie(string role, long accessTokenExp)
    {
        var value = System.Text.Json.JsonSerializer.Serialize(new { role, exp = accessTokenExp });
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("auth_meta", value, cookieOptions);
    }
}
