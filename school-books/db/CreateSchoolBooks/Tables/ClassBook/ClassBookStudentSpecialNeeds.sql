PRINT 'Create ClassBookStudentSpecialNeeds table'
GO

CREATE TABLE [school_books].[ClassBookStudentSpecialNeeds] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [CurriculumId]              INT              NOT NULL,

    CONSTRAINT [PK_ClassBookStudentSpecialNeeds] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId], [CurriculumId]),
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_ClassBookStudentSpecialNeeds_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ClassBookStudentSpecialNeeds_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'ClassBookStudentSpecialNeeds', N'Дневник - СОП оценки по предмет.'

exec school_books.spDescColumn N'ClassBookStudentSpecialNeeds', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentSpecialNeeds', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentSpecialNeeds', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'ClassBookStudentSpecialNeeds', N'CurriculumId'              , N'Идентификатор на предмет от учебния план, по който ученика се оценява със СОП оценки.'
