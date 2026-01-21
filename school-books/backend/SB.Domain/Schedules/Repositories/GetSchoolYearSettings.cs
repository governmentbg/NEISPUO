namespace SB.Domain;

using System;

public partial interface ISchedulesQueryRepository
{
    public record GetSchoolYearSettings(
        DateTime SchoolYearStartDateLimit,
        DateTime SchoolYearStartDate,
        DateTime FirstTermEndDate,
        DateTime SecondTermStartDate,
        DateTime SchoolYearEndDate,
        DateTime SchoolYearEndDateLimit);
}
