namespace SB.Domain;

using FluentValidation;

public class CreateTopicDplrCommandValidator : AbstractValidator<CreateTopicDplrCommand>
{
    public CreateTopicDplrCommandValidator()
    {
        this.RuleFor(c => c.InstId).NotNull();
        this.RuleFor(c => c.SchoolYear).NotNull();
        this.RuleFor(c => c.ClassBookId).NotNull();
        this.RuleFor(c => c.SysUserId).NotNull();
        this.RuleFor(c => c.Date).NotNull();
        this.RuleFor(c => c.Day).NotNull();
        this.RuleFor(c => c.HourNumber).NotNull();
        this.RuleFor(c => c.StartTime).NotNull();
        this.RuleFor(c => c.EndTime).NotNull();
        this.RuleFor(c => c.CurriculumId).NotNull();
        this.RuleFor(s => s.Title).NotEmpty().MaximumLength(1000);
    }
}
