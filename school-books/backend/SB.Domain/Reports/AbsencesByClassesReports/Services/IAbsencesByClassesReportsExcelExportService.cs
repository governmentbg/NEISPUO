namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IAbsencesByClassesReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int absencesByClassesReportId, Stream outputStream, CancellationToken ct);
}
