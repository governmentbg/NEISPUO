namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveSkillsCheckExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamResultProtocol> SkillsCheckExamResultProtocolAggregateRepository)
    : IRequestHandler<RemoveSkillsCheckExamResultProtocolCommand>
{
    public async Task Handle(RemoveSkillsCheckExamResultProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.SkillsCheckExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamResultProtocolId!.Value,
            ct);
        this.SkillsCheckExamResultProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
