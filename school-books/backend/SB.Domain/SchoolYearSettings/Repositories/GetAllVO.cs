namespace SB.Domain;

using System;

public partial interface ISchoolYearSettingsQueryRepository
{
    public record GetAllVO(
        int SchoolYearSettingsId,
        DateTime? SchoolYearStartDate,
        DateTime? FirstTermEndDate,
        DateTime? SecondTermStartDate,
        DateTime? SchoolYearEndDate,
        string Description,
        bool IsForAllClasses,
        string[] BasicClassNames,
        string[] ClassBookNames);
}
