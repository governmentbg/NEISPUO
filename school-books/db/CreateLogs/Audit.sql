PRINT 'Create Audit table'
GO

CREATE TABLE [logs].[Audit] (
    [AuditId]                INT             NOT NULL IDENTITY,
    [AuditCorrelationId]     NVARCHAR(50)    NULL,

    [AuditModuleId]          INT             NOT NULL,
    [SysUserId]              INT             NULL,
    [SysRoleId]              INT             NULL,
    [Username]               NVARCHAR(MAX)   NULL,
    [FirstName]              NVARCHAR(MAX)   NULL,
    [MiddleName]             NVARCHAR(MAX)   NULL,
    [LastName]               NVARCHAR(MAX)   NULL,

    [LoginSessionId]         NVARCHAR(50)    NULL,
    [RemoteIpAddress]        NVARCHAR(50)    NOT NULL,
    [UserAgent]              NVARCHAR(MAX)   NOT NULL,
    [DateUtc]                DATETIME2       NOT NULL,

    [SchoolYear]             INT             NULL,
    [InstId]                 INT             NULL,
    [PersonId]               INT             NULL,
    [ObjectName]             NVARCHAR(50)    NULL,
    [ObjectId]               INT             NULL,

    [Action]                 NVARCHAR(50)    NOT NULL,
    [Data]                   NVARCHAR(MAX)   NOT NULL,

    CONSTRAINT [PK_Audit] PRIMARY KEY ([DateUtc], [AuditId]),
    CONSTRAINT [FK_Audit_AuditModuleId] FOREIGN KEY ([AuditModuleId]) REFERENCES [logs].[AuditModule] ([AuditModuleId]),
)
ON psLogs([DateUtc]);
GO
