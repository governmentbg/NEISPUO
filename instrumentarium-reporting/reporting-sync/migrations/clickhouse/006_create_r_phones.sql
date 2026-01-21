-- Migration: Create Phones table
-- Based on inst_basic.Phones view from the MS SQL server
CREATE TABLE IF NOT EXISTS Phones (
    InstitutionID Int32,
    InstitutionName String,
    InstitutionAbbreviation String,
    CountryName Nullable(String),
    RegionID Int32,
    RegionName String,
    MunicipalityID Int32,
    MunicipalityName String,
    TownID Nullable(Int32),
    TownName String,
    LocalAreaID Nullable(Int32),
    LocalAreaName Nullable(String),
    Email Nullable(String),
    BudgetingSchoolTypeID Int32,
    BaseSchoolTypeName String,
    DetailedSchoolTypeName String,
    FinancialSchoolTypeName String,
    BudgetingInstitutionName String,
    PhoneType Nullable(String),
    PhoneCode Nullable(String),
    PhoneNumber Nullable(String),
    ContactKind Nullable(String),
    IsMain String
) ENGINE = MergeTree()
ORDER BY (RegionID, MunicipalityID, InstitutionID); 