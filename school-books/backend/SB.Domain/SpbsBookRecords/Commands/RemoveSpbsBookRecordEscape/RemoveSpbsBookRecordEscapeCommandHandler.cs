namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveSpbsBookRecordEscapeCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository)
    : IRequestHandler<RemoveSpbsBookRecordEscapeCommand>
{
    public async Task Handle(RemoveSpbsBookRecordEscapeCommand command, CancellationToken ct)
    {
        var spbsBookRecord = await this.SpbsBookRecordAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SpbsBookRecordId!.Value,
            ct);

        spbsBookRecord.RemoveEscape(command.OrderNum!.Value, command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
