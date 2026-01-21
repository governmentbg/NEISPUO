using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MON.Services.Extensions
{
    public static class EFCoreExtensions
    {
        public static IQueryable<T> TagWithSource<T>(
            this IQueryable<T> queryable,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            return queryable.TagWith($"{memberName}  - {filePath}:{lineNumber}");
        }

        public static IQueryable<T> TagWithSource<T>(this IQueryable<T> queryable,
            string tag,
            [CallerLineNumber] int lineNumber = 0,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "")
        {
            return queryable.TagWith($"{tag}{Environment.NewLine}{memberName}  - {filePath}:{lineNumber}");
        }

        public static IQueryable<object> Query(this DbContext context, Type T)
        {
            // Get the generic type definition
            MethodInfo method = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

            // Build a method with the specific type argument you're interested in
            method = method.MakeGenericMethod(T);

            return (IQueryable<object>)method.Invoke(context, null);
        }

        public static IQueryable<object> Query(this DbContext _context, string tableName)
        {
            Type entityType = _context.Model.FindEntityType($"MON.DataAccess.{tableName}").ClrType;
            IQueryable<object> objectContext = _context.Query(entityType);
            return objectContext;
        }

    }

    public static class AsyncEnumerable
    {
        /// <summary>
        /// Creates an <see cref="IAsyncEnumerable{T}"/> which yields no results, similar to <see cref="Enumerable.Empty{TResult}"/>.
        /// </summary>
        public static IAsyncEnumerable<T> Empty<T>() => EmptyAsyncEnumerator<T>.Instance;

        private class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
        {
            public static readonly EmptyAsyncEnumerator<T> Instance = new EmptyAsyncEnumerator<T>();
            public T Current => default;
            public ValueTask DisposeAsync() => default;
            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return this;
            }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
