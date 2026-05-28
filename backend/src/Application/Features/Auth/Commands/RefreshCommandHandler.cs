using Application.Common.Models;
using Application.Features.Auth.Dtos;
using Application.Interfaces;
using Application.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public RefreshCommandHandler(IApplicationDbContext context, ITokenService tokenService, IPasswordHasher passwordHasher, IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _localizer = localizer;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = request.RefreshToken;
        var parts = refreshToken.Split(':');
        if (parts.Length != 2 || !Guid.TryParse(parts[0], out var userId))
        {
            return ApiResponse<LoginResponseDto>.Fail(_localizer["InvalidRefreshToken"].Value, 401);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        // If the user has no stored token hash, their tokens were already revoked.
        if (user == null || user.RefreshTokenHash == null)
        {
            return ApiResponse<LoginResponseDto>.Fail(_localizer["InvalidOrExpiredRefreshToken"].Value, 401);
        }

        // Fix 5: If the presented token does NOT match the stored hash, the token has already been
        // rotated — it was either already used or stolen. Revoke ALL sessions for this user immediately.
        if (!_passwordHasher.VerifyPassword(request.RefreshToken, user.RefreshTokenHash))
        {
            user.RevokeRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<LoginResponseDto>.Fail(_localizer["InvalidRefreshToken"].Value, 401);
        }

        // Expiry is checked after verifying the hash to avoid a timing leak on the token value.
        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            user.RevokeRefreshToken();
            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<LoginResponseDto>.Fail(_localizer["InvalidOrExpiredRefreshToken"].Value, 401);
        }

        var accessTokenResult = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);
        var newRefreshTokenHash = _passwordHasher.HashPassword(newRefreshToken);

        user.UpdateRefreshToken(newRefreshTokenHash, newRefreshTokenExpiry);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<LoginResponseDto>.Success(new LoginResponseDto
        {
            AccessToken = accessTokenResult.Token,
            AccessTokenExpiry = accessTokenResult.ExpiryUnixSeconds,
            Role = user.Role.ToString(),
            RefreshToken = newRefreshToken,
            RefreshTokenExpiry = newRefreshTokenExpiry
        });
    }
}
