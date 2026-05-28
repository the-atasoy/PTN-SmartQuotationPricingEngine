using Application.Features.Auth.Commands;
using Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        // Set refresh token in HttpOnly cookie
        SetRefreshTokenCookie(response.Data.RefreshToken);

        return Ok(response); // response.Data.RefreshToken is [JsonIgnore] so it won't be serialized in body
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { message = "Refresh token is missing." });
        }

        var command = new RefreshCommand(request.AccessToken, refreshToken);
        var response = await _mediator.Send(command);

        if (!response.IsSuccessful || response.Data == null)
        {
            return StatusCode(response.StatusCode, response);
        }

        SetRefreshTokenCookie(response.Data.RefreshToken);

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            await _mediator.Send(new LogoutCommand(userId));
        }

        Response.Cookies.Delete("refresh_token");

        return Ok(new { message = "Logged out successfully." });
    }

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refresh_token", token, cookieOptions);
    }
}

public class RefreshRequestDto
{
    public string AccessToken { get; set; } = default!;
}
