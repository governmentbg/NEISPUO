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

EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByTermReportWeek'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByTermReportWeek] (
    [SchoolYear]                                     SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByTermReportId]              INT              NOT NULL,
    [ScheduleAndAbsencesByTermReportWeekId]          INT              NOT NULL,

    [WeekName]                                       NVARCHAR(100)    NOT NULL,
    [StudentName]                                    NVARCHAR(550)    NULL,
    [AdditionalActivities]                           NVARCHAR(1000)   NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByTermReportWeek] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReportWeek_ScheduleAndAbsencesByTermReport]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId])
        REFERENCES [school_books].[ScheduleAndAbsencesByTermReport] ([SchoolYear], [ScheduleAndAbsencesByTermReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByTermReportWeekDay] (
    [SchoolYear]                                     SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByTermReportId]              INT              NOT NULL,
    [ScheduleAndAbsencesByTermReportWeekId]          INT              NOT NULL,
    [Date]                                           DATE             NOT NULL,

    [DayName]                                        NVARCHAR(100)    NOT NULL,
    [IsOffDay]                                       BIT              NOT NULL,
    [IsEmptyDay]                                     BIT              NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByTermReportWeekDay] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId], [Date]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReportWeekDay_ScheduleAndAbsencesByTermReportWeek]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId])
        REFERENCES [school_books].[ScheduleAndAbsencesByTermReportWeek] ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByTermReportWeekDayHour] (
    [SchoolYear]                                         SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByTermReportId]                  INT              NOT NULL,
    [ScheduleAndAbsencesByTermReportWeekId]              INT              NOT NULL,
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

    CONSTRAINT [PK_ScheduleAndAbsencesByTermReportWeekDayHour]
        PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId], [Date], [HourNumber]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermReportWeekDayHour_ScheduleAndAbsencesByTermReportWeekDay]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId], [Date])
        REFERENCES [school_books].[ScheduleAndAbsencesByTermReportWeekDay] ([SchoolYear], [ScheduleAndAbsencesByTermReportId], [ScheduleAndAbsencesByTermReportWeekId], [Date])
)
WITH (DATA_COMPRESSION = PAGE);
GO
