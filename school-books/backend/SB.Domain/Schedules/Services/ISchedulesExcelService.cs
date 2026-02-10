namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ISchedulesExcelService
{
    Task GetScheduleUsedHoursTableAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        int day,
        Stream outputStream,
        CancellationToken ct);
}
