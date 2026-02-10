namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateShiftCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateShiftCommand, int>
{
    public async Task<int> Handle(CreateShiftCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var shift = new Shift(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Name!,
            command.IsMultiday!.Value,
            false,
            command.Days!.SelectMany(d => d.Hours!.Select(h => (
                day: d.Day!.Value,
                hourNumber: h.HourNumber!.Value,
                startTime: h.StartTime!,
                endTime: h.EndTime!
            ))).ToArray(),
            command.SysUserId!.Value);

        await this.ShiftAggregateRepository.AddAsync(shift, ct);
        await this.UnitOfWork.SaveAsync(ct);

        return shift.ShiftId;
    }
}
