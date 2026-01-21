namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateQualificationExamResultProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository)
    : IRequestHandler<CreateQualificationExamResultProtocolStudentCommand>
{
    public async Task Handle(CreateQualificationExamResultProtocolStudentCommand command, CancellationToken ct)
    {
        var protocol = await this.QualificationExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationExamResultProtocolId!.Value,
            ct);

        foreach (var student in command.Students!)
        {
            protocol.AddStudent(
                student.ClassId!.Value,
                student.PersonId!.Value,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
