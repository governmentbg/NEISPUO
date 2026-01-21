namespace SB.Domain;

using System;

public partial interface ISchoolYearSettingsProvider
{
    public record GetForClassBookVO(
        int? SchoolYearSettingsId,
        DateTime SchoolYearStartDateLimit,
        DateTime SchoolYearStartDate,
        DateTime FirstTermEndDate,
        DateTime SecondTermStartDate,
        DateTime SchoolYearEndDate,
        DateTime SchoolYearEndDateLimit,
        bool HasFutureEntryLock,
        int? PastMonthLockDay);
}
