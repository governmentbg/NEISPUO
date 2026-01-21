namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveAdditionalActivityCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<AdditionalActivity> AdditionalActivityAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveAdditionalActivityCommand>
{
    public async Task Handle(RemoveAdditionalActivityCommand command, CancellationToken ct)
    {
        var additionalActivity = await this.AdditionalActivityAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.AdditionalActivityId!.Value,
            ct);

        if (command.ClassBookId!.Value != additionalActivity.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsAdditionalActivityModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            additionalActivity.Year,
            additionalActivity.WeekNumber,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        this.AdditionalActivityAggregateRepository.Remove(additionalActivity);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
