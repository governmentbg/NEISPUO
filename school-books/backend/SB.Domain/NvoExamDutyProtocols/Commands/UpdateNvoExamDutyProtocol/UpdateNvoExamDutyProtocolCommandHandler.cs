namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateNvoExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository)
    : IRequestHandler<UpdateNvoExamDutyProtocolCommand>
{
    public async Task Handle(UpdateNvoExamDutyProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.NvoExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NvoExamDutyProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.BasicClassId!.Value,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.Date!.Value,
            command.RoomNumber,
            command.DirectorPersonId!.Value,
            command.SupervisorPersonIds!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
