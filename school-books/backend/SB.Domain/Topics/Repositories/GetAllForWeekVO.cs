namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface ITopicsQueryRepository
{
    public record GetAllForWeekVO(
        int TopicId,
        string[] Titles,
        GetAllForWeekVOTeacher[] Teachers,
        GetAllForWeekVOStudent[] Students,
        DateTime Date,
        int ScheduleLessonId,
        DateTime CreateDate,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasUndoAccess { get; set; } // should be mutable
        public bool HasRemoveAccess { get; set; } // should be mutable
    }

    public record GetAllForWeekVOTeacher(
        int TeacherPersonId,
        string TeacherFirstName,
        string TeacherLastName,
        bool IsReplTeacher);

    public record GetAllForWeekVOStudent(
        int StudentPersonId,
        string StudentFirstName,
        string StudentLastName);
}
