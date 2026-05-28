using Application.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Common.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> expression)
    {
        return condition ? query.Where(expression) : query;
    }

    public static IQueryable<T> ContainsAny<T>(
        this IQueryable<T> query,
        Expression<Func<T, string>> propertySelector,
        IReadOnlyCollection<string> values)
    {
        if (values is null || values.Count == 0)
            return query;

        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;

        var notNull = Expression.NotEqual(property, Expression.Constant(null, typeof(string)));

        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;

        Expression? orChain = null;
        foreach (var value in values)
        {
            var containsCall = Expression.Call(property, containsMethod, Expression.Constant(value));
            orChain = orChain == null ? containsCall : Expression.OrElse(orChain, containsCall);
        }

        var body = Expression.AndAlso(notNull, orChain!);
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

        return query.Where(predicate);
    }

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
