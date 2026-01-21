namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateExamCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Exam> ExamAggregateRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateExamCommand>
{
    public async Task Handle(UpdateExamCommand command, CancellationToken ct)
    {
        var exam = await this.ExamAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamId!.Value,
            ct);

        if (command.ClassBookId!.Value != exam.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.CurriculumId == null ||
            command.CurriculumId.Value != exam.CurriculumId)
        {
            // the curriculum check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.CurriculumId)}.");
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

        exam.UpdateData(
            command.Date!.Value,
            command.Description,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
