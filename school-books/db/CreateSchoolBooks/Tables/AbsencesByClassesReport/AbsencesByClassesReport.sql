PRINT 'Create AbsencesByClassesReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByClassesReport'
GO

CREATE TABLE [school_books].[AbsencesByClassesReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [AbsencesByClassesReportId]         INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [Period]                            INT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_AbsencesByClassesReport] PRIMARY KEY ([SchoolYear], [AbsencesByClassesReportId]),

    CONSTRAINT [CHK_AbsencesByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_AbsencesByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_AbsencesByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_AbsencesByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_AbsencesByClassesReport] UNIQUE ([SchoolYear], [InstId], [AbsencesByClassesReportId]),
);
GO

exec school_books.spDescTable  N'AbsencesByClassesReport', N'Справка отсъствия по класове.'

exec school_books.spDescColumn N'AbsencesByClassesReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'AbsencesByClassesReportId' , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'AbsencesByClassesReport', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'Period'                    , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'ClassBookNames'            , N'Филтър за дневници.'

exec school_books.spDescColumn N'AbsencesByClassesReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'AbsencesByClassesReport', N'Version'                   , N'Версия.'
