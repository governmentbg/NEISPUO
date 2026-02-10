namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

public class CreateOffDayCommandValidator : AbstractValidator<CreateOffDayCommand>
{
    public CreateOffDayCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.From).NotNull();
        this.RuleFor(c => c.To).NotNull();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(1000);
        this.RuleFor(c => c.IsForAllClasses).NotNull();
        this.RuleFor(c => c.IsPgOffProgramDay).NotNull();

        this.RuleFor(c => c)
            .Custom((c, context) =>
            {
                if (c.From!.Value > c.To!.Value)
                {
                    context.AddUserFailure("Началната дата не може да е след крайната.");
                }
            })
            .CustomAsync(async (c, context, ct) =>
            {
                if (c is UpdateOffDayCommand)
                {
                    // do not apply this rule in the update command
                    return;
                }

                var schoolYear = c.SchoolYear!.Value;
                var instId = c.InstId!.Value;

                var оffDaysQueryRepository = context.GetServiceProvider().GetRequiredService<IOffDaysQueryRepository>();

                bool isForAllClasses = c.IsForAllClasses!.Value;
                if (isForAllClasses &&
                    (await оffDaysQueryRepository.ExistOffDayForAllClassesAsync(
                        schoolYear,
                        instId,
                        null,
                        c.From!.Value,
                        c.To!.Value,
                        ct)))
                {
                    context.AddUserFailure("Вече има създадени неучебни дни за всички класове за посочения период.");
                }

                if (!isForAllClasses &&
                    c.BasicClassIds != null &&
                    c.BasicClassIds.Length > 0 &&
                    (await оffDaysQueryRepository.ExistOffDayForClassesAsync(
                        schoolYear,
                        instId,
                        null,
                        c.From!.Value,
                        c.To!.Value,
                        c.BasicClassIds,
                        ct)))
                {
                    context.AddUserFailure("Вече има създадени неучебни дни за някои от тези класове за посочения период.");
                }

                if (!isForAllClasses &&
                    c.ClassBookIds != null &&
                    c.ClassBookIds.Length > 0 &&
                    (await оffDaysQueryRepository.ExistOffDayForClassBooksAsync(
                        schoolYear,
                        instId,
                        null,
                        c.From!.Value,
                        c.To!.Value,
                        c.ClassBookIds,
                        ct)))
                {
                    context.AddUserFailure("Вече има създадени неучебни дни за някои от тези паралелки/групи за посочения период.");
                }

                if (
                    await оffDaysQueryRepository.HasHoursInUseAsync(
                        schoolYear,
                        instId,
                        c.From!.Value,
                        c.To!.Value,
                        c.IsForAllClasses ?? false,
                        c.BasicClassIds ?? Array.Empty<int>(),
                        c.ClassBookIds ?? Array.Empty<int>(),
                        ct))
                {
                    context.AddUserFailure("За посочения период има часове, за които има въведени оценки/отсъствия/теми/учителски отсъствия или часове, маркирани като лекторски. " +
                        "За улеснение при проверка на това в кои дневници е нанесена тази информация, може да използвате функционалността \"Администрация > Проверка на дневници\".");
                }
            });
    }
}
