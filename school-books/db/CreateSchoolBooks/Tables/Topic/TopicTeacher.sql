PRINT 'Create TopicTeacher table'
GO

CREATE TABLE [school_books].[TopicTeacher] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,

    CONSTRAINT [PK_TopicTeacher] PRIMARY KEY ([SchoolYear], [TopicId], [PersonId]),

    CONSTRAINT [FK_TopicTeacher_Topic] FOREIGN KEY ([SchoolYear], [TopicId])
        REFERENCES [school_books].[Topic] ([SchoolYear], [TopicId]),

    INDEX [IX_TopicTeacher_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicTeacher_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

exec school_books.spDescTable  N'TopicTeacher', N'Учител към тема на часа.'

exec school_books.spDescColumn N'TopicTeacher', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TopicTeacher', N'TopicId'                   , N'Идентификатор на тема на часа.'
exec school_books.spDescColumn N'TopicTeacher', N'PersonId'                  , N'Идентификатор на учител.'
exec school_books.spDescColumn N'TopicTeacher', N'IsReplTeacher'             , N'Заместник.'
