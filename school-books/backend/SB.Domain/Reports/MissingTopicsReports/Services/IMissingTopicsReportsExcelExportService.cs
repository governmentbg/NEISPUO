namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IMissingTopicsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int missingTopicsReportId, Stream outputStream, CancellationToken ct);
}
