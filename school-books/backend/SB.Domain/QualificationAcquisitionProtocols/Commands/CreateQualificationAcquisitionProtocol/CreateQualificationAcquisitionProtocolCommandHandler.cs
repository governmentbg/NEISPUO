namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record CreateQualificationAcquisitionProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationAcquisitionProtocol> QualificationAcquisitionProtocolAggregateRepository)
    : IRequestHandler<CreateQualificationAcquisitionProtocolCommand, int>
{
    public async Task<int> Handle(CreateQualificationAcquisitionProtocolCommand command, CancellationToken ct)
    {
        var qualificationAcquisitionProtocol = new QualificationAcquisitionProtocol(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.ProtocolType!.Value,
            command.ProtocolNumber,
            command.ProtocolDate,
            command.Profession!,
            command.Speciality!,
            command.QualificationDegreeId!.Value,
            command.Date!.Value,
            command.CommissionNominationOrderNumber!,
            command.CommissionNominationOrderDate!.Value,
            command.CommissionChairman!.Value,
            command.DirectorPersonId!.Value,
            command.CommissionMembers!,
            command.SysUserId!.Value);

        await this.QualificationAcquisitionProtocolAggregateRepository.AddAsync(qualificationAcquisitionProtocol, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return qualificationAcquisitionProtocol.QualificationAcquisitionProtocolId;
    }
}
