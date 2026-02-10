namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ISessionStudentsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int sessionStudentsReportId, Stream outputStream, CancellationToken ct);
}
