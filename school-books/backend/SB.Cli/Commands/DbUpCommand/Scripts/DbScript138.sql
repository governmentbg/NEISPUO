ALTER TABLE [school_books].[ClassBook] ADD [IsValid] BIT NOT NULL DEFAULT 1
GO

ALTER TABLE [school_books].[ClassBook]
DROP CONSTRAINT [UQ_ClassBook_SchoolYear_ClassId]
GO

CREATE UNIQUE INDEX [UQ_ClassBook_SchoolYear_ClassId]
ON [school_books].[ClassBook] (SchoolYear, ClassId)
WHERE IsValid = 1
GO
