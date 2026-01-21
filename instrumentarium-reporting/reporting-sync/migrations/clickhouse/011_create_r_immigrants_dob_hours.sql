-- Migration: Create RImmigrantsDOBHours table
-- Based on inst_basic.RImmigrantsDOBHours view from the MS SQL server
CREATE TABLE IF NOT EXISTS RImmigrantsDOBHours (
    SchoolYear Int16,
    InstitutionID Int32,
    InstitutionName String,
    InstitutionAbbreviation String,
    TownID Nullable(Int32),
    TownName String,
    MunicipalityID Int32,
    MunicipalityName String,
    RegionID Int32,
    RegionName String,
    BaseSchoolTypeID Int32,
    BaseSchoolTypeName String,
    DetailedSchoolTypeID Int32,
    DetailedSchoolTypeName String,
    BudgetingSchoolTypeID Int32,
    BudgetingInstitutionName String,
    FinancialSchoolTypeID Int32,
    FinancialSchoolTypeName String,
    Lecturer Nullable(String),
    HoursWeekly Nullable(Float32)
) ENGINE = MergeTree()
ORDER BY (RegionID, MunicipalityID, InstitutionID, SchoolYear);
