ALTER TABLE [school_books].[Grade]
ADD
    [IsReadFromParent] BIT NOT NULL CONSTRAINT DEFAULT_IsReadFromParent DEFAULT 0
GO

ALTER TABLE [school_books].[Grade]
DROP
    CONSTRAINT DEFAULT_IsReadFromParent
GO

ALTER TABLE [school_books].[Absence]
ADD
    [IsReadFromParent] BIT NOT NULL CONSTRAINT DEFAULT_IsReadFromParent DEFAULT 0
GO

ALTER TABLE [school_books].[Absence]
DROP
    CONSTRAINT DEFAULT_IsReadFromParent
GO

ALTER TABLE [school_books].[Remark]
ADD
    [IsReadFromParent] BIT NOT NULL CONSTRAINT DEFAULT_IsReadFromParent DEFAULT 0
GO

ALTER TABLE [school_books].[Remark]
DROP
    CONSTRAINT DEFAULT_IsReadFromParent
GO
