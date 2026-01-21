namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ILectureSchedulesReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int lectureSchedulesReportId, Stream outputStream, CancellationToken ct);
}
