ALTER TABLE [school_books].[Absence] DROP CONSTRAINT [CHK_Absence_Type]
GO

ALTER TABLE [school_books].[Absence] ADD CONSTRAINT [CHK_Absence_Type] CHECK ([Type] IN (1, 2, 3, 4, 5))
GO
