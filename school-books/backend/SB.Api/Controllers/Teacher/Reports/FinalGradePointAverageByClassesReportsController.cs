namespace SB.Api;

using MediatR;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IFinalGradePointAverageByClassesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class FinalGradePointAverageByClassesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await finalGradePointAverageByClassesReportsQueryRepository.GetAllAsync(
            schoolYear,
            instId,
            httpContextAccessor.GetSysUserId(),
            offset,
            limit,
            ct);

    [HttpPost]
    public async Task<ActionResult<int>> CreateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateFinalGradePointAverageByClassesReportCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        return await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
    }

    [HttpGet("{finalGradePointAverageByClassesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByClassesReportId,
        [FromServices] IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await finalGradePointAverageByClassesReportsQueryRepository.GetAsync(schoolYear, instId, finalGradePointAverageByClassesReportId, ct);
    }

    [HttpGet("{finalGradePointAverageByClassesReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByClassesReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await finalGradePointAverageByClassesReportsQueryRepository.GetItemsAsync(schoolYear, instId, finalGradePointAverageByClassesReportId, offset, limit, ct);
    }

    [HttpDelete("{finalGradePointAverageByClassesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByClassesReportId,
        [FromServices] IMediator mediator,
        [FromServices] IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveFinalGradePointAverageByClassesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                FinalGradePointAverageByClassesReportId = finalGradePointAverageByClassesReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{finalGradePointAverageByClassesReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByClassesReportId,
        [FromServices] IFinalGradePointAverageByClassesReportsExcelExportService finalGradePointAverageByClassesReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await finalGradePointAverageByClassesReportsExcelExportService.ExportAsync(schoolYear, instId, finalGradePointAverageByClassesReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "годишен-успех-от-срочни-годишни-оценки-по-класове.xlsx"
        };
    }
}
