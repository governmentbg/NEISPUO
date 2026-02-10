ALTER TABLE [school_books].[PgResult] ADD [CurriculumId] INT NULL
GO

ALTER TABLE [school_books].[PgResult] DROP CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId]
ALTER TABLE [school_books].[PgResult] ADD CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId_CurriculumId] UNIQUE ([SchoolYear], [ClassBookId], [PersonId], [CurriculumId])
GO

CREATE INDEX [IX_PgResult_CurriculumId] ON [school_books].[PgResult] ([CurriculumId] ASC)
GO
