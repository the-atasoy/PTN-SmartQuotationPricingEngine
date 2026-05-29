using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetRequestById;

public record GetRequestByIdQuery(Guid Id) : IRequest<ApiResponse<RequestDetailDto>>;
