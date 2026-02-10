namespace SB.Api;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IScheduleAndAbsencesByTermAllClassesReportsQueryRepository;

[Authorize(Policy = Policies.ReportAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ScheduleAndAbsencesByTermAllClassesReportsController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] IScheduleAndAbsencesByTermAllClassesReportsQueryRepository scheduleAndAbsencesByTermAllClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await scheduleAndAbsencesByTermAllClassesReportsQueryRepository.GetAllAsync(
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
        [FromBody] CreateScheduleAndAbsencesByTermAllClassesReportCommand command,
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

    [HttpGet("{scheduleAndAbsencesByTermAllClassesReportId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermAllClassesReportId,
        [FromServices] IScheduleAndAbsencesByTermAllClassesReportsQueryRepository scheduleAndAbsencesByTermAllClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByTermAllClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, scheduleAndAbsencesByTermAllClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        return await scheduleAndAbsencesByTermAllClassesReportsQueryRepository.GetAsync(schoolYear, instId, scheduleAndAbsencesByTermAllClassesReportId, ct);
    }

    [HttpDelete("{scheduleAndAbsencesByTermAllClassesReportId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int scheduleAndAbsencesByTermAllClassesReportId,
        [FromServices] IMediator mediator,
        [FromServices] IScheduleAndAbsencesByTermAllClassesReportsQueryRepository scheduleAndAbsencesByTermAllClassesReportsQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        if (await scheduleAndAbsencesByTermAllClassesReportsQueryRepository.GetCreatedBySysUserIdAsync(schoolYear, scheduleAndAbsencesByTermAllClassesReportId, ct)
                != httpContextAccessor.GetSysUserId())
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveScheduleAndAbsencesByTermAllClassesReportCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ScheduleAndAbsencesByTermAllClassesReportId = scheduleAndAbsencesByTermAllClassesReportId,
            },
            ct);

        return new NoContentResult();
    }
}
