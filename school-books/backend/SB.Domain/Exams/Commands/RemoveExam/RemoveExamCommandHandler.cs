namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveExamCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Exam> ExamAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveExamCommand>
{
    public async Task Handle(RemoveExamCommand command, CancellationToken ct)
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

        var exam = await this.ExamAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamId!.Value,
            ct);

        if (command.ClassBookId!.Value != exam.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.IsExternal == false &&
            (command.CurriculumId == null ||
            command.CurriculumId.Value != exam.CurriculumId))
        {
            // the curriculum check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.CurriculumId)}.");
        }

        this.ExamAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
