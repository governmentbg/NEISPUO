ALTER TABLE [school_books].[Support]
ADD
    [IsForAllStudents] BIT NOT NULL CONSTRAINT DEFAULT_IsForAllStudents DEFAULT 0;
GO

ALTER TABLE [school_books].[Support]
DROP
    CONSTRAINT DEFAULT_IsForAllStudents
GO
