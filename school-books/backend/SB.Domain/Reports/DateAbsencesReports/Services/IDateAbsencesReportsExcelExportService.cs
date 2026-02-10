namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IDateAbsencesReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int dateAbsencesReportId, Stream outputStream, CancellationToken ct);
}
