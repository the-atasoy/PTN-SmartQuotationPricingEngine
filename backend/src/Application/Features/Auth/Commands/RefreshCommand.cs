using Application.Common.Models;
using MediatR;

namespace Application.Features.Auth.Commands;

public record RefreshCommand(string RefreshToken) : IRequest<ApiResponse<DTOs.LoginResponseDto>>;
