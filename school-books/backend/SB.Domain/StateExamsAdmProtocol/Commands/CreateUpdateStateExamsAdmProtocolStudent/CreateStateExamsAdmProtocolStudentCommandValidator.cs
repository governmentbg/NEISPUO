namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateStateExamsAdmProtocolStudentCommandValidator : AbstractValidator<CreateStateExamsAdmProtocolStudentCommand>
{
    public CreateStateExamsAdmProtocolStudentCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleSet("Common", () =>
        {
            this.RuleFor(c => c.SchoolYear).NotNull();
            this.RuleFor(c => c.InstId).NotNull();
            this.RuleFor(c => c.SysUserId).NotNull();
            this.RuleFor(c => c.StateExamsAdmProtocolId).NotNull();

            this.RuleFor(c => c.ClassId).NotNull();
            this.RuleFor(c => c.PersonId).NotNull();
            this.RuleFor(c => c.HasFirstMandatorySubject).NotNull();

            this.RuleFor(c => c.AdditionalSubjects).NotNull();
            this.RuleFor(c => c.QualificationSubjects).NotNull();
        });

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var stateExamsAdmProtocolQueryRepository = context.GetServiceProvider().GetRequiredService<IStateExamsAdmProtocolQueryRepository>();

                var isDuplicated = await stateExamsAdmProtocolQueryRepository.IsStudentDuplicatedAsync(
                    c.SchoolYear!.Value,
                    c.StateExamsAdmProtocolId!.Value,
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
