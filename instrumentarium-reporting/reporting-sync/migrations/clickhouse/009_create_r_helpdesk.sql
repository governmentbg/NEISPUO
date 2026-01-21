-- Migration: Create R_Helpdesk table
-- Based on reporting.R_Helpdesk view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Helpdesk (
    IssueID Int32,
    IssueTitle String,
    IssueDescription String,
    IssueCategoryId Int32,
    IssueSubcategoryId Nullable(Int32),
    IssuePriorityId Int32,
    IssueStatusId Int32,
    IssueIsEscalated UInt8,
    IssueIsLevel3Support UInt8,
    IssueSchoolYear Nullable(Int16),
    IssueResolveDate Nullable(DateTime64(7)),
    IssueCreateDate DateTime64(7),
    IssueModifyDate Nullable(DateTime64(7)),
    IssuePhone Nullable(String),
    StatusName Nullable(String),
    StatusCode Nullable(String),
    PriorityName Nullable(String),
    PriorityCode Nullable(String),
    CategoryName Nullable(String),
    CategoryCode Nullable(Int32),
    SubCategoryName Nullable(String),
    SubCategoryCode Nullable(Int32),
    IssueSubmitterUsername Nullable(String),
    IssueSubmitterRole Nullable(String),
    IssueAssignedToUsername Nullable(String),
    IssueCreatedByUsername Nullable(String),
    IssueModifiedByUsername Nullable(String),
    InstitutionId Nullable(Int32),
    InsitutionName Nullable(String)
) ENGINE = MergeTree()
ORDER BY (IssueID);
