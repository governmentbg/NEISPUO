-- Migration: Create R_Graduated_Students table
-- Based on reporting.R_Graduated_Students view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Graduated_Students (
    Year Int16,
    Region String,
    AllDiplomasSUM Int32,
    SecondarySchools Int32,
    SportSchools Int32,
    SeminarySchools Int32,
    ProfiledSchools Int32,
    ProfessionalSchools Int32,
    SpecialSchools Int32,
    ArtSchools Int32
) ENGINE = MergeTree()
ORDER BY (Year, Region); 