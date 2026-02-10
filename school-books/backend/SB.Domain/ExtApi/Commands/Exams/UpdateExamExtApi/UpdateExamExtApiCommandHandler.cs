namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateExamExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Exam> ExamAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateExamExtApiCommand>
{
    public async Task Handle(UpdateExamExtApiCommand command, CancellationToken ct)
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

        var exam = await this.ExamAggregateRepository.FindAsync(
            schoolYear,
            command.ExamId!.Value,
            ct);

        if (command.ClassBookId!.Value != exam.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (command.Type != exam.Type)
        {
            throw new DomainValidationException($"Cannot update the exam type! It is {command.Type} and should be {exam.Type}");
        }

        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                schoolYear,
                classBookId,
                command.CurriculumId!.Value,
                ct))
        {
            throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
        }

        exam.ExtUpdateData(
            command.CurriculumId!.Value,
            command.Date!.Value,
            command.Description,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
