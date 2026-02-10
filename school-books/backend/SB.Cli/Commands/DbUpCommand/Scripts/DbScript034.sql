ALTER TABLE [school_books].[Schedule] ADD [IsSplitting] BIT NOT NULL CONSTRAINT DEFAULT_IsSplitting DEFAULT 0;
GO

ALTER TABLE [school_books].[Schedule] DROP CONSTRAINT DEFAULT_IsSplitting
GO

DROP INDEX [UQ_ScheduleDate_Date] ON [school_books].[VW_UQ_ScheduleDate_Date]
GO

DROP VIEW [school_books].[VW_UQ_ScheduleDate_Date]
GO

CREATE VIEW [school_books].[VW_UQ_ScheduleDate_Date]
WITH SCHEMABINDING
AS
    SELECT
        s.[SchoolYear],
        s.[ClassBookId],
        s.[IsIndividualCurriculum],
        s.[ClassId],
        s.[PersonId],
        sd.[Date]
    FROM
        [school_books].[Schedule] s
        INNER JOIN [school_books].[ScheduleDate] sd ON
            s.[SchoolYear] = sd.[SchoolYear] AND
            s.[ScheduleId] = sd.[ScheduleId]
    WHERE
        s.[IsSplitting] = 0
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_ScheduleDate_Date]
    ON [school_books].[VW_UQ_ScheduleDate_Date] ([SchoolYear], [ClassBookId], [IsIndividualCurriculum], [ClassId], [PersonId], [Date])
GO
