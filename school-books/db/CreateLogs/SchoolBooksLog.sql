PRINT 'Create SchoolBooksLog table'
GO

CREATE TABLE [logs].[SchoolBooksLog] (
    [DateUtc]               DATETIME2       NOT NULL,

    -- standard
    [Level]                 NVARCHAR(50)    NULL,
    [MessageTemplate]       NVARCHAR(MAX)   NULL,
    [Exception]             NVARCHAR(MAX)   NULL,
    [LogEvent]              NVARCHAR(MAX)   NULL,

    -- additional
    [App]                   NVARCHAR(100)   NULL,
    [AppVersion]            NVARCHAR(50)    NULL,
    [Pod]                   NVARCHAR(100)   NULL,
    [IpAddress]             NVARCHAR(50)    NULL,
    [RequestMethod]         NVARCHAR(50)    NULL,
    [RequestPath]           NVARCHAR(MAX)   NULL,
    [StatusCode]            NVARCHAR(50)    NULL,
    [SourceContext]         NVARCHAR(250)   NULL,
    [RequestId]             NVARCHAR(50)    NULL,
    [ActionName]            NVARCHAR(250)   NULL,
    [ElapsedMs]             INT             NULL,
    [SysUserId]             INT             NULL,
    [SessionId]             NVARCHAR(50)    NULL,

    INDEX [CCI_SchoolBooksLog] CLUSTERED COLUMNSTORE,
)
ON psLogsWeekly([DateUtc]);
GO

CREATE TABLE [logs].[SchoolBooksLog2] (
    [DateUtc]               DATETIME2       NOT NULL,

    -- standard
    [Level]                 NVARCHAR(50)    NULL,
    [MessageTemplate]       NVARCHAR(MAX)   NULL,
    [Exception]             NVARCHAR(MAX)   NULL,
    [LogEvent]              NVARCHAR(MAX)   NULL,

    -- additional
    [App]                   NVARCHAR(100)   NULL,
    [AppVersion]            NVARCHAR(50)    NULL,
    [Pod]                   NVARCHAR(100)   NULL,
    [IpAddress]             NVARCHAR(50)    NULL,
    [RequestMethod]         NVARCHAR(50)    NULL,
    [RequestPath]           NVARCHAR(MAX)   NULL,
    [StatusCode]            NVARCHAR(50)    NULL,
    [SourceContext]         NVARCHAR(250)   NULL,
    [RequestId]             NVARCHAR(50)    NULL,
    [ActionName]            NVARCHAR(250)   NULL,
    [ElapsedMs]             INT             NULL,
    [SysUserId]             INT             NULL,
    [SessionId]             NVARCHAR(50)    NULL,

    INDEX [CCI_SchoolBooksLog2] CLUSTERED COLUMNSTORE,
)
ON psLogsWeekly([DateUtc]);
GO
