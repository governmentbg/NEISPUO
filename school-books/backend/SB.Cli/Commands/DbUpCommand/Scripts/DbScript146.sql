ALTER TABLE [school_books].[GradelessStudentsReport] DROP CONSTRAINT [CHK_GradelessStudentsReport_Term]
GO

EXEC sp_rename 'school_books.GradelessStudentsReport.Term', 'Period', 'COLUMN'
GO

ALTER TABLE [school_books].[GradelessStudentsReport] ADD CONSTRAINT [CHK_GradelessStudentsReport_Period] CHECK ([Period] IN (1, 2, 3))
GO

UPDATE [school_books].[GradelessStudentsReport]
SET [Period] = 3 
WHERE [Period] IS NULL;
GO

ALTER TABLE [school_books].[GradelessStudentsReport]
ALTER COLUMN [Period] INT NOT NULL;
GO
