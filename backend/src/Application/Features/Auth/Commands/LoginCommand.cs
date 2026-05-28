using Application.Common.Models;
using MediatR;

namespace Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<ApiResponse<Dtos.LoginResponseDto>>;
