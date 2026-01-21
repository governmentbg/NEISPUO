namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ITopicPlanItemExcelService
{
    Task ExportAsync(int sysUserId, int topicPlanId, Stream outputStream, CancellationToken ct);

    Task<string[]> ImportFromExcelBlobAsync(int blobId, int topicPlanId, CancellationToken ct);
}
