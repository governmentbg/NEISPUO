GO

PRINT 'Create ClassBookStudentFirstGradeResultSpecialNeeds table'
GO

CREATE TABLE [school_books].[ClassBookStudentFirstGradeResultSpecialNeeds] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,

    CONSTRAINT [PK_ClassBookStudentFirstGradeResultSpecialNeeds] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId]),
    CONSTRAINT [FK_ClassBookStudentFirstGradeResultSpecialNeeds_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookStudentFirstGradeResultSpecialNeeds_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

ALTER TABLE [school_books].[FirstGradeResult]
ALTER COLUMN [QualitativeGrade] INT NULL;
GO

ALTER TABLE [school_books].[FirstGradeResult]
ADD
    [SpecialGrade] INT NULL
GO
