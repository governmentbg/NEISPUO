namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IMyHourQueryRepository;


[Authorize(Policy = Policies.InstitutionAccess)]
[ApiController]
[Route("api/[controller]/{schoolYear:int}/{instId:int}/[action]")]
public class MyHourController
{
    [HttpGet("{date:datetime}")]
    public async Task<ActionResult<GetTeacherLessonsVO[]>> GetTeacherLessonsAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]DateTime date,
        [FromServices]IMyHourQueryRepository myHourQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var personId = httpContextAccessor.GetPersonId();

        if (personId == null)
        {
            return new ForbidResult();
        }

        return await myHourQueryRepository.GetTeacherLessonsAsync(
            schoolYear,
            instId,
            date,
            personId.Value,
            ct);
    }

    [Authorize(Policy = Policies.ScheduleLessonAccess)]
    [HttpGet("{classBookId:int}/{scheduleLessonId:int}")]
    public async Task<ActionResult<GetTeacherLessonVO>> GetTeacherLessonAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleLessonId,
        [FromServices]IMyHourQueryRepository myHourQueryRepository,
        [FromServices]IAuthService authService,
        [FromServices]IHttpContextAccessor httpContextAccessor,
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

        var teacherLesson = await myHourQueryRepository.GetTeacherLessonAsync(
            schoolYear,
            classBookId,
            scheduleLessonId,
            ct);

        teacherLesson.TopicHasUndoAccess = teacherLesson.TopicCreatedBySysUserId == sysUserId;
        teacherLesson.TopicHasRemoveAccess = hasClassBookAdminWriteAccess;

        return teacherLesson;
    }

    [Authorize(Policy = Policies.ScheduleLessonAccess)]
    [HttpGet("{classBookId:int}/{scheduleLessonId:int}")]
    public async Task<ActionResult<GetTeacherLessonAbsencesVO[]>> GetTeacherLessonAbsencesAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleLessonId,
        [FromServices]IMyHourQueryRepository myHourQueryRepository,
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

        var absences = await myHourQueryRepository.GetTeacherLessonAbsencesAsync(
            schoolYear,
            classBookId,
            scheduleLessonId,
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

    [Authorize(Policy = Policies.ScheduleLessonAccess)]
    [HttpGet("{classBookId:int}/{scheduleLessonId:int}")]
    public async Task<ActionResult<GetTeacherLessonGradesVO[]>> GetTeacherLessonGradesAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int scheduleLessonId,
        [FromServices]IMyHourQueryRepository myHourQueryRepository,
        CancellationToken ct)
        => await myHourQueryRepository.GetTeacherLessonGradesAsync(
            schoolYear,
            classBookId,
            scheduleLessonId,
            ct);
}
