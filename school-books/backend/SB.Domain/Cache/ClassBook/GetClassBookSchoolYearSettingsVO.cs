namespace SB.Domain;

using System;

public partial interface IClassBookCQSQueryRepository
{
    public record GetClassBookSchoolYearSettingsVO(
        DateTime SchoolYearStartDateLimit,
        DateTime FirstTermEndDate,
        DateTime SecondTermStartDate,
        DateTime SchoolYearEndDateLimit,
        bool HasFutureEntryLock,
        int? PastMonthLockDay);
}
