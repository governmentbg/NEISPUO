PRINT 'Create ExamResultProtocolStudent table'
GO

CREATE TABLE [school_books].[ExamResultProtocolStudent] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [ExamResultProtocolId]    INT              NOT NULL,
    [ClassId]                 INT              NOT NULL,
    [PersonId]                INT              NOT NULL,

    CONSTRAINT [PK_ExamResultProtocolStudent] PRIMARY KEY ([SchoolYear], [ExamResultProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_ExamResultProtocolStudent_ExamResultProtocol] FOREIGN KEY ([SchoolYear], [ExamResultProtocolId]) REFERENCES [school_books].[ExamResultProtocol] ([SchoolYear], [ExamResultProtocolId]),

    -- external references
    CONSTRAINT [FK_ExamResultProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ExamResultProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ExamResultProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'ExamResultProtocolStudent', N'Протокол за резултат от изпит - ученик.'

exec school_books.spDescColumn N'ExamResultProtocolStudent', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ExamResultProtocolStudent', N'ExamResultProtocolId'      , N'Идентификатор на протокол за резултат от изпит.'
exec school_books.spDescColumn N'ExamResultProtocolStudent', N'ClassId'                   , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'ExamResultProtocolStudent', N'PersonId'                  , N'Идентификатор на ученик.'
