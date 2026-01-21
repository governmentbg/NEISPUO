namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveAbsenceCommandValidator : AbstractValidator<RemoveAbsenceCommand>
{
    public RemoveAbsenceCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.AbsenceId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.IsExternal == false)
                {
                    var absencesQueryRepository = context.GetServiceProvider().GetRequiredService<IAbsencesQueryRepository>();

                    var existsVerifiedScheduleLessonForAbsence = await absencesQueryRepository.ExistsVerifiedScheduleLessonForAbsencesAsync(
                        c.SchoolYear!.Value,
                        new [] { c.AbsenceId!.Value },
                        ct);

                    if (existsVerifiedScheduleLessonForAbsence)
                    {
                        context.AddUserFailure($"Отсъствието е въведено за час, който е проверен от директора.");
                    }
                }
            });
    }
}
