namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IRegularGradePointAverageByClassesReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int regularGradePointAverageByClassesReportId, Stream outputStream, CancellationToken ct);
}
