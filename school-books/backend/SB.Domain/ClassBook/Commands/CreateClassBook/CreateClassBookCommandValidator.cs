namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class CreateClassBookCommandValidator : AbstractValidator<CreateClassBookCommand>
{
    public CreateClassBookCommandValidator(IValidator<CreateClassBookCommandClassBook> classBookValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleForEach(c => c.ClassBooks).NotEmpty().SetValidator(classBookValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var schoolYearSettingsQueryRepository = context.GetServiceProvider().GetRequiredService<ISchoolYearSettingsQueryRepository>();
                var existsSchoolYearSettingsDefault = await schoolYearSettingsQueryRepository.ExistsSchoolYearSettingsDefaultAsync(c.SchoolYear!.Value, ct);
                if (!existsSchoolYearSettingsDefault)
                {
                    context.AddUserFailure($"Не може да се създаде дневник, защото не е издадена \"Заповед за определяне на графика на учебното време за учебната {c.SchoolYear.Value} – {c.SchoolYear.Value + 1} година\" от МОН");
                    return;
                }

                var classGroupsQueryRepository = context.GetServiceProvider().GetRequiredService<IClassGroupsQueryRepository>();

                var invalidClassGroups = await classGroupsQueryRepository.GetInvalidClassGroupsForClassBookCreationAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    c.ClassBooks!.Select(c => c.ClassId!.Value).ToArray(),
                    ct);

                foreach (var invalidClassGroup in invalidClassGroups.Where(cg => cg.IsValidFalseClassGroup))
                {
                    context.AddUserFailure($"Не може да се създаде дневник за '{invalidClassGroup.ClassName}', защото групата е маркирана като изтрита в образеца.");
                }

                foreach (var invalidClassGroup in invalidClassGroups.Where(cg => cg.ClassBookTypeError != null))
                {
                    context.AddUserFailure($"Не може да се създаде дневник за '{invalidClassGroup.ClassName}', защото {invalidClassGroup.ClassBookTypeError!.Value.GetEnumDescription()}.");
                }

                var duplicatedClassBookClassNames = invalidClassGroups.Where(cg => cg.DuplicateClassBook).Select(cg => cg.ClassName);
                if (duplicatedClassBookClassNames.Any())
                {
                    context.AddUserFailure($"Вече има създаден дневник за '{string.Join(", ", duplicatedClassBookClassNames)}'");
                }

                var nonexistentClassGroupClassIds = invalidClassGroups.Where(cg => cg.IsNonexistentClassGroup).Select(cg => cg.ClassId);
                if (nonexistentClassGroupClassIds.Any())
                {
                    context.AddUnexpectedFailure($"ClassGroup with classId '{string.Join(", ", nonexistentClassGroupClassIds)}' does not exist");
                }

                var hasClassBookOnParentOrChildLevelClassIds = invalidClassGroups.Where(cg => cg.HasClassBookOnParentOrChildLevel).Select(cg => cg.ClassId);
                if (hasClassBookOnParentOrChildLevelClassIds.Any())
                {
                    context.AddUnexpectedFailure($"A ClassBook for classId '{string.Join(", ", hasClassBookOnParentOrChildLevelClassIds)}' already exists on a parent or child level");
                }
            });
    }
}
