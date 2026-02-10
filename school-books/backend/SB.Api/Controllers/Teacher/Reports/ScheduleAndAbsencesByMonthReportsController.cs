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
using static SB.Domain.IScheduleAndAbsencesByMonthReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ScheduleAndAbsencesByMonthReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await scheduleAndAbsencesByMonthReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateScheduleAndAbsencesByMonthReportCommand command,
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

    [HttpGet("{scheduleAndAbsencesByMonthReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByMonthReportId,
        [FromServices] IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByMonthReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, scheduleAndAbsencesByMonthReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await scheduleAndAbsencesByMonthReportsQueryRepository.GetAsync(schoolYear, instId, scheduleAndAbsencesByMonthReportId, ct);
    }

    [HttpGet("{scheduleAndAbsencesByMonthReportId:int}/weeks")]
    public async Task<ActionResult<GetWeeksVO[]>> GetWeeksAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByMonthReportId,
        [FromServices] IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByMonthReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, scheduleAndAbsencesByMonthReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await scheduleAndAbsencesByMonthReportsQueryRepository.GetWeeksAsync(schoolYear, instId, scheduleAndAbsencesByMonthReportId, ct);
    }

    [HttpDelete("{scheduleAndAbsencesByMonthReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByMonthReportId,
        [FromServices] IMediator mediator,
        [FromServices] IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByMonthReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, scheduleAndAbsencesByMonthReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveScheduleAndAbsencesByMonthReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleAndAbsencesByMonthReportId = scheduleAndAbsencesByMonthReportId,
            },
            ct);

        return new NoContentResult();
    }

    [ProducesResponseType(typeof(FileResult), 200)]
    [Produces(contentType: OpenXmlExtensions.ExcelMimeType)]
    [HttpGet("{scheduleAndAbsencesByMonthReportId:int}")]
    public async Task<FileResult> DownloadExcelFileAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByMonthReportId,
        [FromServices] IScheduleAndAbsencesByMonthReportsExcelExportService scheduleAndAbsencesByMonthReportsExcelExportService,
        CancellationToken ct)
    {
        var ms = new MemoryStream();

        await scheduleAndAbsencesByMonthReportsExcelExportService.ExportAsync(schoolYear, instId, scheduleAndAbsencesByMonthReportId, ms, ct);

        ms.Seek(0, SeekOrigin.Begin);

        return new FileStreamResult(ms, OpenXmlExtensions.ExcelMimeType)
        {
            FileDownloadName = "отсъствия_теми_за_месец.xlsx"
        };
    }
}
