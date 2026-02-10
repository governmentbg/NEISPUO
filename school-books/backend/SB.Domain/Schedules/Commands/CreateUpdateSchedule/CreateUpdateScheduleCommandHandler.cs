namespace SB.Domain;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

internal record CreateScheduleCommandHandler(
    IUnitOfWork UnitOfWork,
    IScopedAggregateRepository<Shift> ShiftAggregateRepository,
    IScopedAggregateRepository<Schedule> ScheduleAggregateRepository,
    ISchedulesQueryRepository SchedulesQueryRepository,
    IShiftsQueryRepository ShiftsQueryRepository,
    IClassBookCachedQueryStore ClassBookCachedQueryStore)
    : IRequestHandler<CreateScheduleCommand, int>, IRequestHandler<UpdateScheduleCommand, int>
{
    public async Task<int> Handle(CreateScheduleCommand command, CancellationToken ct)
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

        if (command.IsIndividualSchedule!.Value &&
            !await this.ClassBookCachedQueryStore.ExistsStudentForClassBookAsync(
                command.SchoolYear!.Value,
                command.ClassBookId!.Value,
                command.PersonId!.Value,
                ct))
        {
            throw new DomainValidationException("This person is not in the class book students list");
        }

        var usedDays = this.CalcUsedDays(command.Days!);
        var offDayDates = await this.SchedulesQueryRepository.GetOffDatesAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);

        var hours = this.CalcHours(command.Days!);

        if (hours.Length == 0)
        {
            throw new DomainValidationException(
                new[] { "No hours were specified for the schedule" },
                new[] { $"Няма свободни дни за избраните седмици в периода {command.StartDate!.Value:dd.MM.yyyy} - {command.EndDate!.Value:dd.MM.yyyy}, които не се използват в други разписания." });
        }

        var dates = await Schedule.CalcDatesAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.IsIndividualSchedule!.Value,
            command.PersonId,
            null,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.Weeks!,
            usedDays,
            this.SchedulesQueryRepository,
            ct);

        dates = dates.Except(offDayDates).ToArray(); // remove off-days from the calculated dates

        if (dates.Length == 0)
        {
            throw new DomainValidationException(
                new[] { "No hours were specified for the schedule" },
                new[] { $"Няма свободни дни за избраните седмици в периода {command.StartDate!.Value:dd.MM.yyyy} - {command.EndDate!.Value:dd.MM.yyyy}, които не се използват в други разписания." });
        }

        (int day, int hourNumber)[] shiftHours;
        Shift? adhocShift = null;
        if (command.HasAdhocShift!.Value)
        {
            adhocShift = new Shift(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                string.Empty,
                command.AdhocShiftIsMultiday!.Value,
                true,
                command.AdhocShiftDays!.SelectMany(d => d.Hours!.Select(h => (
                    day: d.Day!.Value,
                    hourNumber: h.HourNumber!.Value,
                    startTime: h.StartTime!,
                    endTime: h.EndTime!
                ))).ToArray(),
                command.SysUserId!.Value);
            await this.ShiftAggregateRepository.AddAsync(adhocShift, ct);

            shiftHours = adhocShift.Hours.Select(h => (h.Day, h.HourNumber)).ToArray();
        }
        else
        {
            shiftHours =
                (await this.SchedulesQueryRepository.GetShiftHoursForValidationAsync(
                    command.SchoolYear!.Value, command.InstId!.Value, command.ShiftId!.Value, ct))
                .Select(sh => (day: sh.Day, hourNumber: sh.HourNumber))
                .ToArray();
        }

        var scheduleHours = command.Days!.SelectMany(d => d.Hours.Select(h => (day: d.Day, hourNumber: h.HourNumber)));
        var scheduleHoursExcess = scheduleHours.Except(shiftHours);
        if (scheduleHoursExcess.Any())
        {
            string excessStr = string.Join(", ", scheduleHoursExcess.Select(sh => $"(day: {sh.day}, hourNumber: {sh.hourNumber})"));
            throw new DomainValidationException($"The shift's (day, hourNumber) pairs should be a superset of the schedule's (day, hourNumber) pairs. The excess is {excessStr}.");
        }

        var curriculumIds =
            command.Days!.SelectMany(d =>
                d.Hours.SelectMany(h =>
                    h.Groups.Where(g => g.CurriculumId != null).Select(g => g.CurriculumId!.Value)));

        foreach (var curriculumId in curriculumIds)
        {
            if (!await this.ClassBookCachedQueryStore.ExistsCurriculumForClassBookAsync(
                    command.SchoolYear!.Value,
                    command.ClassBookId!.Value,
                    curriculumId,
                    ct))
            {
                throw new DomainValidationException($"This curriculum ({curriculumId}) is not in the class book curriculum list");
            }
        }

        var schedule = new Schedule(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.IsIndividualSchedule!.Value,
            command.IsIndividualSchedule!.Value ? command.PersonId!.Value : null,
            command.Term,
            command.StartDate!.Value,
            command.EndDate!.Value,
            adhocShift != null ? adhocShift.ShiftId : command.ShiftId!.Value,
            dates,
            offDayDates,
            hours,
            command.IncludesWeekend!.Value,
            command.SysUserId!.Value);

        await this.ScheduleAggregateRepository.AddAsync(schedule, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return schedule.ScheduleId;
    }

    public async Task<int> Handle(UpdateScheduleCommand command, CancellationToken ct)
    {
        var usedDays = this.CalcUsedDays(command.Days!);

        var hours = this.CalcHours(command.Days!);

        if (hours.Length == 0)
        {
            throw new DomainValidationException("No hours were specified for the schedule");
        }

        var schedule = await this.ScheduleAggregateRepository.FindAsync(
            command.SchoolYear!.Value,
            command.ScheduleId!.Value,
            ct);

        if (command.ClassBookId!.Value != schedule.ClassBookId ||
            command.IsIndividualSchedule!.Value != schedule.IsIndividualSchedule ||
            command.PersonId != schedule.PersonId)
        {
            throw new DomainValidationException("Cannot change ClassBookId, IsIndividualSchedule, PersonId when updating schedule");
        }

        if (command.ClassBookId!.Value != schedule.ClassBookId)
        {
            // the classBookId check is required as it is part of the auth checks
            throw new DomainValidationException($"Incorrect {nameof(command.ClassBookId)}.");
        }

        var offDayDates = await this.SchedulesQueryRepository.GetOffDatesAsync(command.SchoolYear!.Value, command.ClassBookId!.Value, ct);

        var dates = await Schedule.CalcDatesAsync(
            command.SchoolYear!.Value,
            command.ClassBookId!.Value,
            command.IsIndividualSchedule!.Value,
            command.PersonId,
            command.ScheduleId!.Value,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.Weeks!,
            usedDays,
            this.SchedulesQueryRepository,
            ct);

        if (dates.Length == 0)
        {
            throw new DomainValidationException("No dates were selected for the schedule");
        }

        Shift? newShift = null;
        if (command.HasAdhocShift!.Value)
        {
            var currentShift = await this.ShiftAggregateRepository.FindAsync(
                command.SchoolYear!.Value,
                schedule.ShiftId,
                ct);

            if (currentShift.IsAdhoc)
            {
                // updating the ad-hoc shift

                currentShift.UpdateData(
                    currentShift.Name,
                    command.AdhocShiftIsMultiday!.Value,
                    command.AdhocShiftDays!.SelectMany(d => d.Hours!.Select(h => (
                        day: d.Day!.Value,
                        hourNumber: h.HourNumber!.Value,
                        startTime: h.StartTime!,
                        endTime: h.EndTime!
                    ))).ToArray(),
                    command.SysUserId!.Value);

                newShift = currentShift;
            }
            else
            {
                // changing from regular shift to a ad-hoc shift

                throw new DomainValidationException("Changing from regular shift to a ad-hoc shift is not allowed.");
            }
        }
        else if (command.ShiftId != schedule.ShiftId)
        {
            // changing from regular shift to a new regular shift

            newShift = await this.ShiftAggregateRepository.FindAsync(
                command.SchoolYear!.Value,
                command.ShiftId!.Value,
                ct);
        }

        if (newShift != null)
        {
            var shiftHours = newShift.Hours.Select(h => (day: h.Day, hourNumber: h.HourNumber)).ToArray();

            var scheduleHours = command.Days!.SelectMany(d => d.Hours.Select(h => (day: d.Day, hourNumber: h.HourNumber)));
            var scheduleHoursExcess = scheduleHours.Except(shiftHours);
            if (scheduleHoursExcess.Any())
            {
                string excessStr = string.Join(", ", scheduleHoursExcess.Select(sh => $"(day: {sh.day}, hourNumber: {sh.hourNumber})"));
                throw new DomainValidationException($"The shift's (day, hourNumber) pairs should be a superset of the schedule's (day, hourNumber) pairs. The excess is {excessStr}.");
            }

            var usedScheduleHours =
                (await this.SchedulesQueryRepository.GetScheduleUsedHoursAsync(
                    command.SchoolYear!.Value,
                    command.InstId!.Value,
                    command.ClassBookId!.Value,
                    command.ScheduleId!.Value,
                    ct))
                .Select(h => (day: h.Day, hourNumber: h.HourNumber));

            var usedScheduleHoursExcess = usedScheduleHours.Except(shiftHours);
            if (usedScheduleHoursExcess.Any())
            {
                // all items attached to a ScheduleLesson should have FKs so this just provides
                // a more informative validation error instead of a general exception that will
                // show up as 500 response

                string excessStr = string.Join(", ", usedScheduleHoursExcess.Select(sh => $"(day: {sh.day}, hourNumber: {sh.hourNumber})"));
                throw new DomainValidationException($"The shift's (day, hourNumber) pairs should be a superset of the used (day, hourNumber) pairs in the schedule. The excess is {excessStr}.");
            }
        }

        schedule.UpdateData(
            command.Term,
            command.StartDate!.Value,
            command.EndDate!.Value,
            command.IncludesWeekend!.Value,
            newShift?.ShiftId ?? schedule.ShiftId,
            dates,
            offDayDates,
            hours,
            command.SysUserId!.Value);

        await this.UnitOfWork.SaveAsync(ct);

        return schedule.ScheduleId;
    }

    private int[] CalcUsedDays(CreateScheduleCommandDay[] days)
    {
        return days.Where(d =>
                d.Hours.Any(h =>
                    h.Groups.Any(g => g.CurriculumId.HasValue)))
            .Select(d => d.Day)
            .ToArray();
    }

    private (int day, int hourNumber, int curriculumId, string? location)[] CalcHours(CreateScheduleCommandDay[] days)
    {
        return days!.SelectMany(day =>
            day.Hours.SelectMany(hour =>
                hour.Groups
                    .Where(g => g.CurriculumId.HasValue)
                    .Select(group => (
                        day: day.Day,
                        hourNumber: hour.HourNumber,
                        curriculumId: group.CurriculumId!.Value,
                        location: group.Location))))
            .ToArray();
    }
}
