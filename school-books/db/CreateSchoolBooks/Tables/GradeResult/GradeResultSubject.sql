PRINT 'Create GradeResultSubject table'
GO

CREATE TABLE [school_books].[GradeResultSubject] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [GradeResultId]         INT              NOT NULL,
    [CurriculumId]          INT              NOT NULL,

    [Session1Grade]         INT              NULL,
    [Session1NoShow]        BIT              NULL,
    [Session2Grade]         INT              NULL,
    [Session2NoShow]        BIT              NULL,
    [Session3Grade]         INT              NULL,
    [Session3NoShow]        BIT              NULL,

    CONSTRAINT [PK_GradeResultSubject] PRIMARY KEY ([SchoolYear], [GradeResultId], [CurriculumId]),

    CONSTRAINT [FK_GradeResultSubject_GradeResult] FOREIGN KEY ([SchoolYear], [GradeResultId]) REFERENCES [school_books].[GradeResult] ([SchoolYear], [GradeResultId]),

    -- external references
    CONSTRAINT [FK_GradeResultSubject_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_GradeResultSubject_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'GradeResultSubject', N'Годишен резултат - поправителни сесии по предмет.'

exec school_books.spDescColumn N'GradeResultSubject', N'SchoolYear'                          , N'Учебна година.'
exec school_books.spDescColumn N'GradeResultSubject', N'GradeResultId'                       , N'Идентификатор на годишен резултат.'
exec school_books.spDescColumn N'GradeResultSubject', N'CurriculumId'                        , N'Идентификатор на предмет от учебния план.'

exec school_books.spDescColumn N'GradeResultSubject', N'Session1Grade'                       , N'Оценка от 1ва сесия. Цяло число от 2 до 6.'
exec school_books.spDescColumn N'GradeResultSubject', N'Session1NoShow'                      , N'Неявил се на 1ва сесия.'
exec school_books.spDescColumn N'GradeResultSubject', N'Session2Grade'                       , N'Оценка от 2ра сесия. Цяло число от 2 до 6.'
exec school_books.spDescColumn N'GradeResultSubject', N'Session2NoShow'                      , N'Неявил се на 2ра сесия.'
exec school_books.spDescColumn N'GradeResultSubject', N'Session3Grade'                       , N'Оценка от допълнителна сесия. Цяло число от 2 до 6.'
exec school_books.spDescColumn N'GradeResultSubject', N'Session3NoShow'                      , N'Неявил се на допълнителна сесия.'
