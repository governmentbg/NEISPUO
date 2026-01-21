namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.IAdditionalActivitiesQueryRepository;
using static SB.Domain.ITopicsQueryRepository;

public class TopicsController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllForWeekVO[]>> GetAllForWeekAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int year,
        [FromQuery]int weekNumber,
        [FromServices]ITopicsQueryRepository topicsQueryRepository,
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

        var topicsForWeek = await topicsQueryRepository.GetAllForWeekAsync(schoolYear, classBookId, year, weekNumber, ct);

        foreach (var topic in topicsForWeek)
        {
            topic.HasUndoAccess = topic.CreatedBySysUserId == sysUserId;
            topic.HasRemoveAccess = hasClassBookAdminWriteAccess;
        }

        return topicsForWeek;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] CreateTopicsCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        foreach (int scheduleLessonId in command.Topics?.Select(a => a.ScheduleLessonId).WhereNotNull() ?? Array.Empty<int>())
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

        _ = await mediator.Send(
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

    [HttpPost]
    public async Task<ActionResult> RemoveTopicsAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] RemoveTopicsCommand command,
        [FromServices] ITopicsQueryRepository topicsQueryRepository,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int sysUserId = token.SelectedRole.SysUserId;
        var hasRemoveAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var topics = !hasRemoveAccess ?
            await topicsQueryRepository.GetUndoInfoByIdsAsync(
                schoolYear,
                command.Topics?.Select(t => t.TopicId).WhereNotNull().ToArray() ?? Array.Empty<int>(),
                ct) :
            null;

        var hasUndoAccess =
            topics != null &&
            topics.All(topic =>
                topic.CreatedBySysUserId == sysUserId &&
                topic.CreateDate.AddMinutes(ClassBook.UndoIntervalInMinutes) >= DateTime.Now);

        if (!hasRemoveAccess && !hasUndoAccess)
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

    [HttpGet]
    public async Task<GetAllAdditionalActivitiesForWeekVO[]> GetAllAdditionalActivitiesForWeekAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int year,
        [FromQuery]int weekNumber,
        [FromServices]IAdditionalActivitiesQueryRepository additionalActivitiesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var userId = httpContextAccessor.GetSysUserId();

        var additionalActivities =
            await additionalActivitiesQueryRepository.GetAllAdditionalActivitiesForWeekAsync(
                schoolYear,
                classBookId,
                year,
                weekNumber,
                ct);

        foreach (var additionalActivity in additionalActivities)
        {
            additionalActivity.HasCreatorAccess = additionalActivity.CreatedBySysUserId == userId;
        }

        return additionalActivities;
    }

    [HttpGet("{additionalActivityId:int}")]
    public async Task<GetVO> GetAdditionalActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int additionalActivityId,
        [FromServices]IAdditionalActivitiesQueryRepository additionalActivitiesQueryRepository,
        CancellationToken ct)
        => await additionalActivitiesQueryRepository.GetAsync(schoolYear, classBookId, additionalActivityId, ct);

    [HttpPost("")]
    public async Task CreateAdditionalActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateAdditionalActivityCommand command,
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

    [HttpPost("{additionalActivityId:int}")]
    public async Task<ActionResult> UpdateAdditionalActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int additionalActivityId,
        [FromBody]UpdateAdditionalActivityCommand command,
        [FromServices]IAdditionalActivitiesQueryRepository additionalActivitiesQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var additionalActivity = !hasAdminWriteAccess ?
            await additionalActivitiesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                additionalActivityId,
                ct) :
            null;

        var hasCreatorAccess =
            additionalActivity != null &&
            additionalActivity.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        if (!hasAdminWriteAccess && !hasCreatorAccess)
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
                AdditionalActivityId = additionalActivityId
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{additionalActivityId:int}")]
    public async Task<ActionResult> RemoveAdditionalActivityAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int additionalActivityId,
        [FromServices]IAdditionalActivitiesQueryRepository additionalActivitiesQueryRepository,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        [FromServices]IAuthService authService,
        CancellationToken ct)
    {
        var hasAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var additionalActivity = !hasAdminWriteAccess ?
            await additionalActivitiesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                additionalActivityId,
                ct) :
            null;

        var hasCreatorAccess =
            additionalActivity != null &&
            additionalActivity.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        if (!hasAdminWriteAccess && !hasCreatorAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveAdditionalActivityCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                AdditionalActivityId = additionalActivityId
            },
            ct);

        return new OkResult();
    }

    [HttpGet]
    public async Task<ActionResult<int[]>> GetCurriculumsWithTopicPlanAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromServices]ITopicsQueryRepository topicsQueryRepository,
        CancellationToken ct)
        => await topicsQueryRepository.GetCurriculumsWithTopicPlanAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetCurriculumTopiPlanVO[]>> GetCurriculumTopiPlanAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int curriculumId,
        [FromServices]ITopicsQueryRepository topicsQueryRepository,
        CancellationToken ct)
        => await topicsQueryRepository.GetCurriculumTopiPlanAsync(schoolYear, classBookId, curriculumId, ct);
}
