namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IStudentsAtRiskOfDroppingOutReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int studentsAtRiskOfDroppingOutReportId, Stream outputStream, CancellationToken ct);
}
