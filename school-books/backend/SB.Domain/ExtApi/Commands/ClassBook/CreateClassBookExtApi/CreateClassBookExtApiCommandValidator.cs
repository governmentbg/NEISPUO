namespace SB.Domain;

using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

public class CreateClassBookExtApiCommandValidator : AbstractValidator<CreateClassBookExtApiCommand>
{
    public CreateClassBookExtApiCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();

        this.RuleFor(c => c.ClassId).NotNull();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var classGroupsQueryRepository = context.GetServiceProvider().GetRequiredService<IClassGroupsQueryRepository>();

                int schoolYear = c.SchoolYear!.Value;
                int instId = c.InstId!.Value;
                int classId = c.ClassId!.Value;

                var invalidClassGroups = await classGroupsQueryRepository.GetInvalidClassGroupsForClassBookCreationAsync(
                    schoolYear,
                    instId,
                    new int[] { classId },
                    ct);

                var isValidFalseClassGroup = invalidClassGroups.Where(cg => cg.ClassId == classId && cg.IsValidFalseClassGroup).Any();
                if (isValidFalseClassGroup)
                {
                    context.AddUserFailure($"ClassGroup с classId '{classId}' е IsValid=0.");
                }

                var classBookTypeError = invalidClassGroups
                    .Where(cg => cg.ClassId == classId && cg.ClassBookTypeError != null)
                    .Select(cg => cg.ClassBookTypeError)
                    .FirstOrDefault();
                if (classBookTypeError != null)
                {
                    context.AddUserFailure($"Не може да се създаде дневник за classId '{classId}', защото {classBookTypeError!.Value.GetEnumDescription()}.");
                }

                var duplicatedClassBook = invalidClassGroups.Where(cg => cg.ClassId == classId && cg.DuplicateClassBook).Any();
                if (duplicatedClassBook)
                {
                    context.AddUserFailure($"Вече има създаден дневник за classId '{classId}'");
                }

                var nonexistentClassGroup = invalidClassGroups.Where(cg => cg.ClassId == classId && cg.IsNonexistentClassGroup).Any();
                if (nonexistentClassGroup)
                {
                    context.AddUnexpectedFailure($"ClassGroup with classId '{classId}' does not exist");
                }

                var hasClassBookOnParentOrChildLevel = invalidClassGroups.Where(cg => cg.ClassId == classId && cg.HasClassBookOnParentOrChildLevel).Any();
                if (hasClassBookOnParentOrChildLevel)
                {
                    context.AddUnexpectedFailure($"A ClassBook for classId '{classId}' already exists on a parent or child level");
                }
            });
    }
}
