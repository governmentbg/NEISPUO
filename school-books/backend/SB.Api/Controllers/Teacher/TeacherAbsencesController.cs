namespace SB.Api;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ITeacherAbsencesQueryRepository;

[DisallowWhenInstHasCBExtProvider]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class TeacherAbsencesController
{
    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]string? teacherName,
        [FromQuery]string? replTeacherName,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        CancellationToken ct)
        =>  await teacherAbsencesQueryRepository.GetAllAsync(schoolYear, instId, teacherName, replTeacherName, offset, limit, ct);

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetMyAbsencesAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]string? replTeacherName,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();

        if (personId == null)
        {
            return TableResultVO.Empty<GetAllVO>();
        }

        return await teacherAbsencesQueryRepository.GetAllForAbsenteePersonAsync(schoolYear, instId, personId.Value, replTeacherName, offset, limit, ct);
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetMyReplacementsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromQuery]string? teacherName,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();

        if (personId == null)
        {
            return TableResultVO.Empty<GetAllVO>();
        }

        return await teacherAbsencesQueryRepository.GetAllForReplacementPersonAsync(schoolYear, instId, personId.Value, teacherName, offset, limit, ct);
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{teacherAbsenceId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherAbsenceId,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess =
            await authService.HasInstitutionAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Read,
                instId,
                ct);

        var teacherAbsence = await teacherAbsencesQueryRepository.GetAsync(schoolYear, instId, teacherAbsenceId, ct);

        var personId = httpContextAccessor.GetPersonId();
        var containsTeacher =
            personId.HasValue &&
            (teacherAbsence.TeacherPersonId == personId
                || teacherAbsence.Hours.Any(h => h.ReplTeacherPersonId == personId));

        if (!hasInstitutionAdminReadAccess && !containsTeacher)
        {
            return new ForbidResult();
        }

        return teacherAbsence;
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{teacherAbsenceId:int}/schedule")]
    public async Task<ActionResult<GetScheduleVO>> GetScheduleAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherAbsenceId,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess =
            await authService.HasInstitutionAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Read,
                instId,
                ct);

        var personId = httpContextAccessor.GetPersonId();
        var containsTeacher =
            !hasInstitutionAdminReadAccess && // optimisation, skip the query as it is not needed
            personId.HasValue &&
            await teacherAbsencesQueryRepository.ContainsTeacherAsync(schoolYear, teacherAbsenceId, personId.Value, ct);

        if (!hasInstitutionAdminReadAccess && !containsTeacher)
        {
            return new ForbidResult();
        }

        return await teacherAbsencesQueryRepository.GetScheduleAsync(schoolYear, instId, teacherAbsenceId, ct);
    }

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet("teacherSchedule/{teacherPersonId:int}/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<GetScheduleVO>> GetTeacherScheduleForPeriodAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherPersonId,
        [FromRoute]DateTime startDate,
        [FromRoute]DateTime endDate,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        CancellationToken ct)
        => await teacherAbsencesQueryRepository.GetTeacherScheduleForPeriodAsync(schoolYear, instId, teacherPersonId, startDate, endDate, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet("{teacherAbsenceId:int}/teacherSchedule")]
    public async Task<ActionResult<GetScheduleVO>> GetTeacherScheduleForAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherAbsenceId,
        [FromServices]ITeacherAbsencesQueryRepository teacherAbsencesQueryRepository,
        CancellationToken ct)
        => await teacherAbsencesQueryRepository.GetTeacherScheduleForAbsenceAsync(schoolYear, instId, teacherAbsenceId, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpPost]
    public async Task<int> CreateTeacherAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromBody]CreateTeacherAbsenceCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId()
            },
            ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpPost("{teacherAbsenceId:int}")]
    public async Task UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherAbsenceId,
        [FromBody]UpdateTeacherAbsenceCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TeacherAbsenceId = teacherAbsenceId,
            },
            ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpDelete("{teacherAbsenceId:int}")]
    public async Task RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int teacherAbsenceId,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveTeacherAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                TeacherAbsenceId = teacherAbsenceId,
            },
            ct);
}
