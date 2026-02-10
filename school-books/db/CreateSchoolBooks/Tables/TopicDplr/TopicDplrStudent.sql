PRINT 'Create TopicDplrStudent table'
GO

CREATE TABLE [school_books].[TopicDplrStudent] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,

    CONSTRAINT [PK_TopicDplrStudent] PRIMARY KEY ([SchoolYear], [TopicDplrId], [PersonId]),

    CONSTRAINT [FK_TopicDplrStudent_TopicDplr] FOREIGN KEY ([SchoolYear], [TopicDplrId])
        REFERENCES [school_books].[TopicDplr] ([SchoolYear], [TopicDplrId]),

    INDEX [IX_TopicDplrStudent_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicDplrStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

exec school_books.spDescTable  N'TopicDplrStudent', N'Ученик към тема на часа.'

exec school_books.spDescColumn N'TopicDplrStudent', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TopicDplrStudent', N'TopicDplrId'               , N'Идентификатор на тема на часа.'
exec school_books.spDescColumn N'TopicDplrStudent', N'PersonId'                  , N'Идентификатор на ученика.'
