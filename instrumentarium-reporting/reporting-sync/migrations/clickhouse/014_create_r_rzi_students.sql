-- Migration: Create R_RZI_Students table
-- Based on reporting.R_RZI_Students view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_RZI_Students (
    RegionId Int32,
    RegionName String,
    MunicipalityId Int32,
    MunicipalityName String,
    TownID Int32,
    TownName String,
    InstitutionID Int32,
    InstitutionName String,
    BudgetingSchoolTypeID Int32,
    InstitutionKind Nullable(String),
    GroupsCount Nullable(Int32),
    AllStds Nullable(Int32),
    A1_3male Nullable(Int32),
    A1_3female Nullable(Int32),
    A4_7malePG Nullable(Int32),
    A4_7femalePG Nullable(Int32),
    A7_13maleSch Nullable(Int32),
    A7_13femaleSch Nullable(Int32),
    A14_18male Nullable(Int32),
    A14_18female Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (RegionId, MunicipalityId, InstitutionID);
