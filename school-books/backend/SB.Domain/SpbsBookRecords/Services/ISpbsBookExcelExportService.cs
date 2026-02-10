namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface ISpbsBookExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int recordSchoolYear, Stream outputStream, CancellationToken ct);
}
