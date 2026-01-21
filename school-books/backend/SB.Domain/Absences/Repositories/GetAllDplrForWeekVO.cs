namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public partial interface IAbsencesDplrQueryRepository
{
    public record GetAllDplrForWeekVO(
        int AbsenceId,
        int PersonId,
        DateTime Date,
        int ScheduleLessonId,
        DateTime CreateDate,
        [property: JsonIgnore] int CreatedBySysUserId)
    {
        public bool HasUndoAccess { get; set; } // should be mutable
    }
}
