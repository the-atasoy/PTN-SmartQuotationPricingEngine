using System.Text.Json.Serialization;

namespace Application.Features.Auth.DTOs;

public class LoginResponseDto
{
    public string AccessToken { get; init; } = default!;

    /// <summary>Unix epoch (seconds) when the access token expires. Used by the server to set auth_meta cookie.</summary>
    [JsonIgnore]
    public long AccessTokenExpiry { get; init; }

    /// <summary>Role of the authenticated user. Used by the server to set auth_meta cookie.</summary>
    [JsonIgnore]
    public string Role { get; init; } = default!;

    [JsonIgnore]
    public string RefreshToken { get; init; } = default!;

    [JsonIgnore]
    public DateTime RefreshTokenExpiry { get; init; }
}
