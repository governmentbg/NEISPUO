namespace SB.Domain;

using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal record AddHighSchoolCertificateProtocolStudentsFromClassCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> HighSchoolCertificateProtocolAggregateRepository,
    IExamDutyProtocolsQueryRepository ExamDutyProtocolsQueryRepository)
    : IRequestHandler<AddHighSchoolCertificateProtocolStudentsFromClassCommand>
{
    public async Task Handle(AddHighSchoolCertificateProtocolStudentsFromClassCommand command, CancellationToken ct)
    {
        var classId = command.ClassId!.Value;

        var protocol = await this.HighSchoolCertificateProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.HighSchoolCertificateProtocolId!.Value,
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
