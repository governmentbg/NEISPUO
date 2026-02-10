namespace SB.Domain;

using System;

public partial interface ISchoolYearSettingsQueryRepository
{
    public record GetDefaultVO(
        DateTime PgSchoolYearStartDateLimit,
        DateTime PgSchoolYearStartDate,
        DateTime PgFirstTermEndDate,
        DateTime PgSecondTermStartDate,
        DateTime PgSchoolYearEndDate,
        DateTime PgSchoolYearEndDateLimit,
        DateTime SportSchoolYearStartDateLimit,
        DateTime SportSchoolYearStartDate,
        DateTime SportFirstTermEndDate,
        DateTime SportSecondTermStartDate,
        DateTime SportSchoolYearEndDate,
        DateTime SportSchoolYearEndDateLimit,
        DateTime CplrSchoolYearStartDateLimit,
        DateTime CplrSchoolYearStartDate,
        DateTime CplrFirstTermEndDate,
        DateTime CplrSecondTermStartDate,
        DateTime CplrSchoolYearEndDate,
        DateTime CplrSchoolYearEndDateLimit,
        DateTime OtherSchoolYearStartDateLimit,
        DateTime OtherSchoolYearStartDate,
        DateTime OtherFirstTermEndDate,
        DateTime OtherSecondTermStartDate,
        DateTime OtherSchoolYearEndDate,
        DateTime OtherSchoolYearEndDateLimit);
}
