namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

public class UpdateOffDayCommandValidator : AbstractValidator<UpdateOffDayCommand>
{
    public UpdateOffDayCommandValidator(IValidator<CreateOffDayCommand> createValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(s => (CreateOffDayCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.OffDayId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var schoolYear = c.SchoolYear!.Value;
                var instId = c.InstId!.Value;

                var оffDaysQueryRepository = context.GetServiceProvider().GetRequiredService<IOffDaysQueryRepository>();

                bool isForAllClasses = c.IsForAllClasses!.Value;
                if (isForAllClasses &&
                    (await оffDaysQueryRepository.ExistOffDayForAllClassesAsync(
                        schoolYear,
                        instId,
                        c.OffDayId!.Value,
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
                        c.OffDayId!.Value,
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
                        c.OffDayId!.Value,
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
