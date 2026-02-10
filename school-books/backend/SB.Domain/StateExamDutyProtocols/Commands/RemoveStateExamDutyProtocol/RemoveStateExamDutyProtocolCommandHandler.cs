namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveStateExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamDutyProtocol> StateExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveStateExamDutyProtocolCommand>
{
    public async Task Handle(RemoveStateExamDutyProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.StateExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamDutyProtocolId!.Value,
            ct);
        this.StateExamDutyProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
