namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateExamDutyProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository)
    : IRequestHandler<CreateExamDutyProtocolStudentCommand, int>
{
    public async Task<int> Handle(CreateExamDutyProtocolStudentCommand command, CancellationToken ct)
    {
        var protocol = await this.ExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamDutyProtocolId!.Value,
            ct);

        foreach (var student in command.Students!)
        {
            protocol.AddStudent(
                student.ClassId!.Value,
                student.PersonId!.Value,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);

        return protocol.ExamDutyProtocolId;
    }
}
