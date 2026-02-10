namespace SB.Domain;

using FluentValidation;

public class UpdateIsVerifiedScheduleLessonCommandScheduleLessonValidator : AbstractValidator<UpdateIsVerifiedScheduleLessonCommandScheduleLesson>
{
    public UpdateIsVerifiedScheduleLessonCommandScheduleLessonValidator()
    {
        this.RuleFor(s => s.ScheduleLessonId).NotNull();
        this.RuleFor(s => s.IsVerified).NotNull();
    }
}
