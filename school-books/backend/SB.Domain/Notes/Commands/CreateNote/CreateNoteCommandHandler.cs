namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateNoteCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Note> NoteAggregateRepository,
    INotesQueryRepository NotesQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateNoteCommand, int>
{
    public async Task<int> Handle(CreateNoteCommand command, CancellationToken ct)
    {
        int schoolYear = command.SchoolYear!.Value;
        int classBookId = command.ClassBookId!.Value;

        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{schoolYear}, classBookId:{classBookId}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            schoolYear,
            classBookId,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        foreach (var studentPersonId in command.StudentIds!)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                    schoolYear,
                    classBookId,
                    studentPersonId,
                    ct))
            {
                throw new DomainValidationException($"This person ({studentPersonId}) is not in the class book students list");
            }
        }

        var note = new Note(
            schoolYear,
            classBookId,
            command.StudentIds!,
            command.Description!,
            command.IsForAllStudents!.Value,
            command.SysUserId!.Value);

        await this.NoteAggregateRepository.AddAsync(note, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return note.NoteId;
    }
}
