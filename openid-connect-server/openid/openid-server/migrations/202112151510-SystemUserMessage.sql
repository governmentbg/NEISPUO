CREATE TABLE core.SystemUserMessage (
    SystemUserMessageID int IDENTITY(1, 1) NOT NULL,
    Title VARCHAR(255) NULL,
    Content NVARCHAR(MAX) NULL,
    StartDate datetime2(7) DEFAULT SYSUTCDATETIME() NOT NULL,
    EndDate datetime2(7) DEFAULT SYSUTCDATETIME() NOT NULL,
    Roles NVARCHAR(MAX) NOT NULL
);
