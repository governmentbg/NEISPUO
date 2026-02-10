namespace SB.Domain;

using System;

public partial interface ITeacherAbsencesQueryRepository
{
    public record GetAllVO(
        int TeacherAbsenceId,
        int TeacherPersonId,
        string TeacherName,
        string[] ReplTeacherNames,
        DateTime StartDate,
        DateTime EndDate,
        string Reason);
}
