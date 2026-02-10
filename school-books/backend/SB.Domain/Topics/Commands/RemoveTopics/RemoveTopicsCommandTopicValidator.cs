namespace SB.Domain;

using FluentValidation;

public class RemoveTopicsCommandTopicValidator : AbstractValidator<RemoveTopicsCommandTopic>
{
    public RemoveTopicsCommandTopicValidator()
    {
        this.RuleFor(s => s.TopicId).NotNull();
        this.RuleFor(c => c.ScheduleLessonId).NotNull();
    }
}
