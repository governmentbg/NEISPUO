namespace SB.Domain;

using FluentValidation;

public class UpdateLectureScheduleCommandValidator : AbstractValidator<UpdateLectureScheduleCommand>
{
    public UpdateLectureScheduleCommandValidator(IValidator<CreateLectureScheduleCommand> createValidator)
    {
        this.RuleFor(c => (CreateLectureScheduleCommand)c).SetValidator(createValidator);
        this.RuleFor(s => s.LectureScheduleId).NotNull();
    }
}
