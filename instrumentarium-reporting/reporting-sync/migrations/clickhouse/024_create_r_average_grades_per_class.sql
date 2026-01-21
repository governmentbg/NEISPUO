-- Migration: Create R_Average_Grades_Per_Class table
-- Based on reporting.R_Average_Grades_Per_Class view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Average_Grades_Per_Class (
    RegionID Int32,
    RegionName String,
    MunicipalityName String,
    TownName String,
    SchoolYear Int16,
    InstitutionID Int32,
    InstitutionName String,
    ClassBookId Int32,
    ClassOrGroupName Nullable(String),
    SubjectName Nullable(String),
    SubjectTypeName String,
    StudentsStudyingSubject Nullable(Int32),
    StudentsWithTerm1Grades Nullable(Int32),
    StudentsWithTerm2Grades Nullable(Int32),
    StudentsWithYearGrades Nullable(Int32),
    PercentStudentsWithTerm1Grades Nullable(Float64),
    PercentStudentsWithTerm2Grades Nullable(Float64),
    PercentStudentsWithYearGrades Nullable(Float64),
    AverageTerm1Grade Nullable(Decimal(38,6)),
    AverageTerm2Grade Nullable(Decimal(38,6)),
    AverageYearGrade Nullable(Decimal(38,6))
) ENGINE = MergeTree()
ORDER BY (RegionID, SchoolYear, InstitutionID);
