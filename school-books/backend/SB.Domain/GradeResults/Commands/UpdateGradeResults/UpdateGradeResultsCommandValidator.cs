namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class UpdateGradeResultsCommandValidator : AbstractValidator<UpdateGradeResultsCommand>
{
    public UpdateGradeResultsCommandValidator(IValidator<UpdateGradeResultsCommandStudent> studentValidator)
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.Students).NotNull();
        this.RuleForEach(c => c.Students).SetValidator(studentValidator);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var gradeResultsQueryRepository = context.GetServiceProvider().GetRequiredService<IGradeResultsQueryRepository>();

                var classGradeResults =
                    c.Students!
                    .Where(s => s.PersonId.HasValue)
                    .Select(s => (personId: s.PersonId!.Value, s.RetakeExamCurriculumIds))
                    .ToArray();

                if (await gradeResultsQueryRepository.HasRemovedFilledSessionAsync(
                    c.SchoolYear!.Value,
                    c.ClassBookId!.Value,
                    classGradeResults,
                    ct))
                {
                    context.AddUserFailure("Не може да се премахне предмет, за когото са попълнени данни в сесия.");
                }
            });
    }
}
