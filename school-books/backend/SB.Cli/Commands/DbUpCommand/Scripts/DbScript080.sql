GO

ALTER TABLE [school_books].[Grade] DROP CONSTRAINT [CHK_Grade_DecimalGrade];
ALTER TABLE [school_books].[Grade] ADD CONSTRAINT [CHK_Grade_DecimalGrade] CHECK ([DecimalGrade] = 2 OR [DecimalGrade] BETWEEN 3 AND 6);
GO
