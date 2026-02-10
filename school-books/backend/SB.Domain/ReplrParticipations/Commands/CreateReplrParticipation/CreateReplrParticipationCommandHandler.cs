namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateReplrParticipationCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ReplrParticipation> ReplrParticipationAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateReplrParticipationCommand, int>
{
    public async Task<int> Handle(CreateReplrParticipationCommand command, CancellationToken ct)
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

        var replrParticipation = new ReplrParticipation(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.ReplrParticipationTypeId!.Value,
            command.Date!.Value,
            command.Topic,
            command.InstitutionId,
            command.Attendees!,
            command.SysUserId!.Value);

        await this.ReplrParticipationAggregateRepository.AddAsync(replrParticipation, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return replrParticipation.ReplrParticipationId;
    }
}
