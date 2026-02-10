namespace MON.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGenericRepository<T, V, U> where T : class where V : T where U : class
    {
        Task<V> GetByIdAsync(int id);
        IQueryable<V> GetAll();
        IQueryable<V> Find(Expression<Func<V, bool>> expression);
        Task<int> AddAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task ArchiveAsync(int id);
        Task UpdateAsync(T entity);
    }
}
