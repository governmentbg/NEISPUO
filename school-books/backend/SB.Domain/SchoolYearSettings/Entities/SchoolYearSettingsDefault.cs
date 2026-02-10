namespace SB.Domain;

using System;

public class SchoolYearSettingsDefault
{
    // EF constructor
    private SchoolYearSettingsDefault()
    {
    }

    public int SchoolYear { get; private set; }

    public DateTime PgSchoolYearStartDateLimit { get; private set; }

    public DateTime PgSchoolYearStartDate { get; private set; }

    public DateTime PgFirstTermEndDate { get; private set; }

    public DateTime PgSecondTermStartDate { get; private set; }

    public DateTime PgSchoolYearEndDate { get; private set; }

    public DateTime PgSchoolYearEndDateLimit { get; private set; }

    public DateTime SportSchoolYearStartDateLimit { get; private set; }

    public DateTime SportSchoolYearStartDate { get; private set; }

    public DateTime SportFirstTermEndDate { get; private set; }

    public DateTime SportSecondTermStartDate { get; private set; }

    public DateTime SportSchoolYearEndDate { get; private set; }

    public DateTime SportSchoolYearEndDateLimit { get; private set; }

    public DateTime CplrSchoolYearStartDateLimit  { get; private set; }

    public DateTime CplrSchoolYearStartDate  { get; private set; }

    public DateTime CplrFirstTermEndDate  { get; private set; }

    public DateTime CplrSecondTermStartDate  { get; private set; }

    public DateTime CplrSchoolYearEndDate  { get; private set; }

    public DateTime CplrSchoolYearEndDateLimit  { get; private set; }

    public DateTime OtherSchoolYearStartDateLimit { get; private set; }

    public DateTime OtherSchoolYearStartDate { get; private set; }

    public DateTime OtherFirstTermEndDate { get; private set; }

    public DateTime OtherSecondTermStartDate { get; private set; }

    public DateTime OtherSchoolYearEndDate { get; private set; }

    public DateTime OtherSchoolYearEndDateLimit { get; private set; }
}
