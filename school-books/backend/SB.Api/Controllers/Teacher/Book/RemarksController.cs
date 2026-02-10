namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;
using static SB.Domain.IRemarksQueryRepository;

public class RemarksController : BookController
{
    [HttpGet]
    public async Task<ActionResult<GetAllForClassBookVO[]>> GetAllForClassBookAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromServices] IRemarksQueryRepository remarksQueryRepository,
        CancellationToken ct)
        => await remarksQueryRepository.GetAllForClassBookAsync(schoolYear, classBookId, ct);

    [HttpGet]
    public async Task<ActionResult<GetAllForStudentAndTypeVO[]>> GetAllForStudentAndTypeAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromQuery] int personId,
        [FromQuery] RemarkType type,
        [FromServices] IRemarksQueryRepository remarksQueryRepository,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        OidcToken token = httpContextAccessor.GetToken();
        bool hasClassBookAdminWriteAccess =
            await authService.HasClassBookAdminAccessAsync(
                token,
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                ct);

        var remarks =
            await remarksQueryRepository.GetAllForStudentAndTypeAsync(
                schoolYear,
                classBookId,
                personId,
                type,
                ct);

        foreach (var remark in remarks)
        {
            remark.HasEditAccess = remark.HasRemoveAccess =
                hasClassBookAdminWriteAccess
                || remark.WriteAccessCurriculumTeacherPersonIds.Any(pId => pId == token.SelectedRole.PersonId)
                || remark.ReplTeacherPersonIds.Any(pId => pId == token.SelectedRole.PersonId);
        }

        return remarks;
    }

    [HttpGet("{remarkId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute] int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int remarkId,
        [FromServices] IRemarksQueryRepository remarksQueryRepository,
        CancellationToken ct)
        => await remarksQueryRepository.GetAsync(schoolYear, classBookId, remarkId, ct);

    [HttpPost("{curriculumId:int}")]
    public async Task<ActionResult> CreateRemarkAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromBody] CreateRemarkCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
            httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct
            ) && !await authService.HasReplCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                command.Date!.Value,
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

                CurriculumId = curriculumId
            },
            ct);

        return new OkResult();
    }

    [Authorize(Policy = Policies.CurriculumAccess)]
    [HttpPost("{curriculumId:int}/{remarkId:int}")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromRoute] int remarkId,
        [FromBody] UpdateRemarkCommand command,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        if (!await authService.HasCurriculumAccessAsync(
            httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct
            ) && !await authService.HasReplCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                command.Date!.Value,
                ct))
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

                IsExternal = false,
                CurriculumId = curriculumId,
                RemarkId = remarkId,
            },
            ct);

        return new OkResult();
    }

    [HttpPost("{curriculumId:int}/{remarkId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute] int schoolYear,
        [FromRoute] int instId,
        [FromRoute] int classBookId,
        [FromRoute] int curriculumId,
        [FromRoute] int remarkId,
        [FromServices] IRemarksQueryRepository remarksQueryRepository,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        [FromServices] IAuthService authService,
        CancellationToken ct)
    {
        var hasCurriculumAccess =
            await authService.HasCurriculumAccessAsync(
            httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                ct);

        var remark = !hasCurriculumAccess ?
            await remarksQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                remarkId,
                ct) :
            null;

        var hasReplCurriculumAccess =
            remark != null &&
            !await authService.HasReplCurriculumAccessAsync(
                httpContextAccessor.GetToken(),
                AccessType.Write,
                schoolYear,
                instId,
                classBookId,
                curriculumId,
                remark.Date,
                ct);

        if (!hasCurriculumAccess && !hasReplCurriculumAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveRemarkCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                IsExternal = false,
                CurriculumId = curriculumId,
                RemarkId = remarkId,
            },
            ct);

        return new OkResult();
    }
}
