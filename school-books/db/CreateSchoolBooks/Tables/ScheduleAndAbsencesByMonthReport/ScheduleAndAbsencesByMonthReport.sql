PRINT 'Create ScheduleAndAbsencesByMonthReport table'
GO

EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByMonthReport'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReport] (
    [SchoolYear]                         SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId] INT              NOT NULL,

    [InstId]                             INT              NOT NULL,
    [IsDPLR]                             BIT              NOT NULL,
    [Year]                               INT              NOT NULL,
    [Month]                              INT              NOT NULL,
    [ClassBookName]                      NVARCHAR(560)    NOT NULL,

    [CreateDate]                         DATETIME2        NOT NULL,
    [CreatedBySysUserId]                 INT              NOT NULL,
    [ModifyDate]                         DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                INT              NOT NULL,
    [Version]                            ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReport] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId]),

    -- external references
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ScheduleAndAbsencesByMonthReport] UNIQUE ([SchoolYear], [InstId], [ScheduleAndAbsencesByMonthReportId]),
);
GO

exec school_books.spDescTable  N'ScheduleAndAbsencesByMonthReport', N'Справка отсъствия/теми за месец.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'ScheduleAndAbsencesByMonthReportId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'InstId'                                , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'IsDPLR'                                , N'Дневник за дейности за подкрепа за личностно развитие – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'Year'                                  , N'Година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'Month'                                 , N'Месец.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'ClassBookName'                         , N'Име на дневника.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'CreateDate'                            , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'CreatedBySysUserId'                    , N'Създадено от.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'ModifyDate'                            , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'ModifiedBySysUserId'                   , N'Последна модификация от.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReport', N'Version'                               , N'Версия.'
