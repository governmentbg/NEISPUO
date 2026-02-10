CREATE TABLE [school_books].[ClassBookStudentCarriedAbsence] (
    [SchoolYear]               SMALLINT         NOT NULL,
    [ClassBookId]              INT              NOT NULL,
    [PersonId]                 INT              NOT NULL,

    [FirstTermExcusedCount]    INT              NOT NULL,
    [FirstTermUnexcusedCount]  INT              NOT NULL,
    [FirstTermLateCount]       INT              NOT NULL,
    [SecondTermExcusedCount]   INT              NOT NULL,
    [SecondTermUnexcusedCount] INT              NOT NULL,
    [SecondTermLateCount]      INT              NOT NULL,

    CONSTRAINT [PK_ClassBookStudentCarriedAbsence] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId]),
    CONSTRAINT [FK_ClassBookStudentCarriedAbsence_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    
    -- external references
    CONSTRAINT [FK_ClassBookStudentCarriedAbsence_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO
