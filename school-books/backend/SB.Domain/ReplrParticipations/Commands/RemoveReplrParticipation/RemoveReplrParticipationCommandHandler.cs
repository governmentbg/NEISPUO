namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveReplrParticipationCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ReplrParticipation> ReplrParticipationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<RemoveReplrParticipationCommand>
{
    public async Task Handle(RemoveReplrParticipationCommand command, CancellationToken ct)
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

        var replrParticipation = await this.ReplrParticipationAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ReplrParticipationId!.Value,
            ct);

        if (command.ClassBookId!.Value != replrParticipation.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        this.ReplrParticipationAggregateRepository.Remove(replrParticipation);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
