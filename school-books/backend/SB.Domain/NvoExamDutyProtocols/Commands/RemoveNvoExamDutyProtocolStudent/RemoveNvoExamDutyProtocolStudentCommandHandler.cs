namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveNvoExamDutyProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository)
    : IRequestHandler<RemoveNvoExamDutyProtocolStudentCommand>
{
    public async Task Handle(RemoveNvoExamDutyProtocolStudentCommand command, CancellationToken ct)
    {
        var support = await this.NvoExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NvoExamDutyProtocolId!.Value,
            ct);

        support.RemoveStudent(command.ClassId!.Value, command.PersonId!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
