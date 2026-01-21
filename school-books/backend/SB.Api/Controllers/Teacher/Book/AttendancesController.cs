namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IAttendancesQueryRepository;

public class AttendancesController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllForMonthVO[]>> GetAllForMonthAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int year,
        [FromQuery]int month,
        [FromServices]IAttendancesQueryRepository attendancesQueryRepository,
        CancellationToken ct)
        => await attendancesQueryRepository.GetAllForMonthAsync(schoolYear, classBookId, year, month, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllForDateVO[]>> GetAllForDateAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]DateTime date,
        [FromServices]IAttendancesQueryRepository attendancesQueryRepository,
        CancellationToken ct)
        => await attendancesQueryRepository.GetAllForDateAsync(schoolYear, classBookId, date, ct);

    [HttpGet("{attendanceId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int attendanceId,
        [FromServices]IAttendancesQueryRepository attendancesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int sysUserId = token.SelectedRole.SysUserId;
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var attendance = await attendancesQueryRepository.GetAsync(schoolYear, classBookId, attendanceId, ct);

        attendance.HasUndoAccess =
            attendance.CreatedBySysUserId == sysUserId;
        attendance.HasExcuseAccess =
        attendance.HasRemoveAccess = hasClassBookAdminWriteAccess;

        return attendance;
    }

    [Authorize(Policy = Policies.AttendanceDateAccess)]
    [HttpPost("{date:datetime}")]
    public async Task CreateAttendancesAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]DateTime date,
        [FromBody]CreateAttendancesCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                Date = date,
            },
            ct);

    [HttpPost]
    public async Task<ActionResult> ExcuseAttendanceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]ExcuseAttendanceCommand command,
        [FromServices]IAttendancesQueryRepository attendancesQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasExcuseAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var attendance = !hasExcuseAccess && command.AttendanceId.HasValue ?
            await attendancesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                command.AttendanceId.Value,
                ct) :
            null;

        var hasUndoAccess =
            attendance != null &&
            attendance.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
            attendance.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

        if (!hasExcuseAccess && !hasUndoAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{attendanceId:int}")]
    public async Task<ActionResult> RemoveAttendanceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int attendanceId,
        [FromServices]IAttendancesQueryRepository attendancesQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasRemoveAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var attendance = !hasRemoveAccess ?
            await attendancesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                attendanceId,
                ct) :
            null;

        var hasUndoAccess =
            attendance != null &&
            attendance.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
            attendance.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

        if (!hasRemoveAccess && !hasUndoAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAttendanceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AttendanceId = attendanceId,
            },
            ct);

        return new OkResult();
    }

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost]
    public async Task RemoveAttendancesAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]RemoveAttendancesCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
            },
            ct);
}
