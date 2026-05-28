using Application.Common.Models;
using MediatR;

namespace Application.Features.Auth.Commands;

public record RefreshCommand(string AccessToken, string RefreshToken) : IRequest<ApiResponse<Dtos.LoginResponseDto>>;
