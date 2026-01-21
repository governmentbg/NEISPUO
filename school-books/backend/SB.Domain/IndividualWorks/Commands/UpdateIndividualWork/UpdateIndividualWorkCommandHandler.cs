namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateIndividualWorkCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<IndividualWork> IndividualWorkAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateIndividualWorkCommand>
{
    public async Task Handle(UpdateIndividualWorkCommand command, CancellationToken ct)
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

        var individualWork = await this.IndividualWorkAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.IndividualWorkId!.Value,
            ct);

        if (command.ClassBookId!.Value != individualWork.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        individualWork.UpdateData(
            command.Date!.Value,
            command.IndividualWorkActivity!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
