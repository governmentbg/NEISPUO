PRINT 'Create ClassBookStudentGradeless table'
GO

CREATE TABLE [school_books].[ClassBookStudentGradeless] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [CurriculumId]              INT              NOT NULL,

    [WithoutFirstTermGrade]     BIT              NOT NULL,
    [WithoutSecondTermGrade]    BIT              NOT NULL,
    [WithoutFinalGrade]         BIT              NOT NULL,

    CONSTRAINT [PK_ClassBookStudentGradeless] PRIMARY KEY ([SchoolYear], [ClassBookId], [PersonId], [CurriculumId]),
    CONSTRAINT [FK_ClassBookStudentGradeless_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBookStudentGradeless_WithoutFirstTermGrade_WithoutSecondTermGrade_WithoutFinalGrade]
        CHECK (
            -- if all fields are FALSE (0) the record should not exist
            [WithoutFirstTermGrade] != 0 OR [WithoutSecondTermGrade] != 0 OR [WithoutFinalGrade] != 0
        ),

    -- external references
    CONSTRAINT [FK_ClassBookStudentGradeless_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_ClassBookStudentGradeless_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ClassBookStudentGradeless_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'ClassBookStudentGradeless', N'Дневник - освободен по предмет.'

exec school_books.spDescColumn N'ClassBookStudentGradeless', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentGradeless', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentGradeless', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'ClassBookStudentGradeless', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'

exec school_books.spDescColumn N'ClassBookStudentGradeless', N'WithoutFirstTermGrade'     , N'Освободен от оценка за 1ви срок – да/не.'
exec school_books.spDescColumn N'ClassBookStudentGradeless', N'WithoutSecondTermGrade'    , N'Освободен от оценка за 2ри срок – да/не.'
exec school_books.spDescColumn N'ClassBookStudentGradeless', N'WithoutFinalGrade'         , N'Освободен от годишна оценка – да/не.'
