namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateNoteCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Note> NoteAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateNoteCommand>
{
    public async Task Handle(UpdateNoteCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var note = await this.NoteAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NoteId!.Value,
            ct);

        if (command.ClassBookId!.Value != note.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        var noteStudents = note.Students.Select(s => s.PersonId).ToArray();

        // the ones in the Note.Students have already been checked so we'll skip them
        foreach (var studentPersonId in command.StudentIds!.Except(noteStudents))
        {
            if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    studentPersonId,
                    ct))
            {
                throw new DomainValidationException($"This person ({studentPersonId}) is not in the class book students list");
            }
        }

        note.UpdateData(
            command.StudentIds!,
            command.Description!,
            command.IsForAllStudents!.Value,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
