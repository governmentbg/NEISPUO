namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateStateExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamsAdmProtocol> StateExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<CreateStateExamsAdmProtocolCommand, int>
{
    public async Task<int> Handle(CreateStateExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = new StateExamsAdmProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNum,
            command.ProtocolDate,
            command.CommissionMeetingDate!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.ExamSession,
            command.DirectorPersonId!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);

        await this.StateExamsAdmProtocolCommandAggregateRepository.AddAsync(stateExamsAdmProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return stateExamsAdmProtocol.StateExamsAdmProtocolId;
    }
}
