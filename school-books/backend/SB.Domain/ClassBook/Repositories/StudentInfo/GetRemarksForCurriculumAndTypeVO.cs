namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetRemarksForCurriculumAndTypeVO(
        [property: JsonIgnore] int RemarkId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        DateTime Date,
        RemarkType Type,
        string TypeName,
        string Description,
        [property: JsonIgnore] bool IsReadFromParent);
}
