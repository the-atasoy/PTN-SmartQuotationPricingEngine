using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetAllRequests;

public record GetAllRequestsQuery : PaginatedQuery, IRequest<ApiResponse<PaginatedResult<RequestListItemDto>>>;
