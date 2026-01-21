namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveHighSchoolCertificateProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> HighSchoolCertificateProtocolAggregateRepository)
    : IRequestHandler<RemoveHighSchoolCertificateProtocolCommand>
{
    public async Task Handle(RemoveHighSchoolCertificateProtocolCommand command, CancellationToken ct)
    {
        var highSchoolCertificateProtocol = await this.HighSchoolCertificateProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.HighSchoolCertificateProtocolId!.Value,
            ct);

        this.HighSchoolCertificateProtocolAggregateRepository.Remove(highSchoolCertificateProtocol);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
