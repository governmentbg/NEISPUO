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
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ScheduleAndAbsencesByTermReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IScheduleAndAbsencesByTermReportsQueryRepository scheduleAndAbsencesByTermReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await scheduleAndAbsencesByTermReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateScheduleAndAbsencesByTermReportCommand command,
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

    [HttpGet("{scheduleAndAbsencesByTermReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermReportId,
        [FromServices] IScheduleAndAbsencesByTermReportsQueryRepository scheduleAndAbsencesByTermReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByTermReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await scheduleAndAbsencesByTermReportsQueryRepository.GetAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ct);
    }

    [HttpGet("{scheduleAndAbsencesByTermReportId:int}/weeks")]
    public async Task<ActionResult<GetWeeksVO[]>> GetWeeksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermReportId,
        [FromServices] IScheduleAndAbsencesByTermReportsQueryRepository scheduleAndAbsencesByTermReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByTermReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await scheduleAndAbsencesByTermReportsQueryRepository.GetWeeksAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ct);
    }

    [HttpDelete("{scheduleAndAbsencesByTermReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermReportId,
        [FromServices] IMediator mediator,
        [FromServices] IScheduleAndAbsencesByTermReportsQueryRepository scheduleAndAbsencesByTermReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByTermReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveScheduleAndAbsencesByTermReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleAndAbsencesByTermReportId = scheduleAndAbsencesByTermReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{scheduleAndAbsencesByTermReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermReportId,
        [FromServices] IScheduleAndAbsencesByTermReportsExcelExportService scheduleAndAbsencesByTermReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await scheduleAndAbsencesByTermReportsExcelExportService.ExportAsync(schoolYear, instId, scheduleAndAbsencesByTermReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "отсъствия_теми_за_срок.xlsx"
        };
    }
}
