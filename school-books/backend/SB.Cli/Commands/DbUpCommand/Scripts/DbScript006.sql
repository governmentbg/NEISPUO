DROP INDEX [IX_Absence_ScheduleLesson] ON [school_books].[Absence]
GO

CREATE INDEX [IX_Absence_ScheduleLesson] ON [school_books].[Absence] ([SchoolYear], [ScheduleLessonId]) INCLUDE ([TeacherAbsenceId])
GO

DROP INDEX [IX_Grade_ScheduleLesson] ON [school_books].[Grade]
GO

CREATE INDEX [IX_Grade_ScheduleLesson] ON [school_books].[Grade] ([SchoolYear], [ScheduleLessonId]) INCLUDE ([TeacherAbsenceId])
GO

DROP INDEX [IX_TeacherAbsenceHour] ON [school_books].[TeacherAbsenceHour]
GO

CREATE UNIQUE INDEX [IX_TeacherAbsenceHour_ScheduleLesson] ON [school_books].[TeacherAbsenceHour] ([SchoolYear], [ScheduleLessonId]) INCLUDE ([TeacherAbsenceId], [ReplTeacherPersonId], [ReplTeacherIsNonSpecialist])
GO

ALTER TABLE [school_books].[Topic] DROP CONSTRAINT [UK_Topic_ScheduleLessonId]
GO

CREATE UNIQUE INDEX [IX_Topic_ScheduleLesson] ON [school_books].[Topic] ([SchoolYear], [ScheduleLessonId]) INCLUDE ([TeacherAbsenceId])
GO
