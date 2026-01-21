namespace SB.Blobs;

using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class BlobWriteStream : Stream
{
    private const int CHUNK_SIZE = 100 * 8040;

    private SqlCommand cmdAppendChunk;
    private SqlCommand cmdFirstChunk;
    private SqlParameter paramChunk;
    private SqlParameter paramLength;
    private ILogger logger;

    private int chunkIndex;
    private byte[] chunkBuffer = new byte[CHUNK_SIZE];
    private bool isFirstChunk = true;

    public BlobWriteStream(
        SqlConnection connection,
        ILoggerFactory loggerFactory,
        int blobContentId)
    {
        this.logger = loggerFactory?.CreateLogger<BlobWriteStream>();

        this.cmdFirstChunk = new SqlCommand(
            $@"UPDATE [blobs].[BlobContents]
                SET [Content] = @firstChunk, [ModifyDate] = GETDATE()
                WHERE BlobContentId = @blobContentId",
            connection);
        this.cmdFirstChunk.Parameters.AddWithValue("@blobContentId", blobContentId);

        this.cmdAppendChunk = new SqlCommand(
            $@"UPDATE [blobs].[BlobContents]
                SET [Content].WRITE(@chunk, NULL, @length), [ModifyDate] = GETDATE()
                WHERE BlobContentId = @blobContentId",
            connection);
        this.cmdAppendChunk.Parameters.AddWithValue("@blobContentId", blobContentId);

        this.paramChunk = new SqlParameter("@chunk", SqlDbType.VarBinary);
        this.paramLength = new SqlParameter("@length", SqlDbType.BigInt);
        this.cmdAppendChunk.Parameters.Add(this.paramChunk);
        this.cmdAppendChunk.Parameters.Add(this.paramLength);
    }

    public override bool CanRead
    {
        get { return false; }
    }

    public override bool CanSeek
    {
        get { return false; }
    }

    public override bool CanWrite
    {
        get { return true; }
    }

    public override long Length
    {
        get { throw new InvalidOperationException(); }
    }

    public override long Position
    {
        get
        {
            throw new InvalidOperationException();
        }

        set
        {
            throw new InvalidOperationException();
        }
    }

    private bool WriteChunk(byte[] buffer, ref int offset, ref int count)
    {
        int take = Math.Min(count, CHUNK_SIZE - this.chunkIndex);

        Buffer.BlockCopy(buffer, offset, this.chunkBuffer, this.chunkIndex, take);
        this.chunkIndex += take;
        offset += take;
        count -= take;

        return this.chunkIndex == CHUNK_SIZE;
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        while (count > 0)
        {
            if (this.WriteChunk(buffer, ref offset, ref count))
            {
                await this.FlushAsync(cancellationToken);
            }
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        while (count > 0)
        {
            if (this.WriteChunk(buffer, ref offset, ref count))
            {
                this.Flush();
            }
        }
    }

    private byte[] GetChunk()
    {
        if (this.chunkIndex == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] chunk;

        if (this.chunkIndex < CHUNK_SIZE)
        {
            chunk = new byte[this.chunkIndex];
            Buffer.BlockCopy(this.chunkBuffer, 0, chunk, 0, this.chunkIndex);
        }
        else
        {
            // if we have to write the whole chunk reuse its array
            chunk = this.chunkBuffer;
        }

        this.chunkIndex = 0;

        return chunk;
    }

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        byte[] chunk = this.GetChunk();

        if (chunk.Length == 0)
        {
            return;
        }

        if (this.isFirstChunk)
        {
            this.cmdFirstChunk.Parameters.AddWithValue("@firstChunk", chunk);
            await this.ExecuteNonQueryAsync(this.cmdFirstChunk, cancellationToken);

            this.isFirstChunk = false;
        }
        else
        {
            this.paramChunk.Value = chunk;
            this.paramLength.Value = this.chunkIndex;
            await this.ExecuteNonQueryAsync(this.cmdAppendChunk, cancellationToken);
        }

        this.logger?.LogTrace("Blob stream flushed");
    }

    public override void Flush()
    {
        byte[] chunk = this.GetChunk();

        if (chunk.Length == 0)
        {
            return;
        }

        if (this.isFirstChunk)
        {
            this.cmdFirstChunk.Parameters.AddWithValue("@firstChunk", chunk);
            this.ExecuteNonQuery(this.cmdFirstChunk);

            this.isFirstChunk = false;
        }
        else
        {
            this.paramChunk.Value = chunk;
            this.paramLength.Value = this.chunkIndex;
            this.ExecuteNonQuery(this.cmdAppendChunk);
        }

        this.logger?.LogTrace("Blob stream flushed");
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new InvalidOperationException();
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && this.cmdAppendChunk != null && this.cmdFirstChunk != null)
            {
                using (this.cmdFirstChunk)
                using (this.cmdAppendChunk)
                {
                    this.Flush();
                }
            }
        }
        finally
        {
            this.cmdFirstChunk = null;
            this.cmdAppendChunk = null;
            base.Dispose(disposing);
        }
    }

    private async Task<int> ExecuteNonQueryAsync(SqlCommand cmd, CancellationToken ct)
        => await this.ExecuteAsync(
            async (cmd1, ct1) => await cmd1.ExecuteNonQueryAsync(ct),
            cmd,
            ct);

    private async Task<T> ExecuteAsync<T>(
        Func<SqlCommand, CancellationToken, Task<T>> executor,
        SqlCommand cmd,
        CancellationToken ct)
    {
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

            this.logger?.LogError(ex, "ExecuteAsync failed:\n{CommandText}", cmd.CommandText);
            throw;
        }
    }

    private int ExecuteNonQuery(SqlCommand cmd)
        => this.Execute(
            cmd1 => cmd1.ExecuteNonQuery(),
            cmd);

    private T Execute<T>(
        Func<SqlCommand, T> executor,
        SqlCommand cmd)
    {
        try
        {
            return executor(cmd);
        }
        catch (Exception ex)
        {
            this.logger?.LogError(ex, "Execute failed:\n{CommandText}", cmd.CommandText);
            throw;
        }
    }
}
