GO

ALTER TABLE [school_books].[ClassBookPrint]
ADD [StatusDate] DATETIME2 NULL;
GO

UPDATE [school_books].[ClassBookPrint]
SET [StatusDate] = [CreateDate]
WHERE [StatusDate] IS NULL;
GO

ALTER TABLE [school_books].[ClassBookPrint]
ALTER COLUMN [StatusDate] DATETIME2 NOT NULL;
GO
