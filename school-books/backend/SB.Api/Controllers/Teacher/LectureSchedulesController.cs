namespace SB.Api;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.ILectureSchedulesQueryRepository;

[DisallowWhenInstHasCBExtProvider]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class LectureSchedulesController
{
    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] string? teacherName,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        CancellationToken ct)
        => await lectureSchedulesQueryRepository.GetAllAsync(schoolYear, instId, teacherName, offset, limit, ct);

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetMySchedulesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();

        if (personId == null)
        {
            return TableResultVO.Empty<GetAllVO>();
        }

        return await lectureSchedulesQueryRepository.GetAllForTeacherPersonAsync(schoolYear, instId, personId.Value, offset, limit, ct);
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{lectureScheduleId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureScheduleId,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess =
            await authService.HasInstitutionAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Read,
                instId,
                ct);

        var lectureSchedule = await lectureSchedulesQueryRepository.GetAsync(schoolYear, instId, lectureScheduleId, ct);

        var personId = httpContextAccessor.GetPersonId();
        bool doesNotContainTeacher = personId == null || lectureSchedule.TeacherPersonId != personId;

        if (!hasInstitutionAdminReadAccess && doesNotContainTeacher)
        {
            return new ForbidResult();
        }

        return lectureSchedule;
    }

    [Authorize(Policy = Policies.InstitutionAccess)]
    [HttpGet("{lectureScheduleId:int}/schedule")]
    public async Task<ActionResult<GetScheduleVO>> GetScheduleAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureScheduleId,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        bool hasInstitutionAdminReadAccess =
            await authService.HasInstitutionAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Read,
                instId,
                ct);

        var personId = httpContextAccessor.GetPersonId();
        var doesNotContainTeacher =
            !hasInstitutionAdminReadAccess && // optimisation, skip the query as it is not needed
            (personId == null ||
            await lectureSchedulesQueryRepository.GetTeacherPersonIdAsync(schoolYear, lectureScheduleId, ct) != personId.Value);

        if (!hasInstitutionAdminReadAccess && doesNotContainTeacher)
        {
            return new ForbidResult();
        }

        return await lectureSchedulesQueryRepository.GetScheduleAsync(schoolYear, instId, lectureScheduleId, ct);
    }

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet("teacherSchedule/{teacherPersonId:int}/{startDate:datetime}/{endDate:datetime}")]
    public async Task<ActionResult<GetScheduleVO>> GetTeacherScheduleForPeriodAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int teacherPersonId,
        [FromRoute] DateTime startDate,
        [FromRoute] DateTime endDate,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        CancellationToken ct)
        => await lectureSchedulesQueryRepository.GetTeacherScheduleForPeriodAsync(schoolYear, instId, teacherPersonId, startDate, endDate, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpGet("{lectureScheduleId:int}/teacherSchedule")]
    public async Task<ActionResult<GetScheduleVO>> GetTeacherScheduleForLectureScheduleAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureScheduleId,
        [FromServices] ILectureSchedulesQueryRepository lectureSchedulesQueryRepository,
        CancellationToken ct)
        => await lectureSchedulesQueryRepository.GetTeacherScheduleForLectureScheduleAsync(schoolYear, instId, lectureScheduleId, ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpPost]
    public async Task<int> CreateLectureScheduleAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromBody] CreateLectureScheduleCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
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
    [HttpPost("{lectureScheduleId:int}")]
    public async Task UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureScheduleId,
        [FromBody] UpdateLectureScheduleCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                LectureScheduleId = lectureScheduleId,
            },
            ct);

    [Authorize(Policy = Policies.InstitutionAdminAccess)]
    [HttpDelete("{lectureScheduleId:int}")]
    public async Task RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int lectureScheduleId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveLectureScheduleCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                LectureScheduleId = lectureScheduleId,
            },
            ct);
}
