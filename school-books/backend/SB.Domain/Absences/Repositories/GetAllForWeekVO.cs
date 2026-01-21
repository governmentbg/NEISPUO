namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IAbsencesQueryRepository
{
    public record GetAllForWeekVO(
        int AbsenceId,
        int PersonId,
        AbsenceType Type,
        DateTime Date,
        int ScheduleLessonId,
        DateTime CreateDate,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasUndoAccess { get; set; } // should be mutable
    }
}
