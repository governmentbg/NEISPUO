namespace SB.Api;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IExamsReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ExamsReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamsReportsQueryRepository examsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await examsReportsQueryRepository.GetAllAsync(
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
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
                new CreateExamsReportCommand
                {
                    SchoolYear = schoolYear,
                    InstId = instId,
                    SysUserId = httpContextAccessor.GetSysUserId(),
                },
                ct);

    [HttpGet("{examsReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examsReportId,
        [FromServices] IExamsReportsQueryRepository examsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await examsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, examsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await examsReportsQueryRepository.GetAsync(schoolYear, instId, examsReportId, ct);
    }

    [HttpGet("{examsReportId:int}/items")]
    public async Task<ActionResult<TableResultVO<GetItemsVO>>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examsReportId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IExamsReportsQueryRepository examsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await examsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, examsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await examsReportsQueryRepository.GetItemsAsync(schoolYear, instId, examsReportId, offset, limit, ct);
    }

    [HttpDelete("{examsReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examsReportId,
        [FromServices] IMediator mediator,
        [FromServices] IExamsReportsQueryRepository examsReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await examsReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, examsReportId, ct)
            != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveExamsReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ExamsReportId = examsReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{examsReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int examsReportId,
        [FromServices] IExamsReportsExcelExportService examsReportExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await examsReportExcelExportService.ExportAsync(schoolYear, instId, examsReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "контролни_класни.xlsx"
        };
    }
}
