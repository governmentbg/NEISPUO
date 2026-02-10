PRINT 'Create SessionStudentsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'SessionStudentsReport'
GO

CREATE TABLE [school_books].[SessionStudentsReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [SessionStudentsReportId]               INT              NOT NULL,

    [InstId]                                INT              NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SessionStudentsReport] PRIMARY KEY ([SchoolYear], [SessionStudentsReportId]),

    -- external references
    CONSTRAINT [FK_SessionStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SessionStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SessionStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SessionStudentsReport] UNIQUE ([SchoolYear], [InstId], [SessionStudentsReportId]),
);
GO

exec school_books.spDescTable  N'SessionStudentsReport', N'Справка ученици за поправителни изпити.'

exec school_books.spDescColumn N'SessionStudentsReport', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'SessionStudentsReport', N'SessionStudentsReportId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SessionStudentsReport', N'InstId'                                , N'Идентификатор на институцията.'

exec school_books.spDescColumn N'SessionStudentsReport', N'CreateDate'                            , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'SessionStudentsReport', N'CreatedBySysUserId'                    , N'Създадено от.'
exec school_books.spDescColumn N'SessionStudentsReport', N'ModifyDate'                            , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'SessionStudentsReport', N'ModifiedBySysUserId'                   , N'Последна модификация от.'
exec school_books.spDescColumn N'SessionStudentsReport', N'Version'                               , N'Версия.'
