namespace SB.Blobs;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class BlobWriteRepository
{
    private SqlConnection connection;
    private ILoggerFactory loggerFactory;
    private ILogger logger;

    public BlobWriteRepository(
        SqlConnection connection,
        ILoggerFactory loggerFactory)
    {
        this.connection = connection;
        this.loggerFactory = loggerFactory;
        this.logger = loggerFactory?.CreateLogger<BlobWriteRepository>();
    }

    public record BlobInfo(int BlobId, long Size);
    public async Task<BlobInfo> DrainStreamAsync(Stream readableStream, string fileName, CancellationToken ct)
    {
        int blobContentId = await this.ExecuteScalarAsync<int>(
            $@"INSERT INTO [blobs].[BlobContents] ([BlobContentId], [Hash], [Size], [Content], [CreateDate], [ModifyDate])
                OUTPUT inserted.BlobContentId
                VALUES (NEXT VALUE FOR [blobs].[BlobContentsIdSequence], NULL, NULL, NULL, GETDATE(), GETDATE())",
            Array.Empty<SqlParameter>(),
            ct) ?? throw new Exception("The inserted ID should be returned.");

        using SHA256 sha256 = SHA256.Create();

        using (BlobWriteStream blobWriteStream = new(this.connection, this.loggerFactory, blobContentId))
        using (CryptoStream cryptoStream = new(blobWriteStream, sha256, CryptoStreamMode.Write))
        {
            await readableStream.CopyToAsync(cryptoStream, ct);
        }

        return await this.GetBlobIdAndSizeAsync(
            blobContentId,
            Convert.ToHexString(sha256.Hash),
            fileName,
            ct);
    }

    public async Task DeleteBlobAsync(int blobId, CancellationToken ct)
    {
        // TODO: remove the deletion of the blob content if a cleanup job is implemented
        await this.ExecuteNonQueryAsync(
            $@"DECLARE @blobContentIds TABLE(BlobContentId INT NOT NULL);

                DELETE FROM [blobs].[Blobs]
                OUTPUT deleted.BlobContentId INTO @blobContentIds
                WHERE BlobId = @blobId;

                DECLARE @blobContentId INT = (SELECT BlobContentId FROM @blobContentIds);

                IF ((SELECT COUNT(*) FROM [blobs].[Blobs] WHERE BlobContentId = @blobContentId) = 0)
                BEGIN
                    DELETE FROM [blobs].[BlobContents]
                    WHERE BlobContentId = @blobContentId;
                END;",
            new[]
            {
                new SqlParameter("@blobId", blobId),
            },
            ct);
    }

    private async Task<BlobInfo> GetBlobIdAndSizeAsync(int blobContentId, string hash, string fileName, CancellationToken ct)
    {
        long size = await this.ExecuteScalarAsync<long>(
                $@"SELECT DATALENGTH([Content])
                    FROM [blobs].[BlobContents]
                    WHERE BlobContentId = @blobContentId",
            new[]
            {
                new SqlParameter("@blobContentId", blobContentId)
            },
            ct)
            ?? throw new Exception("DrainStreamAsync should have inserted a BlobContent with that id.");

        async Task<int?> getExistingBlobContentId()
            => await this.ExecuteScalarAsync<int>(
                $@"SELECT BlobContentId
                    FROM [blobs].[BlobContents]
                    WHERE [Hash] = @hash AND [Size] = @size",
                new[]
                {
                    new SqlParameter("@hash", hash),
                    new SqlParameter("@size", size),
                },
                ct);

        int? existingBlobContentId = await getExistingBlobContentId();

        int finalBlobContentId;
        if (existingBlobContentId == null)
        {
            try
            {
                int affectedRows = await this.ExecuteNonQueryAsync(
                    $@"UPDATE [blobs].[BlobContents]
                        SET [Hash] = @hash, [Size] = @size, [ModifyDate] = GETDATE()
                        WHERE BlobContentId = @blobContentId",
                    new[]
                    {
                        new SqlParameter("@blobContentId", blobContentId),
                        new SqlParameter("@hash", hash),
                        new SqlParameter("@size", size),
                    },
                    ct);

                if (affectedRows == 1)
                {
                    finalBlobContentId = blobContentId;
                }
                else
                {
                    throw new Exception($"affectedRows should be such 1 but instead was {affectedRows}.");
                }
            }
            catch (SqlException sqlEx)
            when (sqlEx.Number == SqlServerErrorCodes.ViolationOfUniqueIndex &&
                sqlEx.Message.Contains("UQ_BlobContents_Hash_Size"))
            {
                // looks like someone just preempted us
                // in inserting this blob
                finalBlobContentId =
                    await getExistingBlobContentId()
                    ?? throw new Exception("There should be such a hash/size blob.");
            }
        }
        else
        {
            finalBlobContentId = existingBlobContentId.Value;
        }

        var blobId = await this.ExecuteScalarAsync<int>(
            $@"INSERT INTO [blobs].[Blobs] ([BlobId], [BlobContentId], [FileName], [CreateDate])
                OUTPUT inserted.BlobId
                VALUES (NEXT VALUE FOR [blobs].[BlobsIdSequence], @blobContentId, @fileName, GETDATE())",
            new[]
            {
                new SqlParameter("@blobContentId", finalBlobContentId),
                new SqlParameter("@fileName", fileName),
            },
            ct)
            ?? throw new Exception("The inserted ID should have been returned.");

        return new BlobInfo(blobId, size);
    }

    private async Task<T?> ExecuteScalarAsync<T>(string sql, SqlParameter[] parameters, CancellationToken ct)
        where T : struct
        => await this.ExecuteAsync(
            async (cmd, ct1) =>
            {
                var res = await cmd.ExecuteScalarAsync(ct1);
                if (res == DBNull.Value || res == null)
                {
                    return null;
                }

                return (T?)res;
            },
            sql,
            parameters,
            ct);

    private async Task<int> ExecuteNonQueryAsync(string sql, SqlParameter[] parameters, CancellationToken ct)
        => await this.ExecuteAsync(
            async (cmd, ct1) => await cmd.ExecuteNonQueryAsync(ct1),
            sql,
            parameters,
            ct);

    private async Task<T> ExecuteAsync<T>(
        Func<SqlCommand, CancellationToken, Task<T>> executor,
        string sql,
        SqlParameter[] parameters,
        CancellationToken ct)
    {
        await using SqlCommand cmd = this.connection.CreateCommand();

        cmd.CommandText = sql;
        cmd.Parameters.AddRange(parameters);

        try
        {
            return await executor(cmd, ct);
        }
        catch (Exception ex)
        {
            if (ct.IsCancellationRequested)
            {
                throw new OperationCanceledException(ex.Message, ex, ct);
            }

            this.logger?.LogError(ex, "ExecuteAsync failed:\n{command}", sql);
            throw;
        }
    }
}
