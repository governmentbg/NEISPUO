namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IFinalGradePointAverageByClassesReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int finalGradePointAverageByClassesReportId, Stream outputStream, CancellationToken ct);
}
