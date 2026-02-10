namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateNvoExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateNvoExamDutyProtocolCommand, int>
{
    public async Task<int> Handle(CreateNvoExamDutyProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new NvoExamDutyProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
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

        await this.NvoExamDutyProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.NvoExamDutyProtocolId;
    }
}
