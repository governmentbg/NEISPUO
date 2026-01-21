GO

CREATE TABLE [school_books].[TopicTeacher] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,

    CONSTRAINT [PK_TopicTeacher] PRIMARY KEY ([SchoolYear], [TopicId], [PersonId]),

    CONSTRAINT [FK_TopicTeacher_Topic] FOREIGN KEY ([SchoolYear], [TopicId])
        REFERENCES [school_books].[Topic] ([SchoolYear], [TopicId]),

    -- external references
    CONSTRAINT [FK_TopicTeacher_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO
