PRINT 'Create TopicTitle table'
GO

CREATE TABLE [school_books].[TopicTitle] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [Index]                     INT             NOT NULL,

    [Title]                     NVARCHAR(1000)  NOT NULL,
    [ClassBookTopicPlanItemId]  INT             NULL,

    CONSTRAINT [PK_TopicTitle] PRIMARY KEY ([SchoolYear], [TopicId], [Index]),

    CONSTRAINT [FK_TopicTitle_Topic] FOREIGN KEY ([SchoolYear], [TopicId])
        REFERENCES [school_books].[Topic] ([SchoolYear], [TopicId]),
    CONSTRAINT [FK_TopicTitle_ClassBookTopicPlanItem] FOREIGN KEY ([SchoolYear], [ClassBookTopicPlanItemId])
        REFERENCES [school_books].[ClassBookTopicPlanItem] ([SchoolYear], [ClassBookTopicPlanItemId]),

    -- ClassBookTopicPlanItem deletetion helper index
    INDEX [IX_TopicTitle_ClassBookTopicPlanItem] ([SchoolYear], [ClassBookTopicPlanItemId]),
);
GO

exec school_books.spDescTable  N'TopicTitle', N'Заглавие към тема на часа.'

exec school_books.spDescColumn N'TopicTitle', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TopicTitle', N'TopicId'                   , N'Идентификатор на тема на часа.'
exec school_books.spDescColumn N'TopicTitle', N'Index'                     , N'Пореден номер.'

exec school_books.spDescColumn N'TopicTitle', N'Title'                     , N'Тема.'
exec school_books.spDescColumn N'TopicTitle', N'ClassBookTopicPlanItemId'  , N'Идентификатор на тема от тематично разпределение.'
