GO

ALTER TABLE [school_books].[ClassBookPrint]
ADD [IsFinal] BIT NOT NULL CONSTRAINT [DEFAULT_ClassBookPrint_IsFinal] DEFAULT 0
GO

CREATE UNIQUE INDEX [UQ_ClassBookPrint_IsFinal]
ON [school_books].[ClassBookPrint] ([SchoolYear], [ClassBookId], [IsFinal]) WHERE [IsFinal] = 1
GO

ALTER TABLE [school_books].[ClassBookPrint]
DROP CONSTRAINT [DEFAULT_ClassBookPrint_IsFinal]
GO
