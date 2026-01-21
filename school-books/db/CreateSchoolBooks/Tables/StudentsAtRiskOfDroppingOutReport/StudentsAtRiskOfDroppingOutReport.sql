PRINT 'Create StudentsAtRiskOfDroppingOutReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'StudentsAtRiskOfDroppingOutReport'
GO

CREATE TABLE [school_books].[StudentsAtRiskOfDroppingOutReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [StudentsAtRiskOfDroppingOutReportId]   INT              NOT NULL,

    [InstId]                                INT              NOT NULL,
    [ReportDate]                            DATETIME2        NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_StudentsAtRiskOfDroppingOutReport] PRIMARY KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId]),

    -- external references
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_StudentsAtRiskOfDroppingOutReport] UNIQUE ([SchoolYear], [InstId], [StudentsAtRiskOfDroppingOutReportId]),
);
GO

exec school_books.spDescTable  N'StudentsAtRiskOfDroppingOutReport', N'Справка ученици с риск от отпадане.'

exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'StudentsAtRiskOfDroppingOutReportId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'InstId'                                , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'ReportDate'                            , N'Дата до която е генерирана справката'

exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'CreateDate'                            , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'CreatedBySysUserId'                    , N'Създадено от.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'ModifyDate'                            , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'ModifiedBySysUserId'                   , N'Последна модификация от.'
exec school_books.spDescColumn N'StudentsAtRiskOfDroppingOutReport', N'Version'                               , N'Версия.'
