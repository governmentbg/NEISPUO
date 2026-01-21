namespace SB.Data;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.Domain;
using static SB.Domain.IQueueMessagesRepository;

class QueueMessagesRepository : IQueueMessagesRepository
{
    private UnitOfWork unitOfWork;
    private DataOptions dataOptions;
    private ILogger<QueueMessagesRepository> logger;

    public QueueMessagesRepository(
        UnitOfWork unitOfWork,
        IOptions<DataOptions> dataOptions,
        ILogger<QueueMessagesRepository> logger)
    {
        this.unitOfWork = unitOfWork;
        this.dataOptions = dataOptions.Value;
        this.logger = logger;
    }

    public async Task InsertQueueMessagesAsync(
        QueueMessageType type,
        string[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct)
    {
        List<SqlParameter> parameters = new()
        {
            new SqlParameter("@type", (int)type),
            new SqlParameter("@dueDateUnixTimeMs", (object?)dueDateUnixTimeMs ?? DBNull.Value),
            new SqlParameter("@status", (int)QueueMessageStatus.Pending),
            new SqlParameter("@key", (object?)key ?? DBNull.Value),
            new SqlParameter("@tag", (object?)tag ?? DBNull.Value)
        };

        string sql = """
            INSERT INTO [school_books].[QueueMessage] (
                [Type],
                [DueDateUnixTimeMs],
                [QueueMessageId],
                [Status],
                [Key],
                [Tag],
                [Payload],
                [FailedAttempts],
                [CreateDateUtc],
                [StatusDateUtc]
            )
            VALUES
            """;

        for (int i = 0; i < payloads.Length; i++)
        {
            string payload = payloads[i];

            if (i > 0)
            {
                sql += "\n,";
            }

            sql += $"""
                 (
                    @type,
                    -- queue message always use UTC to prevent problems with daylight saving time
                    COALESCE(@dueDateUnixTimeMs, DATEDIFF_BIG(MILLISECOND, '1970-01-01', GETUTCDATE())),
                    NEXT VALUE FOR [school_books].[QueueMessageIdSequence],
                    @status,
                    @key,
                    @tag,
                    @payload{i},
                    0,
                    GETUTCDATE(),
                    GETUTCDATE()
                )
                """;

            parameters.Add(new SqlParameter($"@payload{i}", payload));
        }

        await this.ExecuteSqlRawAsync(
            sql,
            parameters.ToArray(),
            ct);
    }

    public async Task<IEnumerable<FetchNextVO>> FetchNextAsync(
        QueueMessageType type,
        int nextCount,
        TimeSpan failedAttemptTimeout,
        CancellationToken ct)
    {
        var maxInterval = DateTime.UtcNow - failedAttemptTimeout;

        string sql = $"""
                      WITH m AS (
                          SELECT TOP(@nextCount)
                              [Type],
                              [DueDateUnixTimeMs],
                              [QueueMessageId],
                              [Payload],
                              [Status],
                              [FailedAttempts]
                          FROM [school_books].[QueueMessage]
                          WITH (UPDLOCK, ROWLOCK, READPAST)
                          WHERE
                              [Type] = @type AND
                              [DueDateUnixTimeMs] < @now AND
                              [Status] = {(int)QueueMessageStatus.Pending} AND
                              ([FailedAttempts] = 0 OR
                                  [StatusDateUtc] < @maxInterval)
                          ORDER BY
                              [DueDateUnixTimeMs] ASC
                      )
                      UPDATE m SET [Status] = {(int)QueueMessageStatus.Processing}
                      OUTPUT INSERTED.*
                      """;

        return await this.unitOfWork.DbContext.Set<FetchNextVO>()
            .FromSqlRaw(sql,
                new SqlParameter("type", type),
                new SqlParameter("now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()),
                new SqlParameter("nextCount", nextCount),
                new SqlParameter("maxInterval", maxInterval))
            .ToListAsync(ct);
    }

    public async Task MakePendingAsync(
        QueueMessageType type,
        long startDateUnixTimeMilliseconds,
        long endDateUnixTimeMilliseconds,
        int[] queueMessageIds,
        CancellationToken ct)
    {
        string sql = $"""
            UPDATE [school_books].[QueueMessage] SET
                [Status] = {(int)QueueMessageStatus.Pending}
            WHERE
                [Type] = @type AND
                [DueDateUnixTimeMs] BETWEEN @startDateUnixTimeMilliseconds AND @endDateUnixTimeMilliseconds AND
                [QueueMessageId] IN (SELECT [Id] FROM OPENJSON(@idsJsonArray) WITH ([Id] INT '$')) AND
                [Status] = {(int)QueueMessageStatus.Processing}
            SELECT @@ROWCOUNT
            """;

        int rowCount = await this.ExecuteSqlRawAsync(
            sql,
            new[]
            {
                new SqlParameter("type", type),
                new SqlParameter("startDateUnixTimeMilliseconds", startDateUnixTimeMilliseconds),
                new SqlParameter("endDateUnixTimeMilliseconds", endDateUnixTimeMilliseconds),
                new SqlParameter("idsJsonArray", JsonSerializer.Serialize(queueMessageIds)),
            },
            ct);

        if (rowCount != queueMessageIds.Length)
        {
            this.logger.LogWarning(
                "Unexpected number of rows updated when setting Status to Pending. Expected {count} queue messages to be updated, but {rowCount} were updated.",
                queueMessageIds.Length,
                rowCount);
        }
    }

    public async Task SetStatusProcessedAsync(
        QueueMessageType type,
        long dueDateUnixTimeMs,
        int queueMessageId,
        CancellationToken ct)
    {
        string sql = $"""
            UPDATE [school_books].[QueueMessage] SET
                [Status] = {(int)QueueMessageStatus.Processed},
                [StatusDateUtc] = GETUTCDATE()
            WHERE
                [Type] = @type AND
                [DueDateUnixTimeMs] = @dueDateUnixTimeMs AND
                [QueueMessageId] = @queueMessageId AND
                [Status] = {(int)QueueMessageStatus.Processing}
            """;

        int rowCount = await this.ExecuteSqlRawAsync(
            sql,
            new[]
            {
                new SqlParameter("type", type),
                new SqlParameter("dueDateUnixTimeMs", dueDateUnixTimeMs),
                new SqlParameter("queueMessageId", queueMessageId),
            },
            ct);

        if (rowCount != 1)
        {
            this.logger.LogWarning(
                "Unexpected number of rows updated when setting Status to Processed. Expected 1 queue messages to be updated, but {rowCount} were updated.",
                rowCount);
        }
    }

    public async Task SetErrorAsync(
        QueueMessageType type,
        long dueDateUnixTimeMs,
        int queueMessageId,
        int maxFailedAttempts,
        string error,
        CancellationToken ct)
    {
        string sql = $"""
            UPDATE [school_books].[QueueMessage] SET
                [Status] =
                    CASE WHEN [FailedAttempts] < {maxFailedAttempts - 1}
                        THEN {(int)QueueMessageStatus.Pending}
                        ELSE {(int)QueueMessageStatus.Errored}
                    END,
                [StatusDateUtc] = GETUTCDATE(),
                [FailedAttempts] = [FailedAttempts] + 1,
                [FailedAttemptsErrors] =
                    JSON_MODIFY(
                        COALESCE([FailedAttemptsErrors], '[]'),
                        'append $',
                        @error)
            WHERE
                [Type] = @type AND
                [DueDateUnixTimeMs] = @dueDateUnixTimeMs AND
                [QueueMessageId] = @queueMessageId AND
                [Status] = {(int)QueueMessageStatus.Processing}
            """;

        int rowCount = await this.ExecuteSqlRawAsync(
            sql,
            new[]
            {
                new SqlParameter("type", type),
                new SqlParameter("dueDateUnixTimeMs", dueDateUnixTimeMs),
                new SqlParameter("queueMessageId", queueMessageId),
                new SqlParameter("error", error),
            },
            ct);

        if (rowCount != 1)
        {
            this.logger.LogWarning(
                "Unexpected number of rows updated when setting error. Expected 1 queue messages to be updated, but {rowCount} were updated.",
                rowCount);
        }
    }

    public async Task<IEnumerable<QueueMessage>> CancelMessagesAsync(
        QueueMessageType type,
        string tag,
        CancellationToken ct)
    {
        var dueDate = new DateTimeOffset(DateTime.Now.AddHours(-1)).ToUnixTimeMilliseconds();

        string sql = $"""
                      WITH m AS (
                         SELECT *
                         FROM [school_books].[QueueMessage]
                         WITH (UPDLOCK, ROWLOCK, READPAST)
                         WHERE
                            [Type] = @type AND
                            [DueDateUnixTimeMs] > @dueDate AND
                            [Status] = {(int)QueueMessageStatus.Pending} AND
                            [Tag] = @tag
                      )
                      UPDATE m SET [Status] = {(int)QueueMessageStatus.Cancelled}, [StatusDateUtc] = GETUTCDATE()
                      OUTPUT INSERTED.*
                      """;

        return await this.unitOfWork.DbContext.Set<QueueMessage>()
            .FromSqlRaw(sql,
                new SqlParameter("type", type),
                new SqlParameter("tag", tag),
                new SqlParameter("dueDate", dueDate))
            .AsNoTracking()
            .ToListAsync(ct);
    }

    private async Task<int> ExecuteSqlRawAsync(
        string sql,
        SqlParameter[] parameters,
        CancellationToken ct)
    {
        await using SqlConnection connection = new(this.dataOptions.GetConnectionString());
        await connection.OpenAsync(ct);

        await using SqlCommand cmd = connection.CreateCommand();

        cmd.CommandText = sql;
        cmd.Parameters.AddRange(parameters);
        cmd.RetryLogicProvider =
            SqlConfigurableRetryFactory.CreateExponentialRetryProvider(
                new SqlRetryLogicOption()
                {
                    // 1 try + 5 retries
                    // retrying in 2, 4, 8, 16, 30 (not 32 as max is 30) seconds
                    // for a maximum delay of up to a minute
                    NumberOfTries = 6,
                    MinTimeInterval = TimeSpan.FromSeconds(2),
                    MaxTimeInterval = TimeSpan.FromSeconds(30),
                    TransientErrors = transientErrors,
                });

        return await cmd.ExecuteNonQueryAsync(ct);
    }

    private static readonly int[] transientErrors = new[]
    {
        // copied from https://github.com/dotnet/SqlClient/blob/main/src/Microsoft.Data.SqlClient/src/Microsoft/Data/SqlClient/Reliability/SqlConfigurableRetryFactory.cs
        1204,   // The instance of the SQL Server Database Engine cannot obtain a LOCK resource at this time. Rerun your statement when there are fewer active users. Ask the database administrator to check the lock and memory configuration for this instance, or to check for long-running transactions.
        1205,   // Transaction (Process ID) was deadlocked on resources with another process and has been chosen as the deadlock victim. Rerun the transaction
        1222,   // Lock request time out period exceeded.
        49918,  // Cannot process request. Not enough resources to process request.
        49919,  // Cannot process create or update request. Too many create or update operations in progress for subscription "%ld".
        49920,  // Cannot process request. Too many operations in progress for subscription "%ld".
        4060,   // Cannot open database "%.*ls" requested by the login. The login failed.
        4221,   // Login to read-secondary failed due to long wait on 'HADR_DATABASE_WAIT_FOR_TRANSITION_TO_VERSIONING'. The replica is not available for login because row versions are missing for transactions that were in-flight when the replica was recycled. The issue can be resolved by rolling back or committing the active transactions on the primary replica. Occurrences of this condition can be minimized by avoiding long write transactions on the primary.
        40143,  // The service has encountered an error processing your request. Please try again.
        40613,  // Database '%.*ls' on server '%.*ls' is not currently available. Please retry the connection later. If the problem persists, contact customer support, and provide them the session tracing ID of '%.*ls'.
        40501,  // The service is currently busy. Retry the request after 10 seconds. Incident ID: %ls. Code: %d.
        40540,  // The service has encountered an error processing your request. Please try again.
        40197,  // The service has encountered an error processing your request. Please try again. Error code %d.
        42108,  // Can not connect to the SQL pool since it is paused. Please resume the SQL pool and try again.
        42109,  // The SQL pool is warming up. Please try again.
        10929,  // Resource ID: %d. The %s minimum guarantee is %d, maximum limit is %d and the current usage for the database is %d. However, the server is currently too busy to support requests greater than %d for this database. For more information, see http://go.microsoft.com/fwlink/?LinkId=267637. Otherwise, please try again later.
        10928,  // Resource ID: %d. The %s limit for the database is %d and has been reached. For more information, see http://go.microsoft.com/fwlink/?LinkId=267637.
        10060,  // An error has occurred while establishing a connection to the server. When connecting to SQL Server, this failure may be caused by the fact that under the default settings SQL Server does not allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.) (Microsoft SQL Server, Error: 10060)
        997,    // A connection was successfully established with the server, but then an error occurred during the login process. (provider: Named Pipes Provider, error: 0 - Overlapped I/O operation is in progress)
        233,    // A connection was successfully established with the server, but then an error occurred during the login process. (provider: Shared Memory Provider, error: 0 - No process is on the other end of the pipe.) (Microsoft SQL Server, Error: 233)

        // added by us
        -2 // Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding. The statement has been terminated.
    };
}
