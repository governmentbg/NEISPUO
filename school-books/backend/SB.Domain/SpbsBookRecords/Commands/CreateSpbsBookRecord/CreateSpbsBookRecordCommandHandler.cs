namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateSpbsBookRecordCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository,
    ISpbsBookRecordsQueryRepository SpbsBookRecordsQueryRepository)
    : IRequestHandler<CreateSpbsBookRecordCommand, int>
{
    public async Task<int> Handle(CreateSpbsBookRecordCommand command, CancellationToken ct)
    {
        var spbsBookRecord = new SpbsBookRecord(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            1,
            command.ClassId!.Value,
            command.PersonId!.Value,
            command.SysUserId!.Value);

        await this.SpbsBookRecordAggregateRepository.AddAsync(spbsBookRecord, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return spbsBookRecord.SpbsBookRecordId;
    }
}
