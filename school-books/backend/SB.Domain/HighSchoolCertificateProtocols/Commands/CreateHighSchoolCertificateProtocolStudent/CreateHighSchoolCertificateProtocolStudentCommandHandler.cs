namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateHighSchoolCertificateProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> HighSchoolCertificateProtocolCommandAggregateRepository)
    : IRequestHandler<CreateHighSchoolCertificateProtocolStudentCommand>
{
    public async Task Handle(CreateHighSchoolCertificateProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.HighSchoolCertificateProtocolCommandAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.HighSchoolCertificateProtocolId!.Value,
            ct);

        foreach (var student in command.Students!)
        {
            stateExamsAdmProtocol.AddStudent(
                student.ClassId!.Value,
                student.PersonId!.Value,
                command.SysUserId!.Value);
        }

        await this.UnitOfWork.SaveAsync(ct);
    }
}
