namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateSkillsCheckExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamDutyProtocol> SkillsCheckExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateSkillsCheckExamDutyProtocolCommand, int>
{
    public async Task<int> Handle(CreateSkillsCheckExamDutyProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new SkillsCheckExamDutyProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNumber,
            command.ProtocolDate,
            command.SubjectId!.Value,
            command.SubjectTypeId!.Value,
            command.Date!.Value,
            command.DirectorPersonId!.Value,
            command.StudentsCapacity!.Value!,
            command.SupervisorPersonIds!,
            command.SysUserId!.Value);

        await this.SkillsCheckExamDutyProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.SkillsCheckExamDutyProtocolId;
    }
}
