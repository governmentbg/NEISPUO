namespace SB.Api;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.ITopicsDplrQueryRepository;

public class TopicsDplrController : BookController
{
    [HttpGet]
    public async Task<ActionResult<int[]>> GetExistingHourNumbersForDateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] [SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] DateTime date,
        [FromServices] ITopicsDplrQueryRepository topicsQueryRepository,
        CancellationToken ct)
        => await topicsQueryRepository.GetExistingHourNumbersForDateAsync(schoolYear, classBookId, date, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllDplrForWeekVO[]>> GetAllForWeekAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int year,
        [FromQuery] int weekNumber,
        [FromServices] ITopicsDplrQueryRepository topicsDplrQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        int sysUserId = token.SelectedRole.SysUserId;
        int? selectedRolePersonId = token.SelectedRole.PersonId;
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var topicsForWeek = await topicsDplrQueryRepository.GetAllForWeekAsync(schoolYear, classBookId, year, weekNumber, ct);

        foreach (var topic in topicsForWeek)
        {
            topic.HasUndoAccess = topic.CreatedBySysUserId == sysUserId;
            topic.HasTopicRemoveAccess = hasClassBookAdminWriteAccess;
            topic.HasTopicCreateAccess = hasClassBookAdminWriteAccess
                                         || topicsForWeek
                                             .All(tfw =>
                                                 tfw.Teachers.Any(ctpi => ctpi.TeacherPersonId == selectedRolePersonId));
        }

        return topicsForWeek;
    }

    [HttpPost]
    public async Task<ActionResult> CreateTopicDplrAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] CreateTopicDplrCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        if (!await authService.HasCurriculumAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                command.CurriculumId!.Value,
                ct))
        {
            return new ForbidResult();
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
    public async Task<ActionResult> RemoveTopicDplrAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromBody] RemoveTopicDplrCommand command,
        [FromServices] ITopicsDplrQueryRepository topicsQueryRepository,
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
            await topicsQueryRepository.GetTopicsDplrUndoInfoByIdAsync(
                schoolYear,
                command.TopicDplrId!.Value,
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
}
