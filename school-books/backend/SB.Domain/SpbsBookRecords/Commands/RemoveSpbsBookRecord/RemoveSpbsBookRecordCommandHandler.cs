namespace SB.Domain;

using MediatR;
using System.Threading;
using System.Threading.Tasks;

internal record RemoveSpbsBookRecordCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository)
    : IRequestHandler<RemoveSpbsBookRecordCommand>
{
    public async Task Handle(RemoveSpbsBookRecordCommand command, CancellationToken ct)
    {
        var spbsBookRecord = await this.SpbsBookRecordAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SpbsBookRecordId!.Value,
            ct);

        this.SpbsBookRecordAggregateRepository.Remove(spbsBookRecord);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
