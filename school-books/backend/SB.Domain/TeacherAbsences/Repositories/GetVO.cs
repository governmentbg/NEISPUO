namespace SB.Domain;

using System;
using Microsoft.EntityFrameworkCore;

public partial interface ITeacherAbsencesQueryRepository
{
    public record GetVO(
        int? TeacherPersonId,
        DateTime? StartDate,
        DateTime? EndDate,
        string Reason,
        GetVOHour[] Hours);

    [Keyless]
    public record GetVOHour(
        int ScheduleLessonId,
        int? ReplTeacherPersonId,
        string? ReplTeacherFirstName,
        string? ReplTeacherLastName,
        bool? ReplTeacherIsNonSpecialist,
        string? ExtReplTeacherName,
        bool? IsInUse);
}
