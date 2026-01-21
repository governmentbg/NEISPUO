namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveQualificationAcquisitionProtocolCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<QualificationAcquisitionProtocol> QualificationAcquisitionProtocolAggregateRepository)
    : IRequestHandler<RemoveQualificationAcquisitionProtocolCommand>
{
    public async Task Handle(RemoveQualificationAcquisitionProtocolCommand command, CancellationToken ct)
    {
        var exam = await this.QualificationAcquisitionProtocolAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.QualificationAcquisitionProtocolId!.Value,
            ct);
        this.QualificationAcquisitionProtocolAggregateRepository.Remove(exam);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
