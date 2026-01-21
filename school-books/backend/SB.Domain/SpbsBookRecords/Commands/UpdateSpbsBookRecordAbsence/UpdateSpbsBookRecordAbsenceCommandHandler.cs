namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateSpbsBookRecordAbsenceCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository)
    : IRequestHandler<UpdateSpbsBookRecordAbsenceCommand>
{
    public async Task Handle(UpdateSpbsBookRecordAbsenceCommand command, CancellationToken ct)
    {
        var spbsBookRecord = await this.SpbsBookRecordAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SpbsBookRecordId!.Value,
            ct);

        spbsBookRecord.UpdateAbsence(
            command.OrderNum!.Value,
            command.AbsenceDate!.Value,
            command.AbsenceReason!,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
