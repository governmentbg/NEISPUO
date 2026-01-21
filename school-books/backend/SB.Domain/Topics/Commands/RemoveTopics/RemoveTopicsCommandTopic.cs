namespace SB.Domain;
public record RemoveTopicsCommandTopic(
    int? TopicId,
    int? ScheduleLessonId);
