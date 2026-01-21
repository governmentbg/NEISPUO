namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IFinalGradePointAverageByStudentsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int finalGradePointAverageByStudentsReportId, Stream outputStream, CancellationToken ct);
}
