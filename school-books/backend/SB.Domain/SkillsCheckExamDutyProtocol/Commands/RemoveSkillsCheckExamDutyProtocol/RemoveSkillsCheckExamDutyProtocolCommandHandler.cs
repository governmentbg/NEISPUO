namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveSkillsCheckExamDutyProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamDutyProtocol> SkillsCheckExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveSkillsCheckExamDutyProtocolCommand>
{
    public async Task Handle(RemoveSkillsCheckExamDutyProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.SkillsCheckExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamDutyProtocolId!.Value,
            ct);
        this.SkillsCheckExamDutyProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
