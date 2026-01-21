-- Migration: Create R_PG_Average_Absence_Per_Student table
-- Based on reporting.R_PG_Average_Absence_Per_Student view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_PG_Average_Absence_Per_Student (
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
    TotalUnexcusedAbsences Nullable(Int32),
    ExcusedByMedicalNoticeAbsence Nullable(Int32),
    ExcusedByHealthReasonAbsence Nullable(Int32),
    ExcusedByFamilyReasonAbsence Nullable(Int32),
    ExcusedByOtherReasonAbsence Nullable(Int32),
    TotalExcusedAbsences Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (RegionID, SchoolYear, InstitutionID, PersonID, FirstName, LastName);
