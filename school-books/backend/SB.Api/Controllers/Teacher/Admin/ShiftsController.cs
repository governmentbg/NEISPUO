namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IShiftsQueryRepository;

[DisallowWhenInstHasCBExtProvider]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class ShiftsController
{
    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]IShiftsQueryRepository shiftsQueryRepository,
        CancellationToken ct)
        => await shiftsQueryRepository.GetAllAsync(schoolYear, instId, offset, limit, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpPost]
    public async Task<int> CreateShiftAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreateShiftCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = false
            },
            ct);

    // this enpoint is used by the schedule dialog
    // and should be accessible by the LeadTeacher
    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{shiftId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int shiftId,
        [FromServices]IShiftsQueryRepository shiftsQueryRepository,
        CancellationToken ct)
        => await shiftsQueryRepository.GetAsync(schoolYear, shiftId, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet("{shiftId:int}")]
    public async Task<ActionResult<GetHoursUsedInScheduleVO[]>> GetHoursUsedInScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int shiftId,
        [FromServices]IShiftsQueryRepository shiftsQueryRepository,
        CancellationToken ct)
        => await shiftsQueryRepository.GetHoursUsedInScheduleAsync(schoolYear, instId, shiftId, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpPost("{shiftId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int shiftId,
        [FromBody]UpdateShiftCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = false,
                ShiftId = shiftId,
            },
            ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpDelete("{shiftId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int shiftId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveShiftCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                ShiftId = shiftId,
            },
            ct);
}
