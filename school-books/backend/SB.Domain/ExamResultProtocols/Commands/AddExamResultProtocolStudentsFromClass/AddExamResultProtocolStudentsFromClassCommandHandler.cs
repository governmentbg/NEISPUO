namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record AddExamResultProtocolStudentsFromClassCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamResultProtocol> ExamResultProtocolAggregateRepository,
    IExamDutyProtocolsQueryRepository ExamDutyProtocolsQueryRepository)
    : IRequestHandler<AddExamResultProtocolStudentsFromClassCommand>
{
    public async Task Handle(AddExamResultProtocolStudentsFromClassCommand command, CancellationToken ct)
    {
        var classId = command.ClassId!.Value;

        var protocol = await this.ExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamResultProtocolId!.Value,
            ct);

        var personIds = await this.ExamDutyProtocolsQueryRepository.GetStudentPersonIdsByClassIdAsync(
            command.SchoolYear!.Value,
            classId,
            protocol.Students.Select(s => s.PersonId).ToArray(),
            ct);

        foreach (var personId in personIds)
        {
            protocol.AddStudent(
                classId,
                personId,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
