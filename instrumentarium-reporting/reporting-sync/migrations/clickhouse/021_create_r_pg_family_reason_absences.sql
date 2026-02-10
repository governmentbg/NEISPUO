-- Migration: Create R_PG_Family_Reason_Absences table
-- Based on reporting.R_PG_Family_Reason_Absences view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_PG_Family_Reason_Absences (
    RegionID Int32,
    RegionName String,
    MunicipalityName String,
    TownName String,
    InstitutionID Int32,
    InstitutionName String,
    DetailedSchoolTypeID Int32,
    BudgetingSchoolTypeID Int32,
    FinancialSchoolTypeID Int32,
    SchoolYear Int16,
    ClassOrGroupName Nullable(String),
    Day Nullable(DateTime64(3, 'UTC')),
    Month Nullable(Int32),
    AbsencesPerDay Nullable(Int32),
    AbsencesPerMonth Nullable(Int32),
    AbsencesPerYear Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (RegionID, SchoolYear, InstitutionID);
