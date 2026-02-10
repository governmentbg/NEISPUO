namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateFirstGradeResultExtApiCommandHandler(
    IUnitOfWork UnitOfWork,
    IFirstGradeResultsAggregateRepository FirstGradeResultAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateFirstGradeResultExtApiCommand, int>
{
    public async Task<int> Handle(CreateFirstGradeResultExtApiCommand command, CancellationToken ct)
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

        if (!await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var firstGradeResult = new FirstGradeResult(
            command.SchoolYear!.Value,
            command.PersonId!.Value,
            command.ClassBookId!.Value,
            command.QualitativeGrade,
            command.SpecialGrade,
            command.SysUserId!.Value);

        await this.FirstGradeResultAggregateRepository.AddAsync(firstGradeResult, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return firstGradeResult.FirstGradeResultId;
    }
}
