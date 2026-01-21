namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateClassBooksExtApiCommandValidator : AbstractValidator<CreateClassBooksExtApiCommand>
{
    public CreateClassBooksExtApiCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleForEach(c => c.ClassIds).NotEmpty();

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

                int schoolYear = c.SchoolYear!.Value;
                int instId = c.InstId!.Value;
                var classIds = c.ClassIds!.Select(c => c!).ToArray();

                var invalidClassGroups = await classGroupsQueryRepository.GetInvalidClassGroupsForClassBookCreationAsync(
                    schoolYear,
                    instId,
                    classIds,
                    ct);

                foreach (var invalidClassGroup in invalidClassGroups.Where(cg => cg.ClassBookTypeError != null))
                {
                    context.AddUserFailure($"Не може да се създаде дневник за classId '{invalidClassGroup.ClassId}', защото {invalidClassGroup.ClassBookTypeError}.");
                }

                var duplicatedClassBookClassIds = invalidClassGroups.Where(cg => cg.DuplicateClassBook).Select(cg => cg.ClassId);
                if (duplicatedClassBookClassIds.Any())
                {
                    context.AddUserFailure($"Вече има създаден дневник за classId '{string.Join(", ", duplicatedClassBookClassIds)}'");
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
