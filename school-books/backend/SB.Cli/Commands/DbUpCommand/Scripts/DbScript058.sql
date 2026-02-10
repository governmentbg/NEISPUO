ALTER TABLE [school_books].[Topic] ADD CONSTRAINT [UK_Topic_ScheduleLesson] UNIQUE ([SchoolYear], [ScheduleLessonId])
GO

ALTER TABLE [school_books].[Topic] ADD CONSTRAINT [FK_Topic_TeacherAbsenceHour]
    FOREIGN KEY ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId])
    REFERENCES [school_books].[TeacherAbsenceHour] ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId])
GO

DROP INDEX [IX_Topic_ScheduleLesson] ON [school_books].[Topic]
GO

CREATE INDEX [IX_Topic_ScheduleLesson] ON [school_books].[Topic] ([SchoolYear], [ScheduleLessonId], [TeacherAbsenceId])
GO
