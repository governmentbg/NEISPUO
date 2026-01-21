namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record AddNvoExamDutyProtocolStudentsFromClassCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<NvoExamDutyProtocol> NvoExamDutyProtocolAggregateRepository,
    IExamDutyProtocolsQueryRepository ExamDutyProtocolsQueryRepository)
    : IRequestHandler<AddNvoExamDutyProtocolStudentsFromClassCommand>
{
    public async Task Handle(AddNvoExamDutyProtocolStudentsFromClassCommand command, CancellationToken ct)
    {
        var classId = command.ClassId!.Value;

        var protocol = await this.NvoExamDutyProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.NvoExamDutyProtocolId!.Value,
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
