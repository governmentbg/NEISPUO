namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveNvoExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveNvoExamDutyProtocolCommand>
{
    public async Task Handle(RemoveNvoExamDutyProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.NvoExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NvoExamDutyProtocolId!.Value,
            ct);
        this.NvoExamDutyProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
