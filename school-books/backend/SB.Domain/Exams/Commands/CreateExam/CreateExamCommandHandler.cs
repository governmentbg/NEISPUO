namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateExamCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Exam> ExamAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateExamCommand, int>
{
    public async Task<int> Handle(CreateExamCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.CurriculumId!.Value,
            ct))
        {
            throw new DomainValidationException($"This curriculum is not in the class book curriculum list");
        }

        var exam = new Exam(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.Type!.Value,
            command.CurriculumId!.Value,
            command.Date!.Value,
            command.Description,
            command.SysUserId!.Value);

        await this.ExamAggregateRepository.AddAsync(exam, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return exam.ExamId;
    }
}
