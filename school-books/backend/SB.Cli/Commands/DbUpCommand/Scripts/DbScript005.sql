ALTER TABLE [school_books].[ShiftHour] DROP CONSTRAINT [CHK_ShiftHour_StartTime_EndTime]
GO

ALTER TABLE [school_books].[ShiftHour] ADD CONSTRAINT [CHK_ShiftHour_StartTime_EndTime] CHECK ([StartTime] <= [EndTime])
GO
