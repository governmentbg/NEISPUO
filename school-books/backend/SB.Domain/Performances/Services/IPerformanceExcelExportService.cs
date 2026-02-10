namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IPerformanceExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int classBookId, bool isForAllBooks, Stream outputStream, CancellationToken ct);
}
