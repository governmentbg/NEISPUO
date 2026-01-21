-- Migration: Create R_foreign_languages table
-- Based on inst_basic.R_foreign_languages view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_foreign_languages (
    SchoolYear Int16,
    InstitutionID Int32,
    InstitutionName String,
    ClassYear Nullable(Int32),
    ClassName String,
    ClassType String,
    Teacher String,
    CurriculumPartID Nullable(Int32),
    CurriculumPartName Nullable(String),
    SubjectID Nullable(Int32),
    SubjectName Nullable(String),
    CurriculumSubjectTypeID Nullable(Int32),
    SubjectTypeName Nullable(String),
    FLTypeID Nullable(Int32),
    FLStudyTypeName Nullable(String),
    FLID Nullable(Int32),
    FLName Nullable(String),
    StudentsCount Nullable(Int32),
    RegionID Int32,
    RegionName String,
    MunicipalityID Int32,
    MunicipalityName String,
    TownID Int32,
    TownName String,
    LocalAreaName String,
    BaseSchoolTypeName String,
    DetailedSchoolTypeName String,
    FinancialSchoolTypeName String
) ENGINE = MergeTree()
ORDER BY (RegionID, MunicipalityID, InstitutionID, SchoolYear);
