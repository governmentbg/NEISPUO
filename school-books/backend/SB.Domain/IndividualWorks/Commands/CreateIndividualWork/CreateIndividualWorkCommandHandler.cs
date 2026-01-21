namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateIndividualWorkCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<IndividualWork> IndividualWorkAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateIndividualWorkCommand, int>
{
    public async Task<int> Handle(CreateIndividualWorkCommand command, CancellationToken ct)
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

        var individualWork = new IndividualWork(
            schoolYear,
            classBookId,
            command.PersonId!.Value,
            command.Date!.Value,
            command.IndividualWorkActivity!,
            command.SysUserId!.Value);

        await this.IndividualWorkAggregateRepository.AddAsync(individualWork, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return individualWork.IndividualWorkId;
    }
}
