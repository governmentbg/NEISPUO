namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateUpdateSkillsCheckExamResultProtocolEvaluatorCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamResultProtocol> SkillsCheckExamResultProtocolCommandAggregateRepository)
    : IRequestHandler<CreateSkillsCheckExamResultProtocolEvaluatorCommand>, IRequestHandler<UpdateSkillsCheckExamResultProtocolEvaluatorCommand>
{
    public async Task Handle(CreateSkillsCheckExamResultProtocolEvaluatorCommand command, CancellationToken ct)
    {
        var protocol = await this.SkillsCheckExamResultProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamResultProtocolId!.Value,
            ct);

        protocol.AddEvaluator(
            command.Name!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }

    public async Task Handle(UpdateSkillsCheckExamResultProtocolEvaluatorCommand command, CancellationToken ct)
    {
        var protocol = await this.SkillsCheckExamResultProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamResultProtocolId!.Value,
            ct);

        protocol.UpdateEvaluator(
            command.SkillsCheckExamResultProtocolEvaluatorId!.Value,
            command.Name!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
