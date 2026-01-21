namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetClassBookInfoVO(
        int ClassBookId,
        ClassBookType BookType,
        string BookName,
        int? BasicClassId,
        string? BasicClassName,
        bool IsIndividualCurriculum,
        DateTime SchoolYearStartDateLimit,
        DateTime SchoolYearStartDate,
        DateTime FirstTermEndDate,
        DateTime SecondTermStartDate,
        DateTime SchoolYearEndDate,
        DateTime SchoolYearEndDateLimit,
        string? Profession,
        int AbsenceDaysByFamilyReasonsCount);
}
