namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

public class CreateSchoolYearSettingsCommandValidator : AbstractValidator<CreateSchoolYearSettingsCommand>
{
    public CreateSchoolYearSettingsCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Description).NotEmpty().MaximumLength(100);
        this.RuleFor(c => c.HasFutureEntryLock).NotNull();
        this.RuleFor(c => c.PastMonthLockDay).GreaterThanOrEqualTo(1).LessThanOrEqualTo(28);
        this.RuleFor(c => c.IsForAllClasses).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                int schoolYear = c.SchoolYear!.Value;
                int instId = c.InstId!.Value;

                var schoolYearSettingsQueryRepository =
                    context.GetServiceProvider()
                    .GetRequiredService<ISchoolYearSettingsQueryRepository>();

                var existsSchoolYearSettingsDefault = await schoolYearSettingsQueryRepository.ExistsSchoolYearSettingsDefaultAsync(c.SchoolYear.Value, ct);
                if (!existsSchoolYearSettingsDefault)
                {
                    context.AddUserFailure($"Не може да се създаде настройки на учебната година, защото не е издадена \"Заповед за определяне на графика на учебното време за учебната {c.SchoolYear.Value} – {c.SchoolYear.Value + 1} година\"");
                    return;
                }

                var defaultSettings = await schoolYearSettingsQueryRepository.GetDefaultAsync(schoolYear, ct);
                var isSportSchool = await schoolYearSettingsQueryRepository.IsSportSchoolAsync(schoolYear, instId, ct);
                var isCplr = await schoolYearSettingsQueryRepository.IsCplrAsync(schoolYear, instId, ct);

                DateTime startDateLimit;
                DateTime schoolYearStartDate;
                DateTime firstTermEndDate;
                DateTime secondTermStartDate;
                DateTime schoolYearEndDate;
                DateTime endDateLimit;
                if (isSportSchool)
                {
                    startDateLimit = defaultSettings.SportSchoolYearStartDateLimit;
                    schoolYearStartDate = defaultSettings.SportSchoolYearStartDate;
                    firstTermEndDate = defaultSettings.SportFirstTermEndDate;
                    secondTermStartDate = defaultSettings.SportSecondTermStartDate;
                    schoolYearEndDate = defaultSettings.SportSchoolYearEndDate;
                    endDateLimit = defaultSettings.SportSchoolYearEndDateLimit;
                }
                else if (isCplr)
                {
                    startDateLimit = defaultSettings.CplrSchoolYearStartDateLimit;
                    schoolYearStartDate = defaultSettings.CplrSchoolYearStartDate;
                    firstTermEndDate = defaultSettings.CplrFirstTermEndDate;
                    secondTermStartDate = defaultSettings.CplrSecondTermStartDate;
                    schoolYearEndDate = defaultSettings.CplrSchoolYearEndDate;
                    endDateLimit = defaultSettings.CplrSchoolYearEndDateLimit;
                }
                else
                {
                    // TODO: this could potentially be a problem for PGs
                    // though at the moment they have the same limits/start-end 15.09 - 14.08
                    // and the term dates are irrelevant for them
                    startDateLimit = defaultSettings.OtherSchoolYearStartDateLimit;
                    schoolYearStartDate = defaultSettings.OtherSchoolYearStartDate;
                    firstTermEndDate = defaultSettings.OtherFirstTermEndDate;
                    secondTermStartDate = defaultSettings.OtherSecondTermStartDate;
                    schoolYearEndDate = defaultSettings.OtherSchoolYearEndDate;
                    endDateLimit = defaultSettings.OtherSchoolYearEndDateLimit;
                }

                if (c is not UpdateSchoolYearSettingsCommand
                    && c.IsForAllClasses == true
                    && await schoolYearSettingsQueryRepository.ExistsIsForAllClassesAsync(schoolYear, instId, null, ct))
                {
                    context.AddUserFailure("Не може да създадете още едни настройки на учебната година за всички класове. Редактирайте съществуващате настройки за всички класове.");
                }

                if ((c.SchoolYearStartDate ?? schoolYearStartDate) > (c.FirstTermEndDate ?? firstTermEndDate))
                {
                    context.AddUserFailure("Началото на учебната година не може да е след края на първи срок.");
                }
                if ((c.FirstTermEndDate ?? firstTermEndDate) > (c.SecondTermStartDate ?? secondTermStartDate))
                {
                    context.AddUserFailure("Края на първи срок не може да е след началото на втори срок.");
                }
                if ((c.SecondTermStartDate ?? secondTermStartDate) > (c.SchoolYearEndDate ?? schoolYearEndDate))
                {
                    context.AddUserFailure("Началото на втори срок не може да е след края на учебната година.");
                }

                if (c.SchoolYearStartDate != null && c.SchoolYearStartDate < startDateLimit)
                {
                    context.AddUserFailure($"Началото на учебната година не може да е преди {startDateLimit:dd.MM.yyyy}.");
                }
                if (c.SchoolYearEndDate != null && c.SchoolYearEndDate > endDateLimit)
                {
                    context.AddUserFailure($"Края на учебната година не може да е след {endDateLimit:dd.MM.yyyy}.");
                }
            });
    }
}
