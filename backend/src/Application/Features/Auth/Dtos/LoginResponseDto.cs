using System.Text.Json.Serialization;

namespace Application.Features.Auth.Dtos;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = default!;

    /// <summary>Unix epoch (seconds) when the access token expires. Used by the server to set auth_meta cookie.</summary>
    [JsonIgnore]
    public long AccessTokenExpiry { get; set; }

    /// <summary>Role of the authenticated user. Used by the server to set auth_meta cookie.</summary>
    [JsonIgnore]
    public string Role { get; set; } = default!;

    [JsonIgnore]
    public string RefreshToken { get; set; } = default!;

    [JsonIgnore]
    public DateTime RefreshTokenExpiry { get; set; }
}
