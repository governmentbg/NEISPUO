namespace SB.Domain;

using System.Linq;
using FluentValidation;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator(IValidator<CreateScheduleCommandShiftHour> scheduleDayHourValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.IsIndividualSchedule).NotNull();
        this.When(c => c.IsIndividualSchedule == false, () =>
        {
            this.RuleFor(c => c.PersonId).Null();
        });
        this.When(c => c.IsIndividualSchedule == true, () =>
        {
            this.RuleFor(c => c.PersonId).NotNull();
        });
        this.RuleFor(c => c.Term).IsInEnum();
        this.RuleFor(c => c.StartDate).NotNull();
        this.RuleFor(c => c.EndDate).NotNull();
        this.RuleFor(c => c.IncludesWeekend).NotEmpty();
        this.RuleFor(c => c.HasAdhocShift).NotEmpty();
        this.When(c => c.HasAdhocShift == false, () =>
        {
            this.RuleFor(c => c.ShiftId).NotNull();
            this.RuleFor(c => c.AdhocShiftIsMultiday).Null();
            this.RuleFor(c => c.AdhocShiftDays).Null();
        });
        this.When(c => c.HasAdhocShift == true, () =>
        {
            this.RuleFor(c => c.ShiftId).Null();
            this.RuleFor(c => c.AdhocShiftIsMultiday).NotNull();
            this.RuleFor(c => c.AdhocShiftDays).NotEmpty();
            this.RuleForEach(c => c.AdhocShiftDays)
                .ChildRules(day =>
                {
                    day.RuleForEach(d => d.Hours).SetValidator(scheduleDayHourValidator);
                });
        });
        this.RuleFor(c => c.Weeks).NotEmpty();
        this.RuleFor(c => c.Days).NotEmpty();

        this.RuleFor(c => c)
            .Custom((c, context) =>
            {
                if (c.HasAdhocShift!.Value)
                {
                    foreach (var duplicateDay in c.AdhocShiftDays!.GroupBy(d => d.Day).Where(g => g.Count() > 1))
                    {
                        context.AddUnexpectedFailure($"Adhoc shift day #{duplicateDay.Key} appears twice");
                    }
                }
            });
    }
}
