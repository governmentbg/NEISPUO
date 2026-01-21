namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record UpdateShiftCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IShiftsQueryRepository ShiftsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<UpdateShiftCommand, int>
{
    public async Task<int> Handle(UpdateShiftCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.CheckSchoolYearAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            ct))
        {
            throw new DomainValidationException($"The school year is locked.");
        }

        var shift = await this.ShiftAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ShiftId!.Value,
            ct);

        if (command.InstId!.Value != shift.InstId)
        {
            // the instId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.InstId)}.");
        }

        if (shift.IsAdhoc)
        {
            throw new DomainValidationException("Cannot update adhoc shift");
        }

        shift.UpdateData(
            command.Name!,
            command.IsMultiday!.Value,
            command.Days!.SelectMany(d => d.Hours!.Select(h => (
                day: d.Day!.Value,
                hourNumber: h.HourNumber!.Value,
                startTime: h.StartTime!,
                endTime: h.EndTime!
            ))).ToArray(),
            command.SysUserId!.Value);

        var newShiftHours = shift.Hours.Select(h => (day: h.Day, hourNumber: h.HourNumber));
        var usedHourNumbers =
            (await this.ShiftsQueryRepository.GetHoursUsedInScheduleAsync(
                shift.SchoolYear,
                shift.InstId,
                shift.ShiftId,
                ct))
            .Select(h => (day: h.Day, hourNumber: h.HourNumber));

        var excess = usedHourNumbers.Except(newShiftHours);
        if (excess.Any())
        {
            string excessStr = string.Join(", ", excess.Select(sh => $"(day: {sh.day}, hourNumber: {sh.hourNumber})"));
            throw new DomainValidationException($"The updated shift's (day, hourNumber) pairs should be a superset of the used (day, hourNumber) pairs by schedules. The excess is {excessStr}.");
        }

        await this.UnitOfWork.SaveAsync(ct);

        return shift.ShiftId;
    }
}
