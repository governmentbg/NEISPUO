PRINT 'Create ClassBookCurriculumGradeless table'
GO

CREATE TABLE [school_books].[ClassBookCurriculumGradeless] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [CurriculumId]              INT              NOT NULL,

    CONSTRAINT [PK_ClassBookCurriculumGradeless] PRIMARY KEY ([SchoolYear], [ClassBookId], [CurriculumId]),
    CONSTRAINT [FK_ClassBookCurriculumGradeless_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookCurriculumGradeless_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ClassBookCurriculumGradeless_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'ClassBookCurriculumGradeless', N'Дневник - предмет без оценка.'

exec school_books.spDescColumn N'ClassBookCurriculumGradeless', N'SchoolYear'                          , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookCurriculumGradeless', N'ClassBookId'                         , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookCurriculumGradeless', N'CurriculumId'                        , N'Идентификатор на предмет от учебния план.'
