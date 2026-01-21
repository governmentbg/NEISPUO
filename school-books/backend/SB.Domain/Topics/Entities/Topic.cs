namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;

public class Topic : IAggregateRoot
{
    // EF constructor
    private Topic()
    {
    }

    public Topic(
        int schoolYear,
        int classBookId,
        (string title, int? classBookTopicPlanItemId)[] titles,
        (int personId, bool isReplTeacher)[] teachers,
        DateTime date,
        int scheduleLessonId,
        int? teacherAbsenceId,
        int createdBySysUserId)
    {
        if (titles.Length == 0)
        {
            throw new DomainValidationException("Titles must not be empty.");
        }

        if (teachers.Length == 0)
        {
            throw new DomainValidationException(new[] { "empty_teachers" }, new[] { "Часове без текущи учители не могат да бъдат маркирани като взети." });
        }

        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.Date = date;
        this.ScheduleLessonId = scheduleLessonId;
        this.TeacherAbsenceId = teacherAbsenceId;

        this.titles.AddRange(
            titles.Select((t, i) => new TopicTitle(this, i, t.title, t.classBookTopicPlanItemId)));
        this.teachers.AddRange(
            teachers.Select(t => new TopicTeacher(this, t.personId, t.isReplTeacher)));

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int TopicId { get; private set; }

    public int ClassBookId { get; private set; }

    public DateTime Date { get; private set; }

    public int ScheduleLessonId { get; private set; }

    public int? TeacherAbsenceId { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    // relations
    private readonly List<TopicTitle> titles = new();
    public IReadOnlyCollection<TopicTitle> Titles => this.titles.AsReadOnly();

    private readonly List<TopicTeacher> teachers = new();
    public IReadOnlyCollection<TopicTeacher> Teachers => this.teachers.AsReadOnly();

    public void ClearClassBookTopicPlanItemId(int[] classBookTopicPlanItemIds)
    {
        foreach(var title in this.titles.Where(tt =>
            tt.ClassBookTopicPlanItemId.HasValue &&
            classBookTopicPlanItemIds.Contains(tt.ClassBookTopicPlanItemId.Value)))
        {
            title.ClearClassBookTopicPlanItemId();
        }
    }
}
