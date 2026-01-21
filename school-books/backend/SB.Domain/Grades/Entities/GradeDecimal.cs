namespace SB.Domain;

using System;

public class GradeDecimal : Grade
{
    // EF constructor
    public GradeDecimal()
    {
    }

    public GradeDecimal(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        int? subjectTypeId,
        decimal grade,
        GradeType type,
        SchoolTerm term,
        DateTime date,
        int? scheduleLessonId,
        int? teacherAbsenceId,
        string? comment,
        int createdBySysUserId)
        : base(
            schoolYear,
            classBookId,
            personId,
            curriculumId,
            type,
            term,
            date,
            scheduleLessonId,
            teacherAbsenceId,
            comment,
            createdBySysUserId)
    {
        var disallowFractionalGrade =
            type == GradeType.Term ||
            type == GradeType.OtherClassTerm ||
            type == GradeType.OtherSchoolTerm ||
            (type == GradeType.Final && !Grade.SubjectTypeIsProfilingSubject(subjectTypeId));

        if (!disallowFractionalGrade && !(grade == 2 || (grade >= 3 && grade <= 6)))
        {
            throw new DomainValidationException($"{grade} is not a valid decimal grade");
        }

        if (disallowFractionalGrade && !(grade == 2 || grade == 3 || grade == 4 || grade == 5 || grade == 6))
        {
            throw new DomainValidationException(
                new [] { $"{grade} is not a valid decimal grade for type {type} and subjectTypeId {subjectTypeId}" },
                new [] { $"{grade} не е валидна стойност за този вид оценка и предмет" });
        }

        this.DecimalGrade = grade;
    }

    public decimal DecimalGrade { get; private set; }

    public override string GradeText
    {
        get
        {
            return GradeUtils.GetDecimalGradeText(this.DecimalGrade);
        }
    }
}
