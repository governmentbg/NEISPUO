namespace SB.Domain;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

public class CreateAttendancesCommandValidator : AbstractValidator<CreateAttendancesCommand>
{
    public CreateAttendancesCommandValidator(IValidator<CreateAttendancesCommandAttendance> attendanceValidator)
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Date).NotNull();

        this.RuleFor(c => c.Attendances).NotEmpty();
        this.RuleForEach(c => c.Attendances).SetValidator(attendanceValidator);

        this.RuleFor(c => c.ExcusedReasonComment).MaximumLength(1000);

        this.RuleFor(c => c)
            .CustomAsync(async (c, context, ct) =>
            {
                if (c.Attendances == null || c.Date == null)
                {
                    return;
                }

                var attendancesQueryRepository = context.GetServiceProvider().GetRequiredService<IAttendancesQueryRepository>();

                var schoolYearLimits = await attendancesQueryRepository
                    .GetSchoolYearLimitsAsync(c.SchoolYear!.Value, c.ClassBookId!.Value, ct);

                if (c.Date.Value < schoolYearLimits.SchoolYearStartDateLimit ||
                    c.Date.Value > schoolYearLimits.SchoolYearEndDateLimit)
                {
                    context.AddUserFailure("Не можете да създавате присъствия извън учебната година.");
                }
            });
    }
}
