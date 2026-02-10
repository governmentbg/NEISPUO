PRINT 'Create TopicPlanItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'TopicPlanItem'
GO

CREATE TABLE [school_books].[TopicPlanItem] (
    [TopicPlanItemId]           INT             NOT NULL,

    [TopicPlanId]               INT             NOT NULL,
    [Number]                    INT             NOT NULL,
    [Title]                     NVARCHAR (1000) NOT NULL,
    [Note]                      NVARCHAR (1000) NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [ModifyDate]                DATETIME2       NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_TopicPlanItem] PRIMARY KEY NONCLUSTERED ([TopicPlanItemId]),
    CONSTRAINT [UK_TopicPlanItem] UNIQUE CLUSTERED ([TopicPlanId], [TopicPlanItemId]),
    CONSTRAINT [FK_TopicPlanItem_TopicPlan] FOREIGN KEY ([TopicPlanId]) REFERENCES [school_books].[TopicPlan] ([TopicPlanId]),
);
GO

exec school_books.spDescTable  N'TopicPlanItem', N'Тема от тематично разпределение.'

exec school_books.spDescColumn N'TopicPlanItem', N'TopicPlanItemId'           , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'TopicPlanItem', N'TopicPlanId'               , N'Идентификатор на учител създал тематичното разпределение.'
exec school_books.spDescColumn N'TopicPlanItem', N'Number'                    , N'Тема номер.'
exec school_books.spDescColumn N'TopicPlanItem', N'Title'                     , N'Тема.'
exec school_books.spDescColumn N'TopicPlanItem', N'Note'                      , N'Забележки.'

exec school_books.spDescColumn N'TopicPlanItem', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'TopicPlanItem', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'TopicPlanItem', N'Version'                   , N'Версия.'
