namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateSkillsCheckExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamResultProtocol> SkillsCheckExamResultProtocolAggregateRepository)
    : IRequestHandler<CreateSkillsCheckExamResultProtocolCommand, int>
{
    public async Task<int> Handle(CreateSkillsCheckExamResultProtocolCommand command, CancellationToken ct)
    {
        var examDutyProtocol = new SkillsCheckExamResultProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolNumber,
            command.SubjectId!.Value,
            command.Date,
            command.StudentsCapacity!.Value!,
            command.SysUserId!.Value);

        await this.SkillsCheckExamResultProtocolAggregateRepository.AddAsync(examDutyProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return examDutyProtocol.SkillsCheckExamResultProtocolId;
    }
}
