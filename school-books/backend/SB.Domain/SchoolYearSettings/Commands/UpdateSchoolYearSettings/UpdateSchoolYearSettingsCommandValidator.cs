namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class UpdateSchoolYearSettingsCommandValidator : AbstractValidator<UpdateSchoolYearSettingsCommand>
{
    public UpdateSchoolYearSettingsCommandValidator(IValidator<CreateSchoolYearSettingsCommand> createValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(s => (CreateSchoolYearSettingsCommand)s).SetValidator(createValidator);
        this.RuleFor(s => s.SchoolYearSettingsId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                int schoolYear = c.SchoolYear!.Value;
                int instId = c.InstId!.Value;
                int schoolYearSettingsId = c.SchoolYearSettingsId!.Value;
                var schoolYearSettingsQueryRepository = context.GetServiceProvider().GetRequiredService<ISchoolYearSettingsQueryRepository>();
                if (c.IsForAllClasses!.Value
                    && await schoolYearSettingsQueryRepository.ExistsIsForAllClassesAsync(schoolYear, instId, schoolYearSettingsId, ct))
                {
                    context.AddUserFailure("Не може да създадете още едни настройки на учебната година за всички класове. Редактирайте съществуващате настройки за всички класове.");
                }
            });
    }
}
