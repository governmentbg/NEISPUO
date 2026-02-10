namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IPrintService
{
    Task RenderHtmlAsync(string printParamsStr, TextWriter textWriter, CancellationToken ct);

    Task FinalizePrintAsProcessedAsync(string printParamsStr, int printId, int blobId, string? contentHash, CancellationToken ct);

    Task FinalizePrintAsErroredAsync(string printParamsStr, int printId, CancellationToken ct);
}
