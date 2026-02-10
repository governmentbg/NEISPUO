-- Migration: Create R_Average_Grades_Per_Student table
-- Based on reporting.R_Average_Grades_Per_Student view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Average_Grades_Per_Student (
    RegionID Int32,
    RegionName String,
    MunicipalityName String,
    TownName String,
    SchoolYear Int16,
    InstitutionID Int32,
    InstitutionName String,
    DetailedSchoolTypeID Int32,
    BudgetingSchoolTypeID Int32,
    FinancialSchoolTypeID Int32,
    PersonID Int32,
    FirstName String,
    MiddleName Nullable(String),
    LastName String,
    ClassBookId Int32,
    ClassOrGroupName Nullable(String),
    AverageTerm1 Nullable(Decimal(38,6)),
    AverageTerm2 Nullable(Decimal(38,6)),
    AverageYear Nullable(Decimal(38,6))
) ENGINE = MergeTree()
ORDER BY (RegionID, SchoolYear, InstitutionID, FirstName, LastName, PersonID);
