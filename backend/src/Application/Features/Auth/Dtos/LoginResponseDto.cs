using System.Text.Json.Serialization;

namespace Application.Features.Auth.Dtos;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = default!;

    [JsonIgnore]
    public string RefreshToken { get; set; } = default!;
}
