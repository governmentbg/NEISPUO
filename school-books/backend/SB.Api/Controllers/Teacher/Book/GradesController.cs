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
using static SB.Domain.IGradesQueryRepository;

public class GradesController : BookController
{
    [HttpGet]
    public async Task<GetCurriculumStudentsVO[]> GetCurriculumStudentsAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int curriculumId,
        [FromServices]IGradesQueryRepository gradesQueryRepository,
        CancellationToken ct)
        => await gradesQueryRepository.GetCurriculumStudentsAsync(schoolYear, classBookId, curriculumId, ct);

    [HttpGet]
    public async Task<GetCurriculumGradesVO[]> GetCurriculumGradesAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int curriculumId,
        [FromServices] IGradesQueryRepository gradesQueryRepository,
        CancellationToken ct)
        => await gradesQueryRepository.GetCurriculumGradesAsync(schoolYear, classBookId, curriculumId, ct);

    [HttpGet]
    public async Task<GetProfilingSubjectForecastGradesVO[]> GetProfilingSubjectForecastGradesAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int curriculumId,
        [FromServices]IGradesQueryRepository gradesQueryRepository,
        CancellationToken ct)
        => await gradesQueryRepository.GetProfilingSubjectForecastGradesAsync(schoolYear, classBookId, curriculumId, ct);

    [HttpGet]
    public async Task<GetCurriculumVO> GetCurriculumAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int curriculumId,
        [FromServices]IGradesQueryRepository gradesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        bool hasCurriculumAccess =
            await authService.HasCurriculumAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct);

        var curriculum = await gradesQueryRepository.GetCurriculumAsync(schoolYear, classBookId, curriculumId, ct);

        curriculum.HasCreateGradeWithoutScheduleLessonAccess = hasCurriculumAccess;

        return curriculum;
    }

    [HttpGet("{gradeId:int}")]
    public async Task<GetVO> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int gradeId,
        [FromServices] IGradesQueryRepository gradesQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
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

        var grade = await gradesQueryRepository.GetAsync(schoolYear, classBookId, gradeId, ct);

        grade.HasUndoAccess = grade.CreatedBySysUserId == sysUserId;
        grade.HasRemoveAccess = hasClassBookAdminWriteAccess;

        return grade;
    }

    [HttpPost("{curriculumId:int}")]
    public async Task<ActionResult> CreateGradeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int curriculumId,
        [FromBody]CreateGradeCommand command,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        if (command.Type == null)
        {
            throw new DomainValidationException("Type is required");
        }
        else if (!Grade.GradeTypeRequiresScheduleLesson(command.Type.Value))
        {
            if (!await authService.HasCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct))
            {
                return new ForbidResult();
            }
        }
        else if (command.ScheduleLessonId == null)
        {
            throw new DomainValidationException($"Schedule lesson should not be null when the type is {command.Type}");
        }
        else
        {
            if (!await authService.HasScheduleLessonAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                command.ScheduleLessonId.Value,
                ct))
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
                SysUserId = httpContextAccessor.GetSysUserId(),

                CurriculumId = curriculumId,
            },
            ct);

        return new OkResult();
    }

    [Authorize(Policy = Policies.CurriculumAccess)]
    [HttpPost("{curriculumId:int}")]
    public async Task CreateForecastGradeAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int curriculumId,
        [FromBody]CreateForecastGradeCommand command,
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

                CurriculumId = curriculumId,
            },
            ct);

    [HttpDelete("{gradeId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int gradeId,
        [FromServices]IGradesQueryRepository gradesQueryRepository,
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

        var grade = !hasRemoveAccess ?
            await gradesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                gradeId,
                ct) :
            null;

        var hasUndoAccess =
            grade != null &&
            grade.CreatedBySysUserId == httpContextAccessor.GetSysUserId() &&
            grade.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now;

        if (!hasRemoveAccess && !hasUndoAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveGradeCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                GradeId = gradeId,
            },
            ct);

        return new OkResult();
    }
}
