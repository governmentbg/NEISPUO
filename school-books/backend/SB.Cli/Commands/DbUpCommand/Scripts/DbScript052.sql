ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [CHK_Grade_Type];
GO

ALTER TABLE [school_books].[Grade] ADD CONSTRAINT [CHK_Grade_Type] CHECK ([Type] IN (1, 2, 3, 4, 5, 11, 12, 21, 22, 98, 99));
GO
