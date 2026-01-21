namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateExamResultProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository)
    : IRequestHandler<CreateExamResultProtocolStudentCommand, int>
{
    public async Task<int> Handle(CreateExamResultProtocolStudentCommand command, CancellationToken ct)
    {
        var protocol = await this.ExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamResultProtocolId!.Value,
            ct);

        foreach (var student in command.Students!)
        {
            protocol.AddStudent(
                student.ClassId!.Value,
                student.PersonId!.Value,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);

        return protocol.ExamResultProtocolId;
    }
}
