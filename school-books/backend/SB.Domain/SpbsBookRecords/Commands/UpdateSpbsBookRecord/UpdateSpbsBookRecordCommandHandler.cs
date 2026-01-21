namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateSpbsBookRecordCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository)
    : IRequestHandler<UpdateSpbsBookRecordCommand>
{
    public async Task Handle(UpdateSpbsBookRecordCommand command, CancellationToken ct)
    {
        var spbsBookRecord = await this.SpbsBookRecordAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SpbsBookRecordId!.Value,
            ct);

        spbsBookRecord.Update(
            command.SendingCommission,
            command.SendingCommissionAddress,
            command.SendingCommissionPhoneNumber,
            command.InspectorNames,
            command.InspectorAddress,
            command.InspectorPhoneNumber,
            command.CourtDecisionNumber,
            command.CourtDecisionDate,
            command.IncomingInstId,
            command.IncommingLetterNumber,
            command.IncommingLetterDate,
            command.IncommingDate,
            command.IncommingDocNumber,
            command.TransferInstId,
            command.TransferReason,
            command.TransferProtocolNumber,
            command.TransferProtocolDate,
            command.TransferLetterNumber,
            command.TransferLetterDate,
            command.TransferCertificateNumber,
            command.TransferCertificateDate,
            command.TransferMessageNumber,
            command.TransferMessageDate,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);
    }
}
