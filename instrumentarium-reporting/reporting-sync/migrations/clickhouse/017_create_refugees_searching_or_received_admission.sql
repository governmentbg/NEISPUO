-- Migration: Create RefugeesSearchingOrReceivedAdmission table
-- Based on refugee.RefugeesSearchingOrReceivedAdmission view from the MS SQL server
CREATE TABLE IF NOT EXISTS RefugeesSearchingOrReceivedAdmission (
    SchoolYear Nullable(Int32),
    TownName Nullable(String),
    MunicipalityName Nullable(String),
    RegionName Nullable(String),
    InstitutionID Int32,
    InstitutionAbbreviation Nullable(String),
    FinancialSchoolTypeName Nullable(String),
    StudentCount Nullable(Int32),
    TownID Nullable(Int32),
    MunicipalityID Nullable(Int32),
    RegionID Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (InstitutionID);
