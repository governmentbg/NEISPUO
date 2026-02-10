PRINT 'Create DateAbsencesReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'DateAbsencesReport'
GO

CREATE TABLE [school_books].[DateAbsencesReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [DateAbsencesReportId]              INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [ReportDate]                        DATETIME2        NOT NULL,
    [IsUnited]                          BIT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NULL,
    [ShiftNames]                        NVARCHAR(MAX)    NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_DateAbsencesReport] PRIMARY KEY ([SchoolYear], [DateAbsencesReportId]),

    -- external references
    CONSTRAINT [FK_DateAbsencesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_DateAbsencesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_DateAbsencesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_DateAbsencesReport] UNIQUE ([SchoolYear], [InstId], [DateAbsencesReportId]),
);
GO

exec school_books.spDescTable  N'DateAbsencesReport', N'Справка отсъстващи за деня.'

exec school_books.spDescColumn N'DateAbsencesReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'DateAbsencesReport', N'DateAbsencesReportId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'DateAbsencesReport', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'DateAbsencesReport', N'ReportDate'                , N'Дата, за която е генерирана справката'
exec school_books.spDescColumn N'DateAbsencesReport', N'IsUnited'                  , N'Обединени часовете от всички смени – Да/Не.'
exec school_books.spDescColumn N'DateAbsencesReport', N'ClassBookNames'            , N'Филтър за дневници.'
exec school_books.spDescColumn N'DateAbsencesReport', N'ShiftNames'                , N'Филтър за смени.'

exec school_books.spDescColumn N'DateAbsencesReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'DateAbsencesReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'DateAbsencesReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'DateAbsencesReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'DateAbsencesReport', N'Version'                   , N'Версия.'
