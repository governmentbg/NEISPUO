namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task SaveAsync(CancellationToken ct);
    Task SaveWithRetryingStrategyAsync(ICollection<int>? errorNumbersToAdd, CancellationToken ct);
    Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken ct) where T : class;
    Task BulkInsertAsync<T>(IEnumerable<T> entities, int batchSize, CancellationToken ct) where T : class;
    void UseConnection(DbConnection connection, DbTransaction? transaction = null);
    void UseTransaction(ITransaction transaction);
    Task<ITransaction> BeginTransactionAsync(CancellationToken ct);
}
