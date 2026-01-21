PRINT 'Create ScheduleAndAbsencesByMonthReportWeekDayHour table'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByMonthReportWeekDayHour] (
    [SchoolYear]                                          SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByMonthReportId]                  INT              NOT NULL,
    [ScheduleAndAbsencesByMonthReportWeekId]              INT              NOT NULL,
    [Date]                                                DATE             NOT NULL,
    [HourNumber]                                          INT              NOT NULL,

    [IsEmptyHour]                                         BIT              NULL,
    [CurriculumName]                                      NVARCHAR(550)    NULL,
    [CurriculumTeacherNames]                              NVARCHAR(1000)   NULL,
    [ExcusedStudentClassNumbers]                          NVARCHAR(100)    NULL,
    [UnexcusedStudentClassNumbers]                        NVARCHAR(100)    NULL,
    [LateStudentClassNumbers]                             NVARCHAR(100)    NULL,
    [DplrAbsenceStudentClassNumbers]                      NVARCHAR(100)    NULL,
    [DplrAttendanceStudentClassNumbers]                   NVARCHAR(100)    NULL,
    [Topics]                                              NVARCHAR(MAX)    NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByMonthReportWeekDayHour]
        PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date], [HourNumber]),
    CONSTRAINT [FK_ScheduleAndAbsencesByMonthReportWeekDayHour_ScheduleAndAbsencesByMonthReportWeekDay]
        FOREIGN KEY ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date])
        REFERENCES [school_books].[ScheduleAndAbsencesByMonthReportWeekDay] ([SchoolYear], [ScheduleAndAbsencesByMonthReportId], [ScheduleAndAbsencesByMonthReportWeekId], [Date])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'Справка отсъствия/теми за месец - учебен час на ден от седмицата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'SchoolYear'                                     , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'ScheduleAndAbsencesByMonthReportId'              , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'ScheduleAndAbsencesByMonthReportWeekId'          , N'Идентификатор на седмица от справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'HourNumber'                                     , N'Номер на часа.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'IsEmptyHour'                                    , N'Свободен час – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'CurriculumName'                                 , N'Име на предмета.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'CurriculumTeacherNames'                         , N'Имена на учителите по предмета.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'ExcusedStudentClassNumbers'                     , N'Списък от номерата на отсъстващите ученици по уважителни причини.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'UnexcusedStudentClassNumbers'                   , N'Списък от номерата на отсъстващите ученици по неуважителни причини.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'LateStudentClassNumbers'                        , N'Списък от номерата на закъснелите ученици.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'DplrAbsenceStudentClassNumbers'                 , N'Списък от номерата на отсъстващите ученици (За ДПЛР).'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'DplrAttendanceStudentClassNumbers'              , N'Списък от номерата на присъстващите ученици (За ДПЛР).'
exec school_books.spDescColumn N'ScheduleAndAbsencesByMonthReportWeekDayHour', N'Topics'                                         , N'Теми на урока.'
