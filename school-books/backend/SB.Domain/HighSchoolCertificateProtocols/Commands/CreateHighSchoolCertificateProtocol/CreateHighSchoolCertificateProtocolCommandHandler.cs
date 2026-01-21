namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateHighSchoolCertificateProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<HighSchoolCertificateProtocol> StateExamsAdmProtocolCommandAggregateRepository)
    : IRequestHandler<CreateHighSchoolCertificateProtocolCommand, int>
{
    public async Task<int> Handle(CreateHighSchoolCertificateProtocolCommand command, CancellationToken ct)
    {
        var stateExamsAdmProtocol = new HighSchoolCertificateProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
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

        await this.StateExamsAdmProtocolCommandAggregateRepository.AddAsync(stateExamsAdmProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return stateExamsAdmProtocol.HighSchoolCertificateProtocolId;
    }
}
