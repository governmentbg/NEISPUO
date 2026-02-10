namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

public class CreateHighSchoolCertificateProtocolStudentCommandValidator : AbstractValidator<CreateHighSchoolCertificateProtocolStudentCommand>
{
    public CreateHighSchoolCertificateProtocolStudentCommandValidator()
    {
        this.ClassLevelCascadeMode = CascadeMode.Stop;

        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.HighSchoolCertificateProtocolId).NotNull();

        this.RuleFor(c => c.Students).NotEmpty();

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                var highSchoolCertificateProtocolQueryRepository = context.GetServiceProvider().GetRequiredService<IHighSchoolCertificateProtocolQueryRepository>();

                var hasDuplicatedStudents = await highSchoolCertificateProtocolQueryRepository.HasDuplicatedStudentsAsync(
                    c.SchoolYear!.Value,
                    c.HighSchoolCertificateProtocolId!.Value,
                    c.Students!.Select(s => s.PersonId!.Value).ToArray(),
                    ct);

                if (hasDuplicatedStudents)
                {
                    context.AddUserFailure("Ученик не може да бъде добавян втори път към протокола");
                }
            });
    }
}
