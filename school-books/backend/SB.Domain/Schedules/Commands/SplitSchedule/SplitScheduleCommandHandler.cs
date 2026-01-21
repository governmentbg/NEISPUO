namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record SplitScheduleCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IScopedAggregateRepository<Schedule> ScheduleAggregateRepository,
    ISchedulesQueryRepository SchedulesQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<SplitScheduleCommand, int>
{
    public async Task<int> Handle(SplitScheduleCommand command, CancellationToken ct)
    {
        if (!await this.ClassBookCachedQueryStore.ExistsClassBookAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"A classbook with (schoolYear:{command.SchoolYear!.Value}, classBookId:{command.ClassBookId!.Value}) does not exist.");
        }

        if (!await this.ClassBookCachedQueryStore.CheckClassBookIsValidAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                ct))
        {
            throw new DomainValidationException($"The classbook is marked as invalid (archived).");
        }

        if (!await this.ClassBookCachedQueryStore.CheckBookAllowsModificationsAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            ct))
        {
            throw new DomainValidationException($"The classbook is locked.");
        }

        var schedule = await this.ScheduleAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ScheduleId!.Value,
            ct);

        if (command.ClassBookId!.Value != schedule.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        var usedDays = schedule.Hours.Select(h => h.Day).Distinct().ToArray();
        var offDayDates = await this.SchedulesQueryRepository.GetOffDatesAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);

        var splitDates = await Schedule.CalcDatesAsync(
            schedule.SchoolYear,
            schedule.ClassBookId,
            schedule.IsIndividualSchedule,
            schedule.PersonId,
            schedule.ScheduleId,
            schedule.StartDate,
            schedule.EndDate,
            command.Weeks!,
            usedDays,
            this.SchedulesQueryRepository,
            ct);

        if (splitDates.Length == 0)
        {
            throw new DomainValidationException("No dates were selected for split");
        }

        var shift = await this.ShiftAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            schedule.ShiftId,
            ct);

        Shift? clonedAdhocShift = null;
        if (shift.IsAdhoc)
        {
            clonedAdhocShift = shift.CloneAdhocShift(command.SysUserId!.Value);
            await this.ShiftAggregateRepository.AddAsync(clonedAdhocShift, ct);
        }

        var splitSchedule = schedule.SplitSchedule(
            splitDates,
            clonedAdhocShift,
            command.SysUserId!.Value);

        await this.ScheduleAggregateRepository.AddAsync(splitSchedule, ct);

        await using var transaction = await this.UnitOfWork.BeginTransactionAsync(ct);

        await this.UnitOfWork.SaveAsync(ct);

        splitSchedule.FinalizeSplitting();

        await this.UnitOfWork.SaveAsync(ct);

        await transaction.CommitAsync(ct);

        return splitSchedule.ScheduleId;
    }
}
