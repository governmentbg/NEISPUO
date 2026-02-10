namespace SB.Api;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class RegBookCertificatesExcelController : RegBooksExcelController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{basicDocumentId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int basicDocumentId,
        [FromServices]IRegBookCertificateExcelExportService regBookCertificateExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await regBookCertificateExcelExportService.ExportAsync(schoolYear, instId, basicDocumentId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "рег_книга_изд_удостоверения.xlsx"
        };
    }
}
