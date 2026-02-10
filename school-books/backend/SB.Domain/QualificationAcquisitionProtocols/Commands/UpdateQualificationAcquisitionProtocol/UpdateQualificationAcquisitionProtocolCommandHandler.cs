namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateQualificationAcquisitionProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationAcquisitionProtocol> QualificationAcquisitionProtocolAggregateRepository)
    : IRequestHandler<UpdateQualificationAcquisitionProtocolCommand>
{
    public async Task Handle(UpdateQualificationAcquisitionProtocolCommand command, CancellationToken ct)
    {
        var protocol = await this.QualificationAcquisitionProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationAcquisitionProtocolId!.Value,
            ct);

        protocol.UpdateData(
            command.ProtocolNumber,
            command.ProtocolDate,
            command.Profession!,
            command.Speciality!,
            command.QualificationDegreeId!.Value,
            command.Date!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.DirectorPersonId!.Value,
            command.CommissionChairman!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
