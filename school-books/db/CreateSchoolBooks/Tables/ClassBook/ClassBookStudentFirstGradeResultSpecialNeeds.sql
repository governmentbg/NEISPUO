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

exec school_books.spDescTable  N'ClassBookStudentFirstGradeResultSpecialNeeds', N'Дневник - СОП оценка за общ годишен успех.'

exec school_books.spDescColumn N'ClassBookStudentFirstGradeResultSpecialNeeds', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentFirstGradeResultSpecialNeeds', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentFirstGradeResultSpecialNeeds', N'PersonId'                  , N'Идентификатор на ученик.'
