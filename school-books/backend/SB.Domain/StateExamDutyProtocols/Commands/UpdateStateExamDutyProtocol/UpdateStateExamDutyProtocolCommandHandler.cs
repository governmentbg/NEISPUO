namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateStateExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamDutyProtocol> StateExamDutyProtocolAggregateRepository)
    : IRequestHandler<UpdateStateExamDutyProtocolCommand>
{
    public async Task Handle(UpdateStateExamDutyProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.StateExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.StateExamDutyProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.EduFormId,
            command.Date!.Value,
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.ModulesCount!.Value,
            command.RoomNumber,
            command.SupervisorPersonIds!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
