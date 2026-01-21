DROP INDEX [IX_System_Schedule_SchoolYear_ClassBookId_IsIndividualCurriculum_ClassId_PersonId_Term] ON [school_books].[Schedule]
GO

ALTER TABLE [school_books].[Schedule] DROP COLUMN [IsSystem]
GO
