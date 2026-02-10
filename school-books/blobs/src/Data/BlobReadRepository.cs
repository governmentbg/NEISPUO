namespace SB.Blobs;

using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

public class BlobReadRepository
{
    private SqlConnection connection;
    public BlobReadRepository(SqlConnection connection)
    {
        this.connection = connection;
    }

    public record BlobInfoVO(string Filename, int BlobContentId, long Size, byte[] Version);
    public async Task<BlobInfoVO> GetBlobInfoAsync(int blobId, CancellationToken ct)
    {
        await using SqlCommand cmd = this.connection.CreateCommand();
        cmd.CommandText =
            $@"SELECT b.[FileName], bc.[BlobContentId], bc.[Size], bc.[Version]
                FROM [blobs].[Blobs] b
                JOIN [blobs].[BlobContents] bc on b.BlobContentId = bc.BlobContentId
                WHERE b.BlobId = @blobId";
        cmd.Parameters.AddWithValue("@blobId", blobId);

        await using SqlDataReader reader = await cmd.ExecuteReaderAsync(ct);

        if (!reader.HasRows)
        {
            return null;
        }

        await reader.ReadAsync(ct);

        string filename = reader.GetString(reader.GetOrdinal("FileName"));
        int blobContentId = reader.GetInt32(reader.GetOrdinal("BlobContentId"));
        long size = reader.GetInt64(reader.GetOrdinal("Size"));
        byte[] version = new byte[8];
        reader.GetBytes(reader.GetOrdinal("Version"), 0, version, 0, 8);

        return new BlobInfoVO(filename, blobContentId, size, version);
    }

    public async Task CopyBlobContentToStreamAsync(
        int blobContentId,
        Stream destination,
        int bufferSize,
        CancellationToken ct)
    {
        await this.CopyBlobContentToStreamInternalAsync(blobContentId, destination, bufferSize, null, null, ct);
    }

    public async Task CopyBlobContentToStreamAsync(
        int blobContentId,
        Stream destination,
        int bufferSize,
        long offset,
        long length,
        CancellationToken ct)
    {
        await this.CopyBlobContentToStreamInternalAsync(blobContentId, destination, bufferSize, offset, length, ct);
    }

    private async Task CopyBlobContentToStreamInternalAsync(
        int blobContentId,
        Stream destination,
        int bufferSize,
        long? offset,
        long? length,
        CancellationToken ct)
    {
        await using SqlCommand cmd = this.connection.CreateCommand();

        if (offset != null && length != null)
        {
            cmd.CommandText =
                $@"SELECT SUBSTRING([Content], @offset , @length)
                    FROM [blobs].[BlobContents]
                    WHERE BlobContentId = @blobContentId";
            cmd.Parameters.AddWithValue("@offset", offset.Value + 1); // SUBSTRING start is 1 based
            cmd.Parameters.AddWithValue("@length", length.Value);
            cmd.Parameters.AddWithValue("@blobContentId", blobContentId);
        }
        else
        {
            cmd.CommandText =
                $@"SELECT [Content]
                    FROM [blobs].[BlobContents]
                    WHERE BlobContentId = @blobContentId";
            cmd.Parameters.AddWithValue("@blobContentId", blobContentId);
        }

        // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
        // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
        await using SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, ct);
        if (!(await reader.ReadAsync(ct)) || (await reader.IsDBNullAsync(0, ct)))
        {
            return;
        }

        using Stream data = reader.GetStream(0);
        await data.CopyToAsync(destination, bufferSize, ct);
    }
}
