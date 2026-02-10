PRINT 'Create ScheduleAndAbsencesByMonthReportWeekDay table'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeekDay] (
    [SchoolYear]                                      SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]              INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]          INT              NOT NULL,
    [Date]                                            DATE             NOT NULL,

    [DayName]                                         NVARCHAR(100)    NOT NULL,
    [IsOffDay]                                        BIT              NOT NULL,
    [IsEmptyDay]                                      BIT              NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeekDay] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeekDay_ScheduleAndAbsencesByMonthReportWeek]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReportWeek] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'ScheduleAndAbsencesByMonthReportWeekDay', N'Справка отсъствия/теми за месец - ден от седмицата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'SchoolYear'                                     , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'ScheduleAndAbsencesByMonthReportId'              , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'ScheduleAndAbsencesByMonthReportWeekId'          , N'Идентификатор на елемент от справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'Date'                                           , N'Дата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'DayName'                                        , N'Ден.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'IsOffDay'                                       , N'Неучебен ден – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDay', N'IsEmptyDay'                                     , N'Празен ден – Да/Не.'
