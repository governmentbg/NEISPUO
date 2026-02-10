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
using static SB.Domain.IDateAbsencesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class DateAbsencesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await dateAbsencesReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateDateAbsencesReportCommand command,
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

    [HttpGet("{dateAbsencesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int dateAbsencesReportId,
        [FromServices] IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await dateAbsencesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, dateAbsencesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await dateAbsencesReportsQueryRepository.GetAsync(schoolYear, instId, dateAbsencesReportId, ct);
    }

    [HttpGet("{dateAbsencesReportId:int}/items")]
    public async Task<ActionResult<GetItemsVO[]>> GetItemsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int dateAbsencesReportId,
        [FromServices] IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await dateAbsencesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, dateAbsencesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await dateAbsencesReportsQueryRepository.GetItemsAsync(schoolYear, dateAbsencesReportId, ct);
    }

    [HttpDelete("{dateAbsencesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int dateAbsencesReportId,
        [FromServices] IMediator mediator,
        [FromServices] IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await dateAbsencesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, dateAbsencesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveDateAbsencesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                DateAbsencesReportId = dateAbsencesReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{dateAbsencesReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int dateAbsencesReportId,
        [FromServices] IDateAbsencesReportsExcelExportService dateAbsencesReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await dateAbsencesReportsExcelExportService.ExportAsync(schoolYear, instId, dateAbsencesReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "отсъстващи_за_деня.xlsx"
        };
    }
}
