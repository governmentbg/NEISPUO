namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveSkillsCheckExamResultProtocolEvaluatorCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamResultProtocol> SkillsCheckExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveSkillsCheckExamResultProtocolEvaluatorCommand>
{
    public async Task Handle(RemoveSkillsCheckExamResultProtocolEvaluatorCommand command, CancellationToken ct)
    {
        var protocol = await this.SkillsCheckExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamResultProtocolId!.Value,
            ct);

        protocol.RemoveEvaluator(command.SkillsCheckExamResultProtocolEvaluatorId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
