namespace SB.Domain;

using System;

public partial interface ISchoolYearSettingsQueryRepository
{
    public record GetVO(
        int SchoolYearSettingsId,
        DateTime? SchoolYearStartDate,
        DateTime? FirstTermEndDate,
        DateTime? SecondTermStartDate,
        DateTime? SchoolYearEndDate,
        string Description,
        bool HasFutureEntryLock,
        int? PastMonthLockDay,
        bool IsForAllClasses,
        int[] BasicClassIds,
        int[] ClassBookIds);
}
