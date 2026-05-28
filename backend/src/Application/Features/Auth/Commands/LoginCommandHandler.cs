using Application.Common.Models;
using Application.Features.Auth.Dtos;
using Application.Interfaces;
using Application.Resources;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LoginCommandHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IStringLocalizer<SharedResource> localizer)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _localizer = localizer;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant().Trim(), cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponseDto>.Fail(_localizer["InvalidCredentials"].Value, 401);
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Hash the refresh token before storing it
        var refreshTokenHash = _passwordHasher.HashPassword(refreshToken);
        
        // Save to DB (Refresh token valid for 7 days)
        user.UpdateRefreshToken(refreshTokenHash, DateTime.UtcNow.AddDays(7));
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<LoginResponseDto>.Success(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}
