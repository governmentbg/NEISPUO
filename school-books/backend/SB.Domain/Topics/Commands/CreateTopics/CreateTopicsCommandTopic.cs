namespace SB.Domain;

using System;

public record CreateTopicsCommandTopic
{
    public string? Title { get; init; }
    public int[]? ClassBookTopicPlanItemIds { get; init; }
    public DateTime? Date { get; init; }
    public int? ScheduleLessonId { get; init; }
    public int? TeacherAbsenceId { get; init; }
}
