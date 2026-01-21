PRINT 'Create QualificationExamResultProtocolStudent table'
GO

CREATE TABLE [school_books].[QualificationExamResultProtocolStudent] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [QualificationExamResultProtocolId]     INT              NOT NULL,
    [ClassId]                               INT              NOT NULL,
    [PersonId]                              INT              NOT NULL,

    CONSTRAINT [PK_QualificationExamResultProtocolStudent] PRIMARY KEY ([SchoolYear], [QualificationExamResultProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_QualificationExamResultProtocolStudent_QualificationExamResultProtocol] FOREIGN KEY ([SchoolYear], [QualificationExamResultProtocolId]) REFERENCES [school_books].[QualificationExamResultProtocol] ([SchoolYear], [QualificationExamResultProtocolId]),

    -- external references
    CONSTRAINT [FK_QualificationExamResultProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_QualificationExamResultProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_QualificationExamResultProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'QualificationExamResultProtocolStudent', N'Протокол за резултата от изпит за професионална квалификация - ученик.'

exec school_books.spDescColumn N'QualificationExamResultProtocolStudent', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'QualificationExamResultProtocolStudent', N'QualificationExamResultProtocolId'      , N'Идентификатор на протокол за резултата от изпит за професионална квалификация.'
exec school_books.spDescColumn N'QualificationExamResultProtocolStudent', N'ClassId'                                , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'QualificationExamResultProtocolStudent', N'PersonId'                               , N'Идентификатор на ученик.'
