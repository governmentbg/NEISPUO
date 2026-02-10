ALTER TABLE [school_books].[Sanction] DROP CONSTRAINT [FK_Sanction_SanctionType]
GO

ALTER TABLE [school_books].[Sanction] ADD CONSTRAINT [FK_Sanction_SanctionType] FOREIGN KEY ([SanctionTypeId]) REFERENCES [student].[SanctionType] ([Id])
GO

DROP TABLE [school_books].[SanctionType]
GO
