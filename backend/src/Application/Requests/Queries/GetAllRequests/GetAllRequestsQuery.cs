using Application.Common.Models;
using MediatR;

namespace Application.Requests.Queries.GetAllRequests;

public class GetAllRequestsQuery : IRequest<ApiResponse<PaginatedResult<RequestListItemDto>>>
{
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public string? SortColumn { get; set; }
    public Common.Enums.SortDirection? SortDirection { get; set; }
}
