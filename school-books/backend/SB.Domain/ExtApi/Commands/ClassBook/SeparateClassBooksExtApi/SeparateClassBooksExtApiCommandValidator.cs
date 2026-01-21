namespace SB.Domain;

using System.Linq;
using Common;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class SeparateClassBooksExtApiCommandValidator : AbstractValidator<SeparateClassBooksExtApiCommand>
{
    public SeparateClassBooksExtApiCommandValidator(IValidator<SeparateClassBooksExtApiCommandClassBook?> classBookValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.ParentClassId).NotNull();
        this.RuleForEach(c => c.ChildClassBooks).NotEmpty().SetValidator(classBookValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var classGroupsQueryRepository = context.GetServiceProvider().GetRequiredService<IClassGroupsQueryRepository>();

                var schoolYear = c.SchoolYear!.Value;
                var instId = c.InstId!.Value;
                var parentClassId = c.ParentClassId!.Value;
                var childClassIds = c.ChildClassBooks!.Select(cb => cb.ClassId!.Value).ToArray();

                var invalidClassGroups = await classGroupsQueryRepository.GetInvalidClassGroupsForClassBookCreationAsync(
                    schoolYear,
                    instId,
                    childClassIds,
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

                var childClassGroups = await classGroupsQueryRepository.GetClassGroupsWithClassBookAsync(
                    schoolYear,
                    instId,
                    childClassIds,
                    ct);
                var childrenWithInvalidParent = childClassGroups.Where(ccg => ccg.ClassGroup.ParentClassId != parentClassId).ToArray();
                if (childrenWithInvalidParent.Any())
                {
                    var childIds = string.Join(", ", childrenWithInvalidParent.Select(c => c.ClassGroup.ClassId));
                    context.AddUnexpectedFailure($"ClassGroups with class ids {childIds} is not child of classGroup with class id {parentClassId}");
                }

                var parentClassGroup = (await classGroupsQueryRepository.GetClassGroupsWithClassBookAsync(
                    schoolYear,
                    instId,
                    new[] { parentClassId },
                    ct))
                .First();

                if (parentClassGroup.ClassBook == null)
                {
                    context.AddUnexpectedFailure($"ClassBook with id {parentClassId} does not exist");
                }
            });
    }
}
