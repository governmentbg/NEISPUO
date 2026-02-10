namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveStateExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamsAdmProtocol> StateExamsAdmProtocolAggregateRepository)
    : IRequestHandler<RemoveStateExamsAdmProtocolCommand>
{
    public async Task Handle(RemoveStateExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.StateExamsAdmProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamsAdmProtocolId!.Value,
            ct);

        this.StateExamsAdmProtocolAggregateRepository.Remove(stateExamsAdmProtocol);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
