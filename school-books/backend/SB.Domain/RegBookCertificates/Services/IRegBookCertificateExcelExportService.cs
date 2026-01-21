namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

public interface IRegBookCertificateExcelExportService
{
    Task ExportAsync(int schoolYear, int instId, int basicDocumentId, Stream outputStream, CancellationToken ct);
}
