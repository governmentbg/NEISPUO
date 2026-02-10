PRINT 'Create ExamResultProtocolClass table'
GO

CREATE TABLE [school_books].[ExamResultProtocolClass] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [ExamResultProtocolId]    INT              NOT NULL,
    [ClassId]                 INT              NOT NULL,

    CONSTRAINT [PK_ExamResultProtocolClass] PRIMARY KEY ([SchoolYear], [ExamResultProtocolId], [ClassId]),
    CONSTRAINT [FK_ExamResultProtocolClass_ExamResultProtocol] FOREIGN KEY ([SchoolYear], [ExamResultProtocolId]) REFERENCES [school_books].[ExamResultProtocol] ([SchoolYear], [ExamResultProtocolId]),

    -- external references
    CONSTRAINT [FK_ExamResultProtocolClass_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ExamResultProtocolClass_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'ExamResultProtocolClass', N'Протокол за резултат от изпит - клас.'

exec school_books.spDescColumn N'ExamResultProtocolClass', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'ExamResultProtocolClass', N'ExamResultProtocolId'           , N'Идентификатор на протокол за резултат от изпит.'
exec school_books.spDescColumn N'ExamResultProtocolClass', N'ClassId'                        , N'Идентификатор на група/паралелка.'
