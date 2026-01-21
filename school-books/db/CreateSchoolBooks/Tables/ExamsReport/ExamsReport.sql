PRINT 'Create ExamsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'ExamsReport'
GO

CREATE TABLE [school_books].[ExamsReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [ExamsReportId]                         INT              NOT NULL,

    [InstId]                                INT              NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ExamsReport] PRIMARY KEY ([SchoolYear], [ExamsReportId]),

    -- external references
    CONSTRAINT [FK_ExamsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ExamsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ExamsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ExamsReport] UNIQUE ([SchoolYear], [InstId], [ExamsReportId]),
);
GO

exec school_books.spDescTable  N'ExamsReport', N'Справка контролни/класни.'

exec school_books.spDescColumn N'ExamsReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ExamsReport', N'ExamsReportId'             , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ExamsReport', N'InstId'                    , N'Идентификатор на институцията.'

exec school_books.spDescColumn N'ExamsReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ExamsReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ExamsReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ExamsReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'ExamsReport', N'Version'                   , N'Версия.'
