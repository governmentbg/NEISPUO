ALTER TABLE [school_books].[ScheduleHour] ADD [Location] NVARCHAR(550) NULL
GO

ALTER TABLE [school_books].[ScheduleLesson] DROP CONSTRAINT [FK_ScheduleLesson_ScheduleHour]
GO
