-- Migration: Create R_Classes_Workflows_Active_Year table
-- Based on azure_temp.AzureClassesView from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Classes_Workflows_Active_Year (
    rowID Int32,
    classID Nullable(String),
    workflowType Int32,
    title Nullable(String),
    classCode Nullable(String),
    orgID Nullable(String),
    termID Nullable(Int32),
    termName Nullable(String),
    termStartDate Nullable(DateTime64(7, 'UTC')),
    termEndDate Nullable(DateTime64(7, 'UTC')),
    inProcessing Nullable(Int32),
    errorMessage Nullable(String),
    createdOn DateTime64(7, 'UTC'),
    updatedOn DateTime64(7, 'UTC'),
    guid Nullable(String),
    retryAttempts Nullable(Int32),
    status Int32,
    azureID Nullable(String),
    inProgressResultCount Nullable(Int32),
    isForArchivation Int32
) ENGINE = MergeTree()
ORDER BY (rowID);
