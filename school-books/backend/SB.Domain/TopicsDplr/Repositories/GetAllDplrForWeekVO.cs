namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface ITopicsDplrQueryRepository
{
    public record GetAllDplrForWeekVO(
        int TopicDplrId,
        DateTime Date,
        int Day,
        int HourNumber,
        int CurriculumId,
        string SubjectName,
        string? SubjectNameShort,
        string SubjectTypeName,
        string Title,
        string? Location,
        GetAllDplrForWeekVOTeacher[] Teachers,
        GetAllDplrForWeekVOStudent[] Students,
        DateTime CreateDate,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasUndoAccess { get; set; } // should be mutable
        public bool HasTopicRemoveAccess { get; set; } // should be mutable
        public bool HasTopicCreateAccess { get; set; } // should be mutable
    }

    public record GetAllDplrForWeekVOTeacher(
        int TeacherPersonId,
        string TeacherFirstName,
        string TeacherLastName);

    public record GetAllDplrForWeekVOStudent(
        int StudentPersonId,
        string StudentFirstName,
        string StudentLastName);
}
