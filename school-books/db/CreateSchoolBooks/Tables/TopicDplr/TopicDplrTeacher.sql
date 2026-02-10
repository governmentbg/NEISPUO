PRINT 'Create TopicDplrTeacher table'
GO

CREATE TABLE [school_books].[TopicDplrTeacher] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,

    CONSTRAINT [PK_TopicDplrTeacher] PRIMARY KEY ([SchoolYear], [TopicDplrId], [PersonId]),

    CONSTRAINT [FK_TopicDplrTeacher_TopicDplr] FOREIGN KEY ([SchoolYear], [TopicDplrId])
        REFERENCES [school_books].[TopicDplr] ([SchoolYear], [TopicDplrId]),

    INDEX [IX_TopicDplrTeacher_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicDplrTeacher_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

exec school_books.spDescTable  N'TopicDplrTeacher', N'Учител към тема на часа.'

exec school_books.spDescColumn N'TopicDplrTeacher', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TopicDplrTeacher', N'TopicDplrId'               , N'Идентификатор на тема на часа.'
exec school_books.spDescColumn N'TopicDplrTeacher', N'PersonId'                  , N'Идентификатор на учител.'
