namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveHighSchoolCertificateProtocolStudentCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> HighSchoolCertificateProtocolAggregateRepository)
    : IRequestHandler<RemoveHighSchoolCertificateProtocolStudentCommand>
{
    public async Task Handle(RemoveHighSchoolCertificateProtocolStudentCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = await this.HighSchoolCertificateProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.HighSchoolCertificateProtocolId!.Value,
            ct);

        stateExamsAdmProtocol.RemoveStudent(
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
