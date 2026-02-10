-- Migration: Create R_Regular_Average_Absence_Per_Class table
-- Based on reporting.R_Regular_Average_Absence_Per_Class view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Regular_Average_Absence_Per_Class (
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
    ClassBookId Int32,
    ClassOrGroupName Nullable(String),
    TotalUnexcusedAbsences Nullable(Decimal(38,1)),
    ExcusedByMedicalNoticeAbsence Nullable(Int32),
    ExcusedByHealthReasonAbsence Nullable(Int32),
    ExcusedByFamilyReasonAbsence Nullable(Int32),
    ExcusedByOtherReasonAbsence Nullable(Int32),
    TotalExcusedAbsences Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (RegionID, SchoolYear, InstitutionID);
