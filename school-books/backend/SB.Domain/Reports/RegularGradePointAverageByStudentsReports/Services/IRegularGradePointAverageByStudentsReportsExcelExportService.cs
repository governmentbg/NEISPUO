namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IRegularGradePointAverageByStudentsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int regularGradePointAverageByStudentsReportId, Stream outputStream, CancellationToken ct);
}
