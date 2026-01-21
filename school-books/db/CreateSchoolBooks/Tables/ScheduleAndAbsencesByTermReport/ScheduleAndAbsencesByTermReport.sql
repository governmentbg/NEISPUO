PRINT 'Create ScheduleAndAbsencesByTermReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByTermReport'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByTermReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByTermReportId] INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [IsDPLR]                            BIT              NOT NULL,
    [Term]                              INT              NOT NULL,
    [ClassBookName]                     NVARCHAR(560)    NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByTermReport] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId]),

    CONSTRAINT [CHK_ScheduleAndAbsencesByTermReport_Term] CHECK ([Term] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ScheduleAndAbsencesByTermReport] UNIQUE ([SchoolYear], [InstId], [ScheduleAndAbsencesByTermReportId]),
);
GO

exec school_books.spDescTable  N'ScheduleAndAbsencesByTermReport', N'Справка отсъствия/теми за срок.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'ScheduleAndAbsencesByTermReportId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'InstId'                                , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'IsDPLR'                                , N'Дневник за дейности за подкрепа за личностно развитие – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'Term'                                  , N'Срок. 1 - Първи срок, 2 - Втори срок.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'ClassBookName'                         , N'Име на дневника.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'CreateDate'                            , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'CreatedBySysUserId'                    , N'Създадено от.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'ModifyDate'                            , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'ModifiedBySysUserId'                   , N'Последна модификация от.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReport', N'Version'                               , N'Версия.'
