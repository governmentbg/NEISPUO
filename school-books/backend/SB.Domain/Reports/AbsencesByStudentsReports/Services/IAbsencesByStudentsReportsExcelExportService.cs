namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IAbsencesByStudentsReportsExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int absencesByStudentsReportId, Stream outputStream, CancellationToken ct);
}
