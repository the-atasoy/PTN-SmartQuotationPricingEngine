using Domain.Entities;

namespace Application.Interfaces;

/// <summary>The result of generating an access token.</summary>
public record AccessTokenResult(string Token, long ExpiryUnixSeconds);

public interface ITokenService
{
    AccessTokenResult GenerateAccessToken(User user);
    string GenerateRefreshToken(Guid userId);
}
