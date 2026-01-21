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

EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByMonthReportWeek'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeek] (
    [SchoolYear]                                     SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]             INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]         INT              NOT NULL,

    [WeekName]                                       NVARCHAR(100)    NOT NULL,
    [StudentName]                                    NVARCHAR(550)    NULL,
    [AdditionalActivities]                           NVARCHAR(1000)   NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeek] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeek_ScheduleAndAbsencesByMonthReport]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReport] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeekDay] (
    [SchoolYear]                                     SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]             INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]         INT              NOT NULL,
    [Date]                                           DATE             NOT NULL,

    [DayName]                                        NVARCHAR(100)    NOT NULL,
    [IsOffDay]                                       BIT              NOT NULL,
    [IsEmptyDay]                                     BIT              NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeekDay] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeekDay_ScheduleAndAbsencesByMonthReportWeek]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReportWeek] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeekDayHour] (
    [SchoolYear]                                         SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]                 INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]             INT              NOT NULL,
    [Date]                                               DATE             NOT NULL,
    [HourNumber]                                         INT              NOT NULL,

    [IsEmptyHour]                                        BIT              NULL,
    [CurriculumName]                                     NVARCHAR(550)    NULL,
    [CurriculumTeacherNames]                             NVARCHAR(1000)   NULL,
    [ExcusedStudentClassNumbers]                         NVARCHAR(100)    NULL,
    [UnexcusedStudentClassNumbers]                       NVARCHAR(100)    NULL,
    [LateStudentClassNumbers]                            NVARCHAR(100)    NULL,
    [DplrAbsenceStudentClassNumbers]                     NVARCHAR(100)    NULL,
    [DplrAttendanceStudentClassNumbers]                  NVARCHAR(100)    NULL,
    [Topics]                                             NVARCHAR(MAX)    NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeekDayHour]
        PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date], [HourNumber]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeekDayHour_ScheduleAndAbsencesByMonthReportWeekDay]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReportWeekDay] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date])
)
WITH (DATA_COMPRESSION = PAGE);
GO
