namespace SB.Domain;

using System;
using System.Linq;

public abstract class Grade : IAggregateRoot
{
    // EF constructor
    protected Grade()
    {
    }

    protected Grade(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        GradeType type,
        SchoolTerm term,
        DateTime date,
        int? scheduleLessonId,
        int? teacherAbsenceId,
        string? comment,
        int createdBySysUserId)
    {
        if (scheduleLessonId == null && GradeTypeRequiresScheduleLesson(type))
        {
            throw new DomainValidationException("scheduleLessonId is required when grade type is not Term or Final.");
        }

        this.SchoolYear = schoolYear;
        this.ClassBookId = classBookId;
        this.PersonId = personId;
        this.CurriculumId = curriculumId;
        this.Date = date;
        this.Type = type;
        this.Term = term;
        this.ScheduleLessonId = GradeTypeRequiresScheduleLesson(type) ? scheduleLessonId : null;
        this.TeacherAbsenceId = teacherAbsenceId;
        this.Comment = comment;
        this.IsReadFromParent = false;

        var now = DateTime.Now;
        this.CreateDate = now;
        this.CreatedBySysUserId = createdBySysUserId;
        this.ModifyDate = now;
        this.ModifiedBySysUserId = createdBySysUserId;
    }

    public int SchoolYear { get; private set; }

    public int GradeId { get; private set; }

    public int ClassBookId { get; private set; }

    public int PersonId { get; private set; }

    public int CurriculumId { get; private set; }

    public DateTime Date { get; private set; }

    public GradeCategory Category { get; private set; }

    public GradeType Type { get; private set; }

    public SchoolTerm Term { get; private set; }

    public int? ScheduleLessonId { get; private set; }

    public int? TeacherAbsenceId { get; private set; }

    public string? Comment { get; private set; }

    public bool IsReadFromParent { get; private set; }

    public DateTime CreateDate { get; private set; }

    public int CreatedBySysUserId { get; private set; }

    public DateTime ModifyDate { get; internal set; }

    public int ModifiedBySysUserId { get; private set; }

    public byte[] Version { get; private set; } = null!;

    public abstract string GradeText { get; }

    // Note! This method have identical version in the frontend in
    // frontend/projects/shared/utils/book.ts
    public static readonly Func<GradeType, bool> GradeTypeRequiresScheduleLesson =
        (gradeType) =>
            gradeType != GradeType.Term &&
            gradeType != GradeType.Final &&
            gradeType != GradeType.OtherClass &&
            gradeType != GradeType.OtherSchool &&
            gradeType != GradeType.OtherClassTerm &&
            gradeType != GradeType.OtherSchoolTerm;

    public static readonly Func<int?, bool> SubjectTypeIsProfilingSubject =
        (subjectTypeId) =>
            SubjectType.ProfilingSubjectIds.Contains(subjectTypeId ?? SubjectType.DefaultSubjectTypeId);

    public string EmailTag
    {
        get
        {
            return $"grade:{this.SchoolYear}:{this.GradeId}";
        }
    }

    public string PushNotificationTag
    {
        get
        {
            return $"grade-push:{this.SchoolYear}:{this.GradeId}";
        }
    }

    public void SetAsReadFromParent()
    {
        this.IsReadFromParent = true;
    }
}
