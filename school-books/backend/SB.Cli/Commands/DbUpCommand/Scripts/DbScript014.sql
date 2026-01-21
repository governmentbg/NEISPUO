ALTER TABLE [school_books].[Publication] DROP CONSTRAINT [FK_Publication_InstId]
GO

ALTER TABLE [school_books].[Publication] ADD CONSTRAINT [FK_Publication_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear])
GO
