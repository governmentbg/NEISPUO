namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateGradeChangeExamsAdmProtocolStudentCommandValidator : AbstractValidator<CreateGradeChangeExamsAdmProtocolStudentCommand>
{
    public CreateGradeChangeExamsAdmProtocolStudentCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleSet("Common", () =>
        {
            this.RuleFor(c => c.SchoolYear).NotNull();
            this.RuleFor(c => c.InstId).NotNull();
            this.RuleFor(c => c.SysUserId).NotNull();
            this.RuleFor(c => c.GradeChangeExamsAdmProtocolId).NotNull();

            this.RuleFor(c => c.ClassId).NotNull();
            this.RuleFor(c => c.PersonId).NotNull();

            this.RuleFor(c => c.Subjects).NotEmpty();
        });

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var gradeChangeExamsAdmProtocolQueryRepository = context.GetServiceProvider().GetRequiredService<IGradeChangeExamsAdmProtocolQueryRepository>();

                var isDuplicated = await gradeChangeExamsAdmProtocolQueryRepository.IsStudentDuplicatedAsync(
                    c.SchoolYear!.Value,
                    c.GradeChangeExamsAdmProtocolId!.Value,
                    c.ClassId!.Value,
                    c.PersonId!.Value,
                    ct);

                if (isDuplicated)
                {
                    context.AddUserFailure("Ученик не може да бъде добавян втори път към протокола");
                }
            });
    }
}
