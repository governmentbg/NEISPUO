PRINT 'Create MissingTopicsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'MissingTopicsReport'
GO

CREATE TABLE [school_books].[MissingTopicsReport] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [MissingTopicsReportId] INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [Period]                INT              NOT NULL,
    [Year]                  INT              NULL,
    [Month]                 INT              NULL,
    [TeacherPersonId]       INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_MissingTopicsReport] PRIMARY KEY ([SchoolYear], [MissingTopicsReportId]),
    CONSTRAINT [CHK_MissingTopicsReport_Period] CHECK ([Period] IN (1, 2, 3, 4)),

    -- external references
    CONSTRAINT [FK_MissingTopicsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_MissingTopicsReport_TeacherPersonId] FOREIGN KEY ([TeacherPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_MissingTopicsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_MissingTopicsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_MissingTopicsReport] UNIQUE ([SchoolYear], [InstId], [MissingTopicsReportId]),
);
GO

exec school_books.spDescTable  N'MissingTopicsReport', N'Справка невписани теми.'

exec school_books.spDescColumn N'MissingTopicsReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'MissingTopicsReport', N'MissingTopicsReportId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'MissingTopicsReport', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'MissingTopicsReport', N'Period'                    , N'Период. 1 - за месец, 2 - За първи срок, 3 - за втори срок, 4 - за Цялата година.'
exec school_books.spDescColumn N'MissingTopicsReport', N'Year'                      , N'Филтър за година.'
exec school_books.spDescColumn N'MissingTopicsReport', N'Month'                     , N'Филтър за месец.'
exec school_books.spDescColumn N'MissingTopicsReport', N'TeacherPersonId'           , N'Филтър за учител.'

exec school_books.spDescColumn N'MissingTopicsReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'MissingTopicsReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'MissingTopicsReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'MissingTopicsReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'MissingTopicsReport', N'Version'                   , N'Версия.'
