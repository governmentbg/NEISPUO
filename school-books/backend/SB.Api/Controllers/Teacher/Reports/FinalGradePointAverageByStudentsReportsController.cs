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
using static SB.Domain.IFinalGradePointAverageByStudentsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class FinalGradePointAverageByStudentsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await finalGradePointAverageByStudentsReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateFinalGradePointAverageByStudentsReportCommand command,
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

    [HttpGet("{finalGradePointAverageByStudentsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByStudentsReportId,
        [FromServices] IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await finalGradePointAverageByStudentsReportsQueryRepository.GetAsync(schoolYear, instId, finalGradePointAverageByStudentsReportId, ct);
    }

    [HttpGet("{finalGradePointAverageByStudentsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByStudentsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await finalGradePointAverageByStudentsReportsQueryRepository.GetItemsAsync(schoolYear, instId, finalGradePointAverageByStudentsReportId, offset, limit, ct);
    }

    [HttpDelete("{finalGradePointAverageByStudentsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByStudentsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await finalGradePointAverageByStudentsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, finalGradePointAverageByStudentsReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveFinalGradePointAverageByStudentsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                FinalGradePointAverageByStudentsReportId = finalGradePointAverageByStudentsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{finalGradePointAverageByStudentsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int finalGradePointAverageByStudentsReportId,
        [FromServices] IFinalGradePointAverageByStudentsReportsExcelExportService finalGradePointAverageByStudentsReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await finalGradePointAverageByStudentsReportsExcelExportService.ExportAsync(schoolYear, instId, finalGradePointAverageByStudentsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "годишен-успех-от-срочни-годишни-оценки-по-ученици.xlsx"
        };
    }
}
