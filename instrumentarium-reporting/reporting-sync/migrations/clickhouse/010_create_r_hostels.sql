-- Migration: Create R_hostels table
-- Based on inst_basic.R_hostels view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_hostels (
    ClassId Int32,
    InstitutionID Int32,
    InstitutionName String,
    ClassName String,
    StudentsCount Int32
) ENGINE = MergeTree()
ORDER BY (InstitutionID, ClassId);
