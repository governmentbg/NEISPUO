namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IScheduleAndAbsencesByMonthReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int scheduleAndAbsencesByMonthReportId, Stream outputStream, CancellationToken ct);
}
