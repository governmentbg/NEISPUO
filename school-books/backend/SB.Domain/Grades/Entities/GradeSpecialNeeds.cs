namespace SB.Domain;

using System;
using SB.Common;

public class GradeSpecialNeeds : Grade
{
    // EF constructor
    public GradeSpecialNeeds()
    {
    }

    public GradeSpecialNeeds(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        SpecialNeedsGrade grade,
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
        this.SpecialGrade = grade;
    }

    public SpecialNeedsGrade SpecialGrade { get; private set; }

    public override string GradeText => this.SpecialGrade.GetEnumDescription();
}
