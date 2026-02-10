CREATE INDEX [IX_ScheduleLesson_ScheduleHour] ON
    [school_books].[ScheduleLesson]([SchoolYear], [ScheduleId], [Day], [HourNumber], [CurriculumId])
GO
