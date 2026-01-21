namespace SB.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public static class QueryableExtensions
{
    public static IQueryable<T> WithOffsetAndLimit<T>(this IQueryable<T> query, int? offset, int? limit)
    {
        offset ??= 0;
        if (offset > 0)
        {
            query = query.Skip(offset.Value);
        }

        if (limit.HasValue)
        {
            query = query.Take(limit.Value);
        }

        return query;
    }

    public static Task<IEnumerable<T>> UnionWithOffsetAndLimitAsync<T>(
        this IEnumerable<IQueryable<T>> queries,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return UnionWithOffsetAndLimitAsync(queries, offset, limit, EqualityComparer<T>.Default, ct);
    }

    public static async Task<IEnumerable<T>> UnionWithOffsetAndLimitAsync<T>(
        this IEnumerable<IQueryable<T>> queries,
        int? offset,
        int? limit,
        IEqualityComparer<T> comparer,
        CancellationToken ct)
    {
        if (queries == null || !queries.Any())
        {
            throw new ArgumentException("Parameter should be non-null and non-empty", nameof(queries));
        }

        var next = queries.First();
        var rest = queries.Skip(1);

        if (!rest.Any())
        {
            // this is the final query, execute and return
            return await next.WithOffsetAndLimit(offset, limit).ToListAsync(ct);
        }

        offset ??= 0;
        if (offset > 0)
        {
            var count = next.Count();
            if (offset >= count)
            {
                // the offset is beyond the current query item count, continue with the rest
                return await rest.UnionWithOffsetAndLimitAsync(offset - count, limit, ct);
            }
        }

        var items = await next.WithOffsetAndLimit(offset, limit).ToListAsync(ct);

        if (limit != null && items.Count == limit.Value)
        {
            // we've reached the limit, no more quiering needed
            return items;
        }

        var nextItems = await rest.UnionWithOffsetAndLimitAsync(
            0,
            limit == null ? null : limit - items.Count,
            ct);

        return items.Union(nextItems, comparer);
    }

    private static uint counter;
    public static IQueryable<IdQO> MakeIdsQuery(this DbContext dbContext, int[] ids)
    {
        uint next = Interlocked.Increment(ref counter);
        return dbContext.Set<IdQO>()
            .FromSqlRaw(
                $"SELECT [Id] FROM OPENJSON(@idsJsonArray_{next}) WITH ([Id] INT '$')",
                new SqlParameter($"idsJsonArray_{next}", JsonSerializer.Serialize(ids)));
    }

    public static IQueryable<Id2QO> MakeIdsQuery(this DbContext dbContext, (int id1, int id2)[] ids)
    {
        uint next = Interlocked.Increment(ref counter);
        return dbContext.Set<Id2QO>()
            .FromSqlRaw(
                $"SELECT [Id1], [Id2] FROM OPENJSON(@idsJsonArray_{next}) WITH ([Id1] INT '$.Id1', [Id2] INT '$.Id2')",
                new SqlParameter(
                    $"idsJsonArray_{next}",
                    JsonSerializer.Serialize(
                        ids
                        .Select(t => new Id2QO() { Id1 = t.id1, Id2 = t.id2 })
                        .ToArray()
                    )
                )
            );
    }

    public static IQueryable<Id3QO> MakeIdsQuery(this DbContext dbContext, (int id1, DateTime id2)[] ids)
    {
        uint next = Interlocked.Increment(ref counter);
        return dbContext.Set<Id3QO>()
            .FromSqlRaw(
                $"SELECT [Id1], [Id2] FROM OPENJSON(@idsJsonArray_{next}) WITH ([Id1] INT '$.Id1', [Id2] DATETIME2 '$.Id2')",
                new SqlParameter(
                    $"idsJsonArray_{next}",
                    JsonSerializer.Serialize(
                        ids
                        .Select(t => new Id3QO() { Id1 = t.id1, Id2 = t.id2 })
                        .ToArray()
                    )
                )
            );
    }

    public static async Task<TableResultVO<T>> ToTableResultAsync<T>(this IQueryable<T> query, int? offset, int? limit, CancellationToken ct)
    {
        int length = await query.CountAsync(ct);
        if (length == 0)
        {
            return TableResultVO.Empty<T>();
        }

        T[] result = await query
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);

        return new TableResultVO<T>(result, length);
    }

    public static async Task<TableResultVO<T>> ToTableResultAsync<TItem, T>(this IQueryable<TItem> query, Func<TItem, T> selector, int? offset, int? limit, CancellationToken ct)
    {
        int length = await query.CountAsync(ct);
        if (length == 0)
        {
            return TableResultVO.Empty<T>();
        }

        TItem[] result = await query
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);

        return new TableResultVO<T>(result.Select(selector).ToArray(), length);
    }
}
