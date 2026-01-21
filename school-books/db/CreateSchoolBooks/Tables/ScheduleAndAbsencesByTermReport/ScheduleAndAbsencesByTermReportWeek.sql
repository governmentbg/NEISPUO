PRINT 'Create ScheduleAndAbsencesByTermReportWeek table'
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

exec school_books.spDescTable  N'ScheduleAndAbsencesByTermReportWeek', N'Справка отсъствия/теми за срок - седмица.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'SchoolYear'                                , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'ScheduleAndAbsencesByTermReportId'         , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'ScheduleAndAbsencesByTermReportWeekId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'WeekName'                                  , N'Име на седмицата.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'StudentName'                               , N'Име на ученика. Приложимо за ИФО.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeek', N'AdditionalActivities'                      , N'Допълнителни дейности.'
