-- Migration: Create R_Enrollments_Workflows_Active_Year table
-- Based on azure_temp.AzureEnrollmentsView from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Enrollments_Workflows_Active_Year (
    rowID Int32,
    workflowType Int32,
    userAzureID Nullable(String),
    classAzureID Nullable(String),
    organizationAzureID Nullable(String),
    inProcessing Nullable(Int32),
    errorMessage Nullable(String),
    createdOn DateTime64(7, 'UTC'),
    updatedOn DateTime64(7, 'UTC'),
    guid Nullable(String),
    retryAttempts Nullable(Int32),
    status Int32,
    inProgressResultCount Nullable(Int32),
    userPersonID Nullable(Int32),
    organizationPersonID Nullable(Int32),
    curriculumID Nullable(Int32),
    userRole Nullable(String),
    isForArchivation Int32
) ENGINE = MergeTree()
ORDER BY (rowID);
