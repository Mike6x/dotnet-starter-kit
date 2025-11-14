using FSH.Framework.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FSH.Framework.Persistence;

public static class PaginationExtensions
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public static Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> source,
        IPaginationParameters pagination,
        CancellationToken cancellationToken = default)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(pagination);

        var pageNumber = pagination.PageNumber is null or <= 0
            ? 1
            : pagination.PageNumber.Value;

        var pageSize = pagination.PageSize is null or <= 0
            ? DefaultPageSize
            : pagination.PageSize.Value;

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        var sorted = ApplySorting(source, pagination.Sort);

        return ToPagedResponseInternalAsync(sorted, pageNumber, pageSize, cancellationToken);
    }

    private static async Task<PagedResponse<T>> ToPagedResponseInternalAsync<T>(
        IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        where T : class
    {
        var totalCount = await source.LongCountAsync(cancellationToken).ConfigureAwait(false);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);

        if (pageNumber > totalPages && totalPages > 0)
        {
            pageNumber = totalPages;
        }

        var skip = (pageNumber - 1) * pageSize;

        var items = await source
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return new PagedResponse<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }

    private static IQueryable<T> ApplySorting<T>(IQueryable<T> source, string? sortExpression)
    {
        if (string.IsNullOrWhiteSpace(sortExpression))
        {
            return source;
        }

        var clauses = sortExpression
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (clauses.Length == 0)
        {
            return source;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        IOrderedQueryable<T>? ordered = null;

        foreach (var rawClause in clauses)
        {
            if (string.IsNullOrWhiteSpace(rawClause))
            {
                continue;
            }

            var clause = rawClause.Trim();
            var descending = clause[0] == '-';
            if (clause[0] is '-' or '+')
            {
                clause = clause[1..];
            }

            if (string.IsNullOrWhiteSpace(clause))
            {
                continue;
            }

            var property = typeof(T).GetProperty(
                clause,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);
            if (property is null)
            {
                // Unknown property; skip this clause to keep sorting safe.
                continue;
            }

            var propertyAccess = Expression.Property(parameter, property);
            var keySelectorType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            var keySelector = Expression.Lambda(keySelectorType, propertyAccess, parameter);

            string? methodName;
            if (descending)
            {
                if (descending)
                {
                    methodName = ordered is null
                ? ("OrderByDescending")
                : ("ThenByDescending");
                }
                else
                {
                    methodName = ordered is null
                ? ("OrderByDescending")
                : ("ThenBy");
                }
            }
            else
            {
                if (descending)
                {
                    methodName = ordered is null
                ? ("OrderBy")
                : ("ThenByDescending");
                }
                else
                {
                    methodName = ordered is null
                ? ("OrderBy")
                : ("ThenBy");
                }
            }

            var call = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.PropertyType },
                (ordered ?? source).Expression,
                Expression.Quote(keySelector));

            ordered = (IOrderedQueryable<T>)(ordered ?? source.Provider.CreateQuery<T>(call));
        }

        return ordered ?? source;
    }
}
