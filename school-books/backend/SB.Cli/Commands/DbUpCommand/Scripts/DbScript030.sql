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
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_ScheduleDate_Date]
    ON [school_books].[VW_UQ_ScheduleDate_Date] ([SchoolYear], [ClassBookId], [IsIndividualCurriculum], [ClassId], [PersonId], [Date])
GO
