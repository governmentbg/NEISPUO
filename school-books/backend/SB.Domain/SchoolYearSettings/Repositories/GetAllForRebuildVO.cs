namespace SB.Domain;

using System;

public partial interface ISchoolYearSettingsQueryRepository
{
    public record GetAllForRebuildVO(
        int SchoolYearSettingsId,
        DateTime? SchoolYearStartDate,
        DateTime? FirstTermEndDate,
        DateTime? SecondTermStartDate,
        DateTime? SchoolYearEndDate,
        bool HasFutureEntryLock,
        int? PastMonthLockDay,
        bool IsForAllClasses,
        int[] BasicClassIds,
        int[] ClassBookIds);
}
