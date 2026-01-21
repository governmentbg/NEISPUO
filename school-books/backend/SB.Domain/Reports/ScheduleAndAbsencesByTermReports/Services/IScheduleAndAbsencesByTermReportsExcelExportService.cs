namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IScheduleAndAbsencesByTermReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int scheduleAndAbsencesByTermReportId, Stream outputStream, CancellationToken ct);
}
