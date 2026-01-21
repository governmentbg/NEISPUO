ALTER TABLE [school_books].[Grade] DROP CONSTRAINT IF EXISTS [CHK_Grade_ScheduleLessonId];
GO

ALTER TABLE [school_books].[Grade] ADD CONSTRAINT [CHK_Grade_ScheduleLessonId] CHECK (
    ([Type] NOT IN (21, 22, 98, 99) AND [ScheduleLessonId] IS NOT NULL) OR
    ([Type] IN     (21, 22, 98, 99) AND [ScheduleLessonId] IS NULL)
);
GO
