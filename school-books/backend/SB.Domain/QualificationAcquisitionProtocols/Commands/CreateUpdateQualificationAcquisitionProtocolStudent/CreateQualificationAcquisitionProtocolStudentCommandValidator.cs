namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateQualificationAcquisitionProtocolStudentCommandValidator : AbstractValidator<CreateQualificationAcquisitionProtocolStudentCommand>
{
    public CreateQualificationAcquisitionProtocolStudentCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleSet("Common", () =>
        {
            this.RuleFor(c => c.SchoolYear).NotNull();
            this.RuleFor(c => c.InstId).NotNull();
            this.RuleFor(c => c.SysUserId).NotNull();
            this.RuleFor(c => c.QualificationAcquisitionProtocolId).NotNull();

            this.RuleFor(c => c.ClassId).NotNull();
            this.RuleFor(c => c.PersonId).NotNull();
            this.RuleFor(c => c.ExamsPassed).NotNull();
        });

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var qualificationAcquisitionProtocolsQueryRepository = context.GetServiceProvider().GetRequiredService<IQualificationAcquisitionProtocolsQueryRepository>();

                var isDuplicated = await qualificationAcquisitionProtocolsQueryRepository.IsStudentDuplicatedAsync(
                    c.SchoolYear!.Value,
                    c.QualificationAcquisitionProtocolId!.Value,
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
