namespace SB.Api;

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Common;
using SB.Domain;
using static SB.Domain.INotesQueryRepository;

public class NotesController : BookController
{
    [HttpGet]
    public async Task<ActionResult<TableResultVO<GetAllVO>>> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromQuery]int? offset,
        [FromQuery]int? limit,
        [FromServices]INotesQueryRepository notesQueryRepository,
        CancellationToken ct)
        => await notesQueryRepository.GetAllAsync(schoolYear, classBookId, offset, limit, ct);

    [HttpPost]
    public async Task<int> CreateNoteAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromBody]CreateNoteCommand command,
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

    [HttpGet("{noteId:int}")]
    public async Task<ActionResult<GetVO>> GetAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int noteId,
        [FromServices]INotesQueryRepository notesQueryRepository,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
    {
        var note = await notesQueryRepository.GetAsync(schoolYear, classBookId, noteId, ct);

        note.HasCreatorAccess = note.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        return note;
    }

    [HttpPost("{noteId:int}")]
    public async Task<ActionResult> UpdateAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int noteId,
        [FromBody]UpdateNoteCommand command,
        [FromServices]INotesQueryRepository notesQueryRepository,
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

        var note = !hasAdminWriteAccess ?
            await notesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                noteId,
                ct) :
            null;

        var hasCreatorAccess =
            note != null &&
            note.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

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
                NoteId = noteId,
            },
            ct);

        return new OkResult();
    }

    [HttpDelete("{noteId:int}")]
    public async Task<ActionResult> RemoveAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int instId,
        [FromRoute]int classBookId,
        [FromRoute]int noteId,
        [FromServices]INotesQueryRepository notesQueryRepository,
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

        var note = !hasAdminWriteAccess ?
            await notesQueryRepository.GetAsync(
                schoolYear,
                classBookId,
                noteId,
                ct) :
            null;

        var hasCreatorAccess =
            note != null &&
            note.CreatedBySysUserId == httpContextAccessor.GetSysUserId();

        if (!hasAdminWriteAccess && !hasCreatorAccess)
        {
            return new ForbidResult();
        }

        await mediator.Send(
            new RemoveNoteCommand
            {
                SchoolYear = schoolYear,
                InstId = instId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NoteId = noteId,
            },
            ct);

        return new OkResult();
    }
}
