namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IExamsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int examsReportId, Stream outputStream, CancellationToken ct);
}
