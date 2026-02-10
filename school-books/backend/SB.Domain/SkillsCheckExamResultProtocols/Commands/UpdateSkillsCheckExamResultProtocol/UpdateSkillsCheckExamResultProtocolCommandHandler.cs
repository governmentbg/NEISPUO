namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateSkillsCheckExamResultProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SkillsCheckExamResultProtocol> SkillsCheckExamResultProtocolAggregateRepository)
    : IRequestHandler<UpdateSkillsCheckExamResultProtocolCommand>
{
    public async Task Handle(UpdateSkillsCheckExamResultProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.SkillsCheckExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SkillsCheckExamResultProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.SubjectId!.Value,
            command.Date,
            command.StudentsCapacity!.Value!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
