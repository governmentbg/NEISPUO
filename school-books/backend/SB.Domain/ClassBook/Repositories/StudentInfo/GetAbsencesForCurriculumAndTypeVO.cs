namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetAbsencesForCurriculumAndTypeVO(
        [property: JsonIgnore] int AbsenceId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        AbsenceType Type,
        string TypeName,
        int? ExcusedReasonId,
        string? ExcusedReasonName,
        string? ExcusedReasonComment,
        bool? ReplTeacherIsNonSpecialist,
        [property: JsonIgnore] bool IsReadFromParent);
}
