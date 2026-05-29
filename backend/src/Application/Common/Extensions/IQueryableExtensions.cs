using Application.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Extensions;

public static class IQueryableExtensions
{

    public static IQueryable<T> Sort<T>(
        this IQueryable<T> query,
        string? sortColumn,
        SortDirection? sortDirection,
        string? defaultSortColumn = null,
        bool forceDescendingOnDefault = false) where T : class
    {
        var targetColumn = string.IsNullOrWhiteSpace(sortColumn) ? defaultSortColumn : sortColumn;
        var targetDirection = sortDirection;

        if (string.IsNullOrWhiteSpace(targetColumn))
            return query;

        if (string.IsNullOrWhiteSpace(sortColumn) && forceDescendingOnDefault)
        {
            targetDirection = SortDirection.Desc;
        }

        var property = typeof(T).GetProperties()
            .FirstOrDefault(p => p.Name.Equals(targetColumn, StringComparison.OrdinalIgnoreCase));

        if (property == null)
            return query;

        Expression<Func<T, object>> orderExpr = entity => EF.Property<object>(entity, property.Name);

        return targetDirection == SortDirection.Desc
            ? query.OrderByDescending(orderExpr)
            : query.OrderBy(orderExpr);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize)
    {
        return query.Skip(page * pageSize).Take(pageSize);
    }
}
