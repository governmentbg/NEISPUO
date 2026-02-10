namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateHighSchoolCertificateProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> HighSchoolCertificateProtocolAggregateRepository)
    : IRequestHandler<UpdateHighSchoolCertificateProtocolCommand, int>
{
    public async Task<int> Handle(UpdateHighSchoolCertificateProtocolCommand command, CancellationToken ct)
    {
        var highSchoolCertificateProtocol = await this.HighSchoolCertificateProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.HighSchoolCertificateProtocolId!.Value,
            ct);

        highSchoolCertificateProtocol.UpdateData(
            command.Stage!,
            command.ProtocolNum,
            command.ProtocolDate,
            command.CommissionMeetingDate!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.ExamSession,
            command.DirectorPersonId!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return highSchoolCertificateProtocol.HighSchoolCertificateProtocolId;
    }
}
