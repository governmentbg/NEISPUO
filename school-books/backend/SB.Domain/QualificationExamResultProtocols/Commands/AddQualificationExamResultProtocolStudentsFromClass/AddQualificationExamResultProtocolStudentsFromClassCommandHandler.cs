namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record AddQualificationExamResultProtocoltudentsFromClassCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationExamResultProtocol> QualificationExamResultProtocolAggregateRepository,
    IExamDutyProtocolsQueryRepository ExamDutyProtocolsQueryRepository)
    : IRequestHandler<AddQualificationExamResultProtocolStudentsFromClassCommand>
{
    public async Task Handle(AddQualificationExamResultProtocolStudentsFromClassCommand command, CancellationToken ct)
    {
        var classId = command.ClassId!.Value;

        var protocol = await this.QualificationExamResultProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationExamResultProtocolId!.Value,
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
