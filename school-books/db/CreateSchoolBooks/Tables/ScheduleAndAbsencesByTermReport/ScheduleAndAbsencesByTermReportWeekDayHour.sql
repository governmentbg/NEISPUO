PRINT 'Create ScheduleAndAbsencesByTermReportWeekDayHour table'
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

exec school_books.spDescTable  N'ScheduleAndAbsencesByTermReportWeekDayHour', N'Справка отсъствия/теми за срок - учебен час на ден от седмицата.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'SchoolYear'                                     , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'ScheduleAndAbsencesByTermReportId'              , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'ScheduleAndAbsencesByTermReportWeekId'          , N'Идентификатор на седмица от справка невписани теми.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'HourNumber'                                     , N'Номер на часа.'

exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'IsEmptyHour'                                    , N'Свободен час – Да/Не.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'CurriculumName'                                 , N'Име на предмета.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'CurriculumTeacherNames'                         , N'Имена на учителите по предмета.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'ExcusedStudentClassNumbers'                     , N'Списък от номерата на отсъстващите ученици по уважителни причини.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'UnexcusedStudentClassNumbers'                   , N'Списък от номерата на отсъстващите ученици по неуважителни причини.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'LateStudentClassNumbers'                        , N'Списък от номерата на закъснелите ученици.'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'DplrAbsenceStudentClassNumbers'                 , N'Списък от номерата на отсъстващите ученици (За ДПЛР).'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'DplrAttendanceStudentClassNumbers'              , N'Списък от номерата на присъстващите ученици (За ДПЛР).'
exec school_books.spDescColumn N'ScheduleAndAbsencesByTermReportWeekDayHour', N'Topics'                                         , N'Теми на урока.'
