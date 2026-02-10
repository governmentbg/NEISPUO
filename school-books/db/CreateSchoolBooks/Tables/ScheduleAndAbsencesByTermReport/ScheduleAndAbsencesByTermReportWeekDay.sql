PRINT 'Create ScheduleAndAbsencesByTermReportWeekDay table'
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

exec school_books.spDescTable  N'ScheduleAndAbsencesByTermReportWeekDay', N'Справка отсъствия/теми за срок - ден от седмицата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'SchoolYear'                                     , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'ScheduleAndAbsencesByTermReportId'              , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'ScheduleAndAbsencesByTermReportWeekId'          , N'Идентификатор на елемент от справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'Date'                                           , N'Дата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'DayName'                                        , N'Ден.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'IsOffDay'                                       , N'Неучебен ден – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDay', N'IsEmptyDay'                                     , N'Празен ден – Да/Не.'
