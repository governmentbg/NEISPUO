ALTER TABLE [school_books].[Schedule] ADD [IncludesWeekend] BIT NOT NULL CONSTRAINT DEFAULT_IncludesWeekend DEFAULT 0;
ALTER TABLE [school_books].[Schedule] DROP CONSTRAINT DEFAULT_IncludesWeekend;
GO

ALTER TABLE [school_books].[ScheduleDate] DROP CONSTRAINT [CHK_ScheduleDate_Day];
ALTER TABLE [school_books].[ScheduleDate] ADD CONSTRAINT [CHK_ScheduleDate_Day] CHECK ([Day] BETWEEN 1 AND 7);
GO

ALTER TABLE [school_books].[ScheduleHour] DROP CONSTRAINT [CHK_ScheduleHour_Day];
ALTER TABLE [school_books].[ScheduleHour] ADD CONSTRAINT [CHK_ScheduleHour_Day] CHECK ([Day] BETWEEN 1 AND 7);
GO

ALTER TABLE [school_books].[Shift] ADD [IsMultiday] BIT NOT NULL CONSTRAINT DEFAULT_IsMultiday DEFAULT 0;
ALTER TABLE [school_books].[Shift] DROP CONSTRAINT DEFAULT_IsMultiday;
GO

ALTER TABLE [school_books].[Shift] ADD [IsAdhoc] BIT NOT NULL CONSTRAINT DEFAULT_IsAdhoc DEFAULT 0;
ALTER TABLE [school_books].[Shift] DROP CONSTRAINT DEFAULT_IsAdhoc;
GO

ALTER TABLE [school_books].[ShiftHour] ADD [Day] INT NOT NULL CONSTRAINT DEFAULT_Day DEFAULT 1;
ALTER TABLE [school_books].[ShiftHour] DROP CONSTRAINT DEFAULT_Day;
GO

ALTER TABLE [school_books].[ShiftHour] DROP CONSTRAINT [PK_ShiftHour];
ALTER TABLE [school_books].[ShiftHour] ADD CONSTRAINT [PK_ShiftHour] PRIMARY KEY ([SchoolYear], [ShiftId], [Day], [HourNumber]);
ALTER TABLE [school_books].[ShiftHour] ADD CONSTRAINT [CHK_ShiftHour_Day] CHECK ([Day] BETWEEN 1 AND 7);
ALTER TABLE [school_books].[ShiftHour] ADD CONSTRAINT [CHK_ShiftHour_HourNumber] CHECK ([HourNumber] >= 0);
GO

INSERT INTO
    [school_books].[ShiftHour] (
        [SchoolYear],
        [ShiftId],
        [Day],
        [HourNumber],
        [StartTime],
        [EndTime]
    )
SELECT
    s.[SchoolYear],
    s.[ShiftId],
    sd.[Day],
    sh.[HourNumber],
    sh.[StartTime],
    sh.[EndTime]
FROM
    [school_books].[Shift] s
    INNER JOIN [school_books].[ShiftHour] sh ON s.[SchoolYear] = sh.[SchoolYear] AND s.[ShiftId] = sh.[ShiftId]
    CROSS JOIN (
        VALUES (2), (3), (4), (5), (6), (7)
    ) AS sd([Day])
GO
