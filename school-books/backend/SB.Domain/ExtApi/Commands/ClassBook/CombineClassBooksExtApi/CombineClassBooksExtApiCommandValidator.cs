namespace SB.Domain;

using System.Linq;
using Common;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CombineClassBooksExtApiCommandValidator : AbstractValidator<CombineClassBooksExtApiCommand>
{
    public CombineClassBooksExtApiCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(s => s.ParentClassId).NotNull();
        this.RuleFor(c => c.ParentClassBookName).MaximumLength(300);
        this.RuleFor(c => c.ChildClassIdForDataTransfer).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var classGroupsQueryRepository = context.GetServiceProvider().GetRequiredService<IClassGroupsQueryRepository>();

                var schoolYear = c.SchoolYear!.Value;
                var instId = c.InstId!.Value;
                var parentClassId = c.ParentClassId!.Value;
                var childClassId = c.ChildClassIdForDataTransfer!.Value;

                var invalidClassGroups = await classGroupsQueryRepository.GetInvalidClassGroupsForClassBookCreationAsync(
                    schoolYear,
                    instId,
                    new int[] { parentClassId },
                    ct);

                var isValidFalseClassGroup = invalidClassGroups.Any(cg => cg.ClassId == parentClassId && cg.IsValidFalseClassGroup);
                if (isValidFalseClassGroup)
                {
                    context.AddUserFailure($"ClassGroup с classId '{parentClassId}' е IsValid=0.");
                }

                var classBookTypeError = invalidClassGroups
                    .Where(cg => cg.ClassId == parentClassId && cg.ClassBookTypeError != null)
                    .Select(cg => cg.ClassBookTypeError)
                    .FirstOrDefault();
                if (classBookTypeError != null)
                {
                    context.AddUserFailure($"Не може да се създаде дневник за classId '{parentClassId}', защото {classBookTypeError!.Value.GetEnumDescription()}.");
                }

                var duplicatedClassBook = invalidClassGroups.Any(cg => cg.ClassId == parentClassId && cg.DuplicateClassBook);
                if (duplicatedClassBook)
                {
                    context.AddUserFailure($"Вече има създаден дневник за classId '{parentClassId}'");
                }

                var nonexistentClassGroup = invalidClassGroups.Any(cg => cg.ClassId == parentClassId && cg.IsNonexistentClassGroup);
                if (nonexistentClassGroup)
                {
                    context.AddUnexpectedFailure($"ClassGroup with classId '{parentClassId}' does not exist");
                }

                var childClassGroup =
                    (await classGroupsQueryRepository.GetClassGroupsWithClassBookAsync(
                    c.SchoolYear!.Value,
                    c.InstId!.Value,
                    new[] { childClassId },
                    ct))
                .First();

                if (childClassGroup.ClassBook == null)
                {
                    context.AddUnexpectedFailure($"ClassBook with classId {childClassId} does not exist");
                }

                if (childClassGroup.ClassGroup.ParentClassId != parentClassId)
                {
                    context.AddUnexpectedFailure($"ClassGroup with class id {childClassId} is not child of classGroup with class id {parentClassId}");
                }
            });
    }
}
