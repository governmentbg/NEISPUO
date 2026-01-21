-- Migration: Create R_Organizations_Workflows_Active_Year table
-- Based on azure_temp.AzureOrganizationsView from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Organizations_Workflows_Active_Year (
    rowID Int32,
    organizationID Nullable(String),
    workflowType Int32,
    name Nullable(String),
    description Nullable(String),
    principalId Nullable(String),
    principalName Nullable(String),
    principalEmail Nullable(String),
    highestGrade Nullable(Int32),
    lowestGrade Nullable(Int32),
    phone Nullable(String),
    city Nullable(String),
    area Nullable(String),
    country Nullable(String),
    postalCode Nullable(String),
    street Nullable(String),
    inProcessing Nullable(Int32),
    errorMessage Nullable(String),
    createdOn DateTime64(7, 'UTC'),
    updatedOn DateTime64(7, 'UTC'),
    guid Nullable(String),
    retryAttempts Nullable(Int32),
    status Int32,
    username Nullable(String),
    password Nullable(String),
    personID Nullable(Int32),
    isForArchivation Int32,
    azureID Nullable(String),
    inProgressResultCount Nullable(Int32)
) ENGINE = MergeTree()
ORDER BY (rowID);
