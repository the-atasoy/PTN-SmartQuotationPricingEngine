using Application.Common.Enums;

namespace Application.Common.Models;

public record PaginatedQuery
{
    public int Page { get; init; } = 0;
    public int PageSize { get; init; } = 10;
    public string? SortColumn { get; init; }
    public SortDirection? SortDirection { get; init; }
}
