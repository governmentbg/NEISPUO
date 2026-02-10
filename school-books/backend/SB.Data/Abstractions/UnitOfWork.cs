namespace SB.Data;

using EFCore.BulkExtensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SB.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

internal class UnitOfWork : IUnitOfWork
{
    private bool disposed;
    private IDbContextTransactionManager? dbContextTransactionManager;
    private Lazy<DbContext> dbContext;

    public UnitOfWork(
        IDbContextFactory<DbContext> dbContextFactory)
    {
        this.dbContext = new Lazy<DbContext>(() => dbContextFactory.CreateDbContext());
    }

    public DbContext DbContext => this.dbContext.Value;

    private IDbContextTransactionManager DbContextTransactionManager
    {
        get
        {
            if (this.dbContextTransactionManager == null)
            {
                this.dbContextTransactionManager = this.DbContext.Database.GetService<IDbContextTransactionManager>();
            }

            return this.dbContextTransactionManager;
        }
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        try
        {
            await this.DbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainUpdateConcurrencyException();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
    }

    // copied from https://github.com/dotnet/efcore/blob/main/src/EFCore/Storage/ExecutionStrategy.cs
    private const int DefaultMaxRetryCount = 6;
    private static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(30);
    public async Task SaveWithRetryingStrategyAsync(ICollection<int>? errorNumbersToAdd, CancellationToken ct)
    {
        try
        {
            SqlServerRetryingExecutionStrategy strategy = new(this.DbContext, DefaultMaxRetryCount, DefaultMaxDelay, errorNumbersToAdd);
            await strategy.ExecuteAsync(
                async () =>
                    await this.DbContext.SaveChangesAsync(
                        acceptAllChangesOnSuccess: false,
                        cancellationToken: ct));

            this.DbContext.ChangeTracker.AcceptAllChanges();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainUpdateConcurrencyException();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
        catch (RetryLimitExceededException ex) when (ex.InnerException is DbUpdateException { InnerException: SqlException sqlEx })
        {
            throw new DomainUpdateSqlException(sqlEx);
        }
    }

    public async Task BulkInsertAsync<T>(IEnumerable<T> entities, CancellationToken ct) where T : class
    {
        await this.BulkInsertAsync(entities, 2000, ct);
    }

    public async Task BulkInsertAsync<T>(IEnumerable<T> entities, int batchSize, CancellationToken ct) where T : class
    {
        await this.DbContext.BulkInsertAsync(entities, new BulkConfig { BatchSize = batchSize }, cancellationToken: ct);
    }

    public void UseConnection(DbConnection connection, DbTransaction? transaction = null)
    {
        this.DbContext.Database.SetDbConnection(connection);
        if (transaction != null)
        {
            this.DbContext.Database.UseTransaction(transaction);
        }
    }

    public void UseTransaction(ITransaction transaction)
    {
        transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));

        var dbTransaction = transaction.GetDbTransaction();
        var dbConnection = dbTransaction.Connection ?? throw new DomainException("The provided transaction is not associated with a connection.");

        this.UseConnection(dbConnection, dbTransaction);
    }

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct)
    {
        var existingTransaction = this.DbContextTransactionManager.CurrentTransaction;
        if (existingTransaction != null)
        {
            return new Transaction(existingTransaction, true);
        }

        // transaction isolation level must not be specified as it is persisted across requests by the connection pool
        return new Transaction(await this.DbContext.Database.BeginTransactionAsync(ct), false);
    }

    public void Dispose()
    {
        // sync disposal is required as IServiceScope currently does not
        // provide async disposal and working with it will make Autofac
        // to throw an exception that the scope is disposed synchronously
        // but the service only supports asynchronous disposal
        if (!this.disposed)
        {
            if (this.dbContext.IsValueCreated)
            {
                this.dbContext.Value.Dispose();
            }

            this.dbContextTransactionManager = null;
            this.disposed = true;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!this.disposed)
        {
            if (this.dbContext.IsValueCreated)
            {
                await this.dbContext.Value.DisposeAsync();
            }

            this.dbContextTransactionManager = null;
            this.disposed = true;
        }
    }
}
