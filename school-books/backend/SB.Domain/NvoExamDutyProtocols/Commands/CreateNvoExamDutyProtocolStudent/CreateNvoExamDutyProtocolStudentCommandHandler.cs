namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateNvoExamDutyProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateNvoExamDutyProtocolStudentCommand, int>
{
    public async Task<int> Handle(CreateNvoExamDutyProtocolStudentCommand command, CancellationToken ct)
    {
        var protocol = await this.NvoExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NvoExamDutyProtocolId!.Value,
            ct);

        foreach (var student in command.Students!)
        {
            protocol.AddStudent(
                student.ClassId!.Value,
                student.PersonId!.Value,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);

        return protocol.NvoExamDutyProtocolId;
    }
}
