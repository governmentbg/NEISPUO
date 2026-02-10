namespace MON.Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Linq.Dynamic.Core;
    using MON.Models.Grid;
    using MON.Shared;

    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Where<TEntity>(
            this IQueryable<TEntity> source,
            bool condition = false,
            Expression<Func<TEntity, bool>> predicate = null)
        {
            if (condition && predicate != null)
            {
                source = source.Where(predicate);
            }

            return source;
        }

        public static IQueryable<TEntity> Where<TEntity>(
            this IQueryable<TEntity> source,
            string colName, Type colType, string filterOp, object filter)
        {

            if (!string.IsNullOrWhiteSpace(colName) && !string.IsNullOrWhiteSpace(filterOp) && filter != null)
            {
                switch (filterOp.ToLower())
                {
                    case "equal":
                        source = source.Where($"{colName} != null && {colName} == @0", filter);
                        break;
                    case "notequal":
                        source = source.Where($"{colName} == null || {colName} != @0", filter);
                        break;
                    case "contains":
                        if (colType == typeof(string))
                        {
                            source = source.Where($"{colName} != null && {colName}.ToLower().Contains(@0)", filter);
                        }
                        else
                        {
                            source = source.Where($"{colName} != null && {colName}.ToString().Contains(@0)", filter);
                        }
                        break;
                    case "startswith":
                        if (colType == typeof(string))
                        {
                            source = source.Where($"{colName} != null && {colName}.ToLower().StartsWith(@0)", filter);
                        }
                        else
                        {
                            source = source.Where($"{colName} != null && {colName}.ToString().StartsWith(@0)", filter);
                        }
                        break;
                    case "endswith":
                        if (colType == typeof(string))
                        {
                            source = source.Where($"{colName} != null && {colName}.ToLower().EndsWith(@0)", filter);
                        }
                        else
                        {
                            source = source.Where($"{colName} != null && {colName}.ToString().EndsWith(@0)", filter);
                        }
                        break;
                    default:
                        break;
                }
            }


            return source;
        }

        public static IQueryable<T> PagedBy<T>(
            this IQueryable<T> source,
            int pageIndex,
            int pageSize,
            int indexFrom = 0,
            CancellationToken cancellationToken = default)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }

            if (pageSize < 0) // Значи всички
            {
                pageSize = int.MaxValue;
            }

            return source.Skip((pageIndex - indexFrom) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Filter<T>(this IQueryable<T> source, GridFilter[] filters)
        {
            if(filters.IsNullOrEmpty())
            {
                return source;
            }

            foreach (var filter in filters.Where(x => !x.Filter.IsNullOrWhiteSpace()))
            {
                source = source.Where(filter.Field, typeof(string), filter.Op, filter.Filter);
            }

            return source;

        }
    }
}
