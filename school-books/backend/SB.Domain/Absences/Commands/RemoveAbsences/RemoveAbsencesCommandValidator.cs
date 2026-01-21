namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class RemoveAbsencesCommandValidator : AbstractValidator<RemoveAbsencesCommand>
{
    public RemoveAbsencesCommandValidator(IValidator<RemoveAbsencesCommandAbsence> absenceValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Absences).NotEmpty();
        this.RuleForEach(c => c.Absences).SetValidator(absenceValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var absencesQueryRepository = context.GetServiceProvider().GetRequiredService<IAbsencesQueryRepository>();

                var existsVerifiedScheduleLessonForAbsences = await absencesQueryRepository.ExistsVerifiedScheduleLessonForAbsencesAsync(
                    c.SchoolYear!.Value,
                    c.Absences!.Select(a => a.AbsenceId!.Value).ToArray(),
                    ct);

                if (existsVerifiedScheduleLessonForAbsences)
                {
                    context.AddUserFailure($"Не може да се изтрие отсъствие въведено за час, който е проверен от директора.");
                }
            });
    }
}
