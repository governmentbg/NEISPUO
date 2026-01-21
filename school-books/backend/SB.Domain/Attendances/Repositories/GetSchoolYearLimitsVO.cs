namespace SB.Domain;

using System;

public partial interface IAttendancesQueryRepository
{
    public record GetSchoolYearLimitsVO(
        DateTime SchoolYearStartDateLimit,
        DateTime SchoolYearEndDateLimit);
}
