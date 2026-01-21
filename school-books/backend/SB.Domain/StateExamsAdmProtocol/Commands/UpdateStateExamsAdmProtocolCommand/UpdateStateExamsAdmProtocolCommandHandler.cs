namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateStateExamsAdmProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamsAdmProtocol> StateExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<UpdateStateExamsAdmProtocolCommand, int>
{
    public async Task<int> Handle(UpdateStateExamsAdmProtocolCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.StateExamsAdmProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamsAdmProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.UpdateData(
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

        await this.UnitOfWork.SaveAsync(ct);

        return stateExamsAdmProtocol.StateExamsAdmProtocolId;
    }
}
