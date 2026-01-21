namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IGradelessStudentsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int gradelessStudentsReportId, Stream outputStream, CancellationToken ct);
}
