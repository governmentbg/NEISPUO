namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateAbsenceCommandValidator : AbstractValidator<CreateAbsenceCommand>
{
    public CreateAbsenceCommandValidator(IValidator<CreateAbsenceCommandAbsence> absenceValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.Absences).NotNull();
        this.RuleForEach(c => c.Absences).SetValidator(absenceValidator);

        this.RuleFor(c => c.ExcusedReasonComment).MaximumLength(1000);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.Absences!.Any())
                {
                    var absencesQueryRepository = context.GetServiceProvider().GetRequiredService<IAbsencesQueryRepository>();

                    var scheduleLessonIsVerified = await absencesQueryRepository.ExistsVerifiedScheduleLessonAsync(
                        c.SchoolYear!.Value,
                        c.Absences!.Select(a => a.ScheduleLessonId!.Value).ToArray(),
                        ct);

                    if (scheduleLessonIsVerified)
                    {
                        context.AddUserFailure($"Не може да се въведе отсъствие за час, който е проверен от директора.");
                    }
                }
            });
    }
}
