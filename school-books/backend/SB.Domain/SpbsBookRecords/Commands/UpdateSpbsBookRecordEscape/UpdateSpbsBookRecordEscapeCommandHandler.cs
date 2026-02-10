namespace SB.Domain;

using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

internal record UpdateSpbsBookRecordEscapeCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<SpbsBookRecord> SpbsBookRecordAggregateRepository)
    : IRequestHandler<UpdateSpbsBookRecordEscapeCommand>
{
    public async Task Handle(UpdateSpbsBookRecordEscapeCommand command, CancellationToken ct)
    {
        var spbsBookRecord = await this.SpbsBookRecordAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.SpbsBookRecordId!.Value,
            ct);

        spbsBookRecord.UpdateEscape(
            command.OrderNum!.Value,
            command.EscapeDate!.Value,
            TimeSpan.Parse(command.EscapeTime!),
            command.PoliceNotificationDate!.Value,
            TimeSpan.Parse(command.PoliceNotificationTime!),
            command.PoliceLetterNumber!,
            command.PoliceLetterDate!.Value,
            command.ReturnDate,
            command.SysUserId!.Value);
        await this.UnitOfWork.SaveAsync(ct);
    }
}
