namespace SB.Domain;

using System;
using SB.Common;

public class GradeQualitative : Grade
{
    // EF constructor
    public GradeQualitative()
    {
    }

    public GradeQualitative(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        QualitativeGrade grade,
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
        this.QualitativeGrade = grade;
    }

    public QualitativeGrade QualitativeGrade { get; private set; }

    public override string GradeText => this.QualitativeGrade.GetEnumDescription();
}
