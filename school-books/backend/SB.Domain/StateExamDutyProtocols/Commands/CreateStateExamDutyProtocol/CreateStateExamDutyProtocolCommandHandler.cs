namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateStateExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<StateExamDutyProtocol> StateExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateStateExamDutyProtocolCommand, int>
{
    public async Task<int> Handle(CreateStateExamDutyProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new StateExamDutyProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SessionType,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.EduFormId,
            command.Date!.Value,
            command.OrderNumber!,
            command.OrderDate!.Value,
            command.ModulesCount!.Value!,
            command.RoomNumber,
            command.SupervisorPersonIds!,
            command.SysUserId!.Value);

        await this.StateExamDutyProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.StateExamDutyProtocolId;
    }
}
