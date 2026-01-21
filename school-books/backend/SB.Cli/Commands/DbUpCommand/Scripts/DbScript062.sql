-- Absence
DROP INDEX [IX_Absence_ClassId] ON [school_books].[Absence];
ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [FK_Absence_ClassGroup];
ALTER TABLE [school_books].[Absence] DROP COLUMN [ClassId];
GO

-- Attendance
DROP INDEX [IX_Attendance_ClassId] ON [school_books].[Attendance];
ALTER TABLE [school_books].[Attendance] DROP CONSTRAINT [FK_Attendance_ClassGroup];
ALTER TABLE [school_books].[Attendance] DROP COLUMN [ClassId];
GO

-- FirstGradeResult
DROP INDEX [IX_FirstGradeResult_ClassId] ON [school_books].[FirstGradeResult];
ALTER TABLE [school_books].[FirstGradeResult] DROP CONSTRAINT [FK_FirstGradeResult_ClassGroup];
ALTER TABLE [school_books].[FirstGradeResult] DROP COLUMN [ClassId];
GO

-- Grade
DROP INDEX [IX_Grade_ClassId] ON [school_books].[Grade];
ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [FK_Grade_ClassGroup];
ALTER TABLE [school_books].[Grade] DROP COLUMN [ClassId];
GO

-- GradeResult
DROP INDEX [IX_GradeResult_ClassId] ON [school_books].[GradeResult];
ALTER TABLE [school_books].[GradeResult] DROP CONSTRAINT [FK_GradeResult_ClassGroup];
ALTER TABLE [school_books].[GradeResult] DROP COLUMN [ClassId];
GO

-- IndividualWork
DROP INDEX [IX_IndividualWork_ClassId] ON [school_books].[IndividualWork];
ALTER TABLE [school_books].[IndividualWork] DROP CONSTRAINT [FK_IndividualWork_ClassGroup];
ALTER TABLE [school_books].[IndividualWork] DROP COLUMN [ClassId];
GO

-- NoteStudent
DROP INDEX [IX_NoteStudent_ClassId] ON [school_books].[NoteStudent];
ALTER TABLE [school_books].[NoteStudent] DROP CONSTRAINT [PK_NoteStudent];
ALTER TABLE [school_books].[NoteStudent] DROP CONSTRAINT [FK_NoteStudent_ClassGroup];
ALTER TABLE [school_books].[NoteStudent] DROP COLUMN [ClassId];
ALTER TABLE [school_books].[NoteStudent] ADD CONSTRAINT [PK_NoteStudent] PRIMARY KEY ([SchoolYear], [NoteId], [PersonId]);
GO

-- PgResult
DROP INDEX [IX_PgResult_ClassId] ON [school_books].[PgResult];
ALTER TABLE [school_books].[PgResult] DROP CONSTRAINT [FK_PgResult_ClassGroup];
ALTER TABLE [school_books].[PgResult] DROP COLUMN [ClassId];
GO

-- Remark
DROP INDEX [IX_Remark_ClassId] ON [school_books].[Remark];
ALTER TABLE [school_books].[Remark] DROP CONSTRAINT [FK_Remark_ClassGroup];
ALTER TABLE [school_books].[Remark] DROP COLUMN [ClassId];
GO

-- Sanction
DROP INDEX [IX_Sanction_ClassId] ON [school_books].[Sanction];
ALTER TABLE [school_books].[Sanction] DROP CONSTRAINT [FK_Sanction_ClassGroup];
ALTER TABLE [school_books].[Sanction] DROP COLUMN [ClassId];
GO

-- Schedule
ALTER TABLE [school_books].[Schedule] DROP CONSTRAINT [CHK_Schedule_IsIndividualCurriculum_ClassId_PersonId];
ALTER TABLE [school_books].[Schedule] ADD CONSTRAINT [CHK_Schedule_IsIndividualCurriculum_PersonId]
    CHECK (
        ([IsIndividualCurriculum] = 0 AND [PersonId] IS NULL) OR
        ([IsIndividualCurriculum] = 1 AND [PersonId] IS NOT NULL)
    );
GO

DROP INDEX [UQ_ScheduleDate_Date] ON [school_books].[VW_UQ_ScheduleDate_Date];
DROP VIEW [school_books].[VW_UQ_ScheduleDate_Date];
GO
CREATE VIEW [school_books].[VW_UQ_ScheduleDate_Date]
WITH SCHEMABINDING
AS
    SELECT
        s.[SchoolYear],
        s.[ClassBookId],
        s.[IsIndividualCurriculum],
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
    ON [school_books].[VW_UQ_ScheduleDate_Date] ([SchoolYear], [ClassBookId], [IsIndividualCurriculum], [PersonId], [Date])
GO

DROP INDEX [IX_Schedule_ClassId] ON [school_books].[Schedule];
ALTER TABLE [school_books].[Schedule] DROP CONSTRAINT [FK_Schedule_ClassGroup];
ALTER TABLE [school_books].[Schedule] DROP COLUMN [ClassId];
GO

-- SupportStudent
DROP INDEX [IX_SupportStudent_ClassId] ON [school_books].[SupportStudent];
ALTER TABLE [school_books].[SupportStudent] DROP CONSTRAINT [PK_SupportStudent];
ALTER TABLE [school_books].[SupportStudent] DROP CONSTRAINT [FK_SupportStudent_ClassGroup];
ALTER TABLE [school_books].[SupportStudent] DROP COLUMN [ClassId];
ALTER TABLE [school_books].[SupportStudent] ADD CONSTRAINT [PK_SupportStudent] PRIMARY KEY ([SchoolYear], [SupportId], [PersonId]);
GO
