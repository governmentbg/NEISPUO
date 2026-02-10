namespace SB.Api;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;

public class PerformancesExcelController : BookController
{
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromServices] IPerformanceExcelExportService performanceExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await performanceExcelExportService.ExportAsync(schoolYear, instId, classBookId, false, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "изяви.xlsx"
        };
    }

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet]
    public async Task<FileResult> DownloadExcelFileForAllBooksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromServices] IPerformanceExcelExportService performanceExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await performanceExcelExportService.ExportAsync(schoolYear, instId, classBookId, true, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "изяви_всички_дневници.xlsx"
        };
    }
}
