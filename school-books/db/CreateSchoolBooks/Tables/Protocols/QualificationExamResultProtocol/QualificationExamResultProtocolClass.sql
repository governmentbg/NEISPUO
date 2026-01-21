PRINT 'Create QualificationExamResultProtocolClass table'
GO

CREATE TABLE [school_books].[QualificationExamResultProtocolClass] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [QualificationExamResultProtocolId]     INT              NOT NULL,
    [ClassId]                               INT              NOT NULL,

    CONSTRAINT [PK_QualificationExamResultProtocolClass] PRIMARY KEY ([SchoolYear], [QualificationExamResultProtocolId], [ClassId]),
    CONSTRAINT [FK_QualificationExamResultProtocolClass_QualificationExamResultProtocol] FOREIGN KEY ([SchoolYear], [QualificationExamResultProtocolId]) REFERENCES [school_books].[QualificationExamResultProtocol] ([SchoolYear], [QualificationExamResultProtocolId]),

    -- external references
    CONSTRAINT [FK_QualificationExamResultProtocolClass_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_QualificationExamResultProtocolClass_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'QualificationExamResultProtocolClass', N'Протокол за резултата от изпит за професионална квалификация - клас.'

exec school_books.spDescColumn N'QualificationExamResultProtocolClass', N'SchoolYear'                               , N'Учебна година.'
exec school_books.spDescColumn N'QualificationExamResultProtocolClass', N'QualificationExamResultProtocolId'        , N'Идентификатор на протокол за резултата от изпит за професионална квалификация.'
exec school_books.spDescColumn N'QualificationExamResultProtocolClass', N'ClassId'                                  , N'Идентификатор на група/паралелка.'
