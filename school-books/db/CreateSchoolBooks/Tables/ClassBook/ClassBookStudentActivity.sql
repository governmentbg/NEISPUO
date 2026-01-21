PRINT 'Create ClassBookStudentActivity table'
GO

CREATE TABLE [school_books].[ClassBookStudentActivity] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,

    [Activities]                NVARCHAR(1000)   NOT NULL,

    CONSTRAINT [PK_ClassBookStudentActivity] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId]),
    CONSTRAINT [FK_ClassBookStudentActivity_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookStudentActivity_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
);

exec school_books.spDescTable  N'ClassBookStudentActivity', N'Дневник - дейности по интереси.'

exec school_books.spDescColumn N'ClassBookStudentActivity', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentActivity', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentActivity', N'PersonId'                  , N'Идентификатор на ученик.'

exec school_books.spDescColumn N'ClassBookStudentActivity', N'Activities'                , N'Дейности по интереси за ученика.'
