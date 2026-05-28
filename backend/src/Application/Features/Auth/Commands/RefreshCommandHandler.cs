using System.Security.Claims;
using Application.Common.Models;
using Application.Features.Auth.Dtos;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public RefreshCommandHandler(IApplicationDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        }
        catch
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid access token.", 401);
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid access token claims.", 401);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null || user.RefreshTokenHash == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid or expired refresh token.", 401);
        }

        if (!_passwordHasher.VerifyPassword(request.RefreshToken, user.RefreshTokenHash))
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid refresh token.", 401);
        }

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _passwordHasher.HashPassword(newRefreshToken);

        user.UpdateRefreshToken(newRefreshTokenHash, DateTime.UtcNow.AddDays(7));
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<LoginResponseDto>.Success(new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}
