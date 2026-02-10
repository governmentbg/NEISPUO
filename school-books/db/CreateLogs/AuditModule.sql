PRINT 'Create AuditModule table'
GO

CREATE TABLE [logs].[AuditModule] (
    [AuditModuleId]          INT             NOT NULL,
    [Name]                   NVARCHAR(MAX)   NOT NULL,

    CONSTRAINT [PK_AuditModule] PRIMARY KEY ([AuditModuleId]),
);
GO
