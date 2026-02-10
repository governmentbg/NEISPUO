PRINT 'Create AbsencesByStudentsReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByStudentsReport'
GO

CREATE TABLE [school_books].[AbsencesByStudentsReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [AbsencesByStudentsReportId]        INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [Period]                            INT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_AbsencesByStudentsReport] PRIMARY KEY ([SchoolYear], [AbsencesByStudentsReportId]),

    CONSTRAINT [CHK_AbsencesByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_AbsencesByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_AbsencesByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_AbsencesByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_AbsencesByStudentsReport] UNIQUE ([SchoolYear], [InstId], [AbsencesByStudentsReportId]),
);
GO

exec school_books.spDescTable  N'AbsencesByStudentsReport', N'Справка отсъствия по ученици.'

exec school_books.spDescColumn N'AbsencesByStudentsReport', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'AbsencesByStudentsReportId', N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'AbsencesByStudentsReport', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'Period'                    , N'Филтър за период. 1 - първи срок, 2 - втори срок 3 - цялата година.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'ClassBookNames'            , N'Филтър за дневници.'

exec school_books.spDescColumn N'AbsencesByStudentsReport', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'AbsencesByStudentsReport', N'Version'                   , N'Версия.'
