namespace SB.Api;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class SpbsBookExcelController : RegBooksExcelController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{recordSchoolYear:int}")]
    public async Task<FileStreamResult> DownloadSpbsBookExcelFileAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int recordSchoolYear,
        [FromServices]ISpbsBookExcelExportService spbsBookExcelExportService,
        CancellationToken ct)
    {
        var excelStream = new MemoryStream(); // this stream is closed from asp.net

        await spbsBookExcelExportService.ExportAsync(schoolYear, instId, recordSchoolYear, excelStream, ct);

        excelStream.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(excelStream, OpenXmlExtensions.ExcelMimeType) { FileDownloadName = "книга за движението на учениците от СПИ/ВУИ.xlsx" };
    }
}
