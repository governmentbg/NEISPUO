PRINT 'Create ScheduleAndAbsencesByMonthReportWeek table'
GO

EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByMonthReportWeek'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeek] (
    [SchoolYear]                                      SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]              INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]          INT              NOT NULL,

    [WeekName]                                        NVARCHAR(100)    NOT NULL,
    [StudentName]                                     NVARCHAR(550)    NULL,
    [AdditionalActivities]                            NVARCHAR(1000)   NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeek] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeek_ScheduleAndAbsencesByMonthReport]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReport] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'ScheduleAndAbsencesByMonthReportWeek', N'Справка отсъствия/теми за месец - седмица.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'SchoolYear'                                , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'ScheduleAndAbsencesByMonthReportId'         , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'ScheduleAndAbsencesByMonthReportWeekId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'WeekName'                                  , N'Име на седмицата.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'StudentName'                               , N'Име на ученика. Приложимо за ИФО.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeek', N'AdditionalActivities'                      , N'Допълнителни дейности.'
