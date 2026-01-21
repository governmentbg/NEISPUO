namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAbsencesQueryRepository;

public class AbsencesController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllForClassBookVO[]>> GetAllForClassBookAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? curriculumId,
        [FromQuery]DateTime? fromDate,
        [FromQuery]DateTime? toDate,
        [FromServices]IAbsencesQueryRepository absencesQueryRepository,
        CancellationToken ct)
        => await absencesQueryRepository.GetAllForClassBookAsync(schoolYear, classBookId, curriculumId, fromDate, toDate, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllForStudentAndTypeVO[]>> GetAllForStudentAndTypeAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int personId,
        [FromQuery]AbsenceType type,
        [FromQuery]int? curriculumId,
        [FromQuery]DateTime? fromDate,
        [FromQuery]DateTime? toDate,
        [FromServices]IAbsencesQueryRepository absencesQueryRepository,
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

        var absences = await absencesQueryRepository.GetAllForStudentAndTypeAsync(
            schoolYear,
            classBookId,
            personId,
            type,
            curriculumId,
            fromDate,
            toDate,
            ct);

        foreach (var absence in absences)
        {
            absence.HasUndoAccess =
                absence.CreatedBySysUserId == sysUserId;
            absence.HasExcuseAccess =
            absence.HasRemoveAccess =
                hasClassBookAdminWriteAccess;
        }

        return absences;
    }

    [HttpGet]
    public async Task<ActionResult<GetAllForWeekVO[]>> GetAllForWeekAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int year,
        [FromQuery]int weekNumber,
        [FromServices]IAbsencesQueryRepository absencesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        int sysUserId = httpContextAccessor.GetSysUserId();

        var absences = await absencesQueryRepository.GetAllForWeekAsync(schoolYear, classBookId, year, weekNumber, ct);

        foreach (var absence in absences)
        {
            absence.HasUndoAccess = absence.CreatedBySysUserId == sysUserId;
        }

        return absences;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAbsenceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] CreateAbsenceCommand command,
        [FromServices] IAbsencesQueryRepository absencesQueryRepository,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        foreach (int scheduleLessonId in command.Absences?.Select(a => a.ScheduleLessonId).WhereNotNull() ?? Array.Empty<int>())
        {
            if (!await authService.HasScheduleLessonAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                scheduleLessonId,
                ct))
            {
                return new ForbidResult();
            }
        }

        var convertToLateAbsences = command.Absences?.Select(a => a.ConvertToLateId).WhereNotNull() ?? Array.Empty<int>();
        var undoAbsences = command.Absences?.Select(a => a.UndoAbsenceId).WhereNotNull() ?? Array.Empty<int>();

        foreach (var absenceId in convertToLateAbsences.Concat(undoAbsences))
        {
            var absence = await absencesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                absenceId,
                ct);

            var hasUndoAccess =
                absence != null &&
                absence.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
                absence.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

            if (!hasUndoAccess)
            {
                return new ForbidResult();
            }
        }

        _ = await mediator.Send(
            command with
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId()
            },
            ct);

        return new OkResult();
    }

    [HttpPost("{absenceId:int}")]
    public async Task<ActionResult> ConvertToLateAbsenceAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int absenceId,
        [FromServices] IAbsencesQueryRepository absencesQueryRepository,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var absence = await absencesQueryRepository.GetAsync(
            schoolYear,
            classBookId,
            absenceId,
            ct);

        var hasUndoAccess =
            absence != null &&
            absence.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
            absence.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

        if (!hasUndoAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new ConvertToLateAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                AbsenceId = absenceId,
            },
            ct);

        return new OkResult();
    }

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost("{absenceId:int}")]
    public async Task ExcuseAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int absenceId,
        [FromBody]ExcuseAbsenceCommand command,
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

                AbsenceId = absenceId,
            },
            ct);

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost]
    public async Task ExcuseAbsencesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] ExcuseAbsencesCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
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

    [HttpDelete("{absenceId:int}")]
    public async Task<ActionResult> RemoveAbsenceAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int absenceId,
        [FromServices]IAbsencesQueryRepository absencesQueryRepository,
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

        var absence = !hasRemoveAccess ?
            await absencesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                absenceId,
                ct) :
            null;

        var hasUndoAccess =
            absence != null &&
            absence.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
            absence.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

        if (!hasRemoveAccess && !hasUndoAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAbsenceCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                IsExternal = false,

                AbsenceId = absenceId,
            },
            ct);

        return new OkResult();
    }

    [Authorize(Policy = Policies.ClassBookAdminAccess)]
    [HttpPost]
    public async Task RemoveAbsencesAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] RemoveAbsencesCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
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
