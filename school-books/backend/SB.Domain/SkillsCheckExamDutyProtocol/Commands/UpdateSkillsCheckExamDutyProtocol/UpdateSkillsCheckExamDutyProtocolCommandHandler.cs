namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateSkillsCheckExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamDutyProtocol> SkillsCheckExamDutyProtocolAggregateRepository)
    : IRequestHandler<UpdateSkillsCheckExamDutyProtocolCommand>
{
    public async Task Handle(UpdateSkillsCheckExamDutyProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.SkillsCheckExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamDutyProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.Date!.Value,
            command.DirectorPersonId!.Value,
            command.StudentsCapacity!.Value!,
            command.SupervisorPersonIds!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
