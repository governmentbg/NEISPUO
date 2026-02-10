-- Migration: Create RefugeeWithdrawnRequests table
-- Based on refugee.RefugeeWithdrawnRequests view from the MS SQL server
CREATE TABLE IF NOT EXISTS RefugeeWithdrawnRequests (
    SchoolYear Nullable(Int32),
    TownName Nullable(String),
    MunicipalityName Nullable(String),
    RegionName Nullable(String),
    ApplicationNumber Int32,
    CreateDate Nullable(String),
    PersonalIDTypeName String,
    PersonalIDType Int32,
    FirstName String,
    MiddleName String,
    LastName String,
    ClassOrGroup String,
    CancellationDate Nullable(String),
    CancellationReason Nullable(String),
    TownID Nullable(Int32),
    MunicipalityID Nullable(Int32),
    RegionID Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (ApplicationNumber);
