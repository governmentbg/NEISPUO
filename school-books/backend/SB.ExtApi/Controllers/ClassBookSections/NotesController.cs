namespace SB.ExtApi;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SB.Domain;

public class NotesController : ClassBookSectionController
{
    /// <summary>{{Notes.GetAll.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Notes.GetAll.Returns}}</returns>
    [HttpGet]
    public async Task<NoteDO[]> GetAllAsync(
        [FromRoute]int schoolYear,
        [FromRoute][SuppressMessage("", "IDE0060")]int institutionId,
        [FromRoute]int classBookId,
        [FromServices]IExtApiQueryRepository extApiQueryRepository,
        CancellationToken ct)
        => await extApiQueryRepository.NotesGetAllAsync(schoolYear, classBookId, ct);

    /// <summary>{{Notes.Create.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <returns>{{Common.CreationReturns}}</returns>
    [HttpPost]
    public async Task<int> CreateNoteAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromBody]NoteDO note,
        [FromServices]IMediator mediator,
        [FromServices]IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new CreateNoteCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),

                IsForAllStudents = note.IsForAllStudents,
                StudentIds = note.IsForAllStudents
                    ? Array.Empty<int>()
                    #pragma warning disable CS0618 // member is obsolete
                    : note.StudentIds ?? note.Students?.Select(s => s.PersonId).ToArray(),
                    #pragma warning restore CS0618 // member is obsolete
                Description = note.Description,
            },
            ct);

    /// <summary>{{Notes.Update.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="noteId">{{Common.NoteId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpPut("{noteId:int}")]
    public async Task UpdateNoteAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int noteId,
        [FromBody]NoteDO note,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new UpdateNoteCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NoteId = noteId,

                IsForAllStudents = note.IsForAllStudents,
                StudentIds = note.IsForAllStudents
                    ? Array.Empty<int>()
                    #pragma warning disable CS0618 // member is obsolete
                    : note.StudentIds ?? note.Students?.Select(s => s.PersonId).ToArray(),
                    #pragma warning restore CS0618 // member is obsolete
                Description = note.Description,
            },
            ct);

    /// <summary>{{Notes.Delete.Summary}}</summary>
    /// <include file='../../Documentation/Documentation.xml' path='Documentation/CommonClassBookSectionParams/*'/>
    /// <param name="noteId">{{Common.NoteId}}</param>
    /// <returns>{{Common.NoResponse}}</returns>
    [HttpDelete("{noteId:int}")]
    public async Task RemoveNoteAsync(
        [FromRoute]int schoolYear,
        [FromRoute]int institutionId,
        [FromRoute]int classBookId,
        [FromRoute]int noteId,
        [FromServices] IMediator mediator,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken ct)
        => await mediator.Send(
            new RemoveNoteCommand
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassBookId = classBookId,
                SysUserId = httpContextAccessor.GetSysUserId(),
                NoteId = noteId,
            },
            ct);
}
