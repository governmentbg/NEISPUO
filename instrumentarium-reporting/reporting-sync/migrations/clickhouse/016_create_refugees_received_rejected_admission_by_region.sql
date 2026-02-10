-- Migration: Create RefugeesReceivedRejectedAdmissionByRegion table
-- Based on refugee.RefugeesReceivedRejectedAdmissionByRegion view from the MS SQL server
CREATE TABLE IF NOT EXISTS RefugeesReceivedRejectedAdmissionByRegion (
    SchoolYear Nullable(Int32),
    RegionName Nullable(String),
    ChildNumber Nullable(Int32),
    Rejected Nullable(Int32),
    KindergartenLastInstitution Nullable(Int32),
    SchoolLastInstitution Nullable(Int32),
    ClassOrGroup String,
    StudentsWithEGN Nullable(Int32),
    StudentsWithLNCH Nullable(Int32),
    HasDocumentForCompletedClass Nullable(Int32),
    HasImmunizationStatusDocumentSum Nullable(Int32),
    KGEnrolledWithEGN Nullable(Int32),
    KGEnrolledWithLNCH Nullable(Int32),
    SEnrolledWithEGN Nullable(Int32),
    SEnrolledWithLNCH Nullable(Int32),
    RegionID Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (ClassOrGroup);
