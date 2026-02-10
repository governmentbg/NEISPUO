namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record AddExamDutyProtocolStudentsFromClassCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<ExamDutyProtocol> ExamDutyProtocolAggregateRepository,
    IExamDutyProtocolsQueryRepository ExamDutyProtocolsQueryRepository)
    : IRequestHandler<AddExamDutyProtocolStudentsFromClassCommand>
{
    public async Task Handle(AddExamDutyProtocolStudentsFromClassCommand command, CancellationToken ct)
    {
        var classId = command.ClassId!.Value;

        var protocol = await this.ExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ExamDutyProtocolId!.Value,
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
