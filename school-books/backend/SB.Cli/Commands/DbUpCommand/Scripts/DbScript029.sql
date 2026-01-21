ALTER TABLE  [school_books].[Absence]
DROP
    CONSTRAINT [FK_Absence_ScheduleLesson_Date],
    CONSTRAINT [FK_Absence_ScheduleLesson_CurriculumId];

ALTER TABLE [school_books].[Grade]
DROP
    CONSTRAINT [FK_Grade_ScheduleLesson_Date],
    CONSTRAINT [FK_Grade_ScheduleLesson_CurriculumId];

ALTER TABLE [school_books].[Topic]
DROP
    CONSTRAINT [FK_Topic_ScheduleLesson_Date],
    CONSTRAINT [FK_Topic_TeacherAbsenceHour];
GO

ALTER TABLE [school_books].[ScheduleLesson]
DROP
    CONSTRAINT [UQ_ScheduleLesson_Date],
    CONSTRAINT [UQ_ScheduleLesson_CurriculumId];
