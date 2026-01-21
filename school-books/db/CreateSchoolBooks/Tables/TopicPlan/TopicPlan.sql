PRINT 'Create TopicPlan table'
GO

EXEC [school_books].[spCreateIdSequence] N'TopicPlan'
GO

CREATE TABLE [school_books].[TopicPlan] (
    [TopicPlanId]               INT             NOT NULL,

    [Name]                      NVARCHAR (100)  NOT NULL,
    [BasicClassId]              INT             NULL,
    [SubjectId]                 INT             NULL,
    [SubjectTypeId]             INT             NULL,
    [TopicPlanPublisherId]      INT             NULL,
    [TopicPlanPublisherOther]   NVARCHAR (MAX)  NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [CreatedBySysUserId]        INT             NOT NULL,
    [ModifyDate]                DATETIME2       NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_TopicPlan] PRIMARY KEY NONCLUSTERED ([TopicPlanId]),
    CONSTRAINT [UK_TopicPlan] UNIQUE CLUSTERED ([CreatedBySysUserId], [TopicPlanId]),
    CONSTRAINT [FK_TopicPlan_TopicPlanPublisher] FOREIGN KEY([TopicPlanPublisherId]) REFERENCES [school_books].[TopicPlanPublisher] ([Id]),

    -- external references
    CONSTRAINT [FK_TopicPlan_BasicClassId] FOREIGN KEY ([BasicClassId]) REFERENCES [inst_nom].[BasicClass] ([BasicClassID]),
    CONSTRAINT [FK_TopicPlan_Subject] FOREIGN KEY([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_TopicPlan_SubjectType] FOREIGN KEY([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_TopicPlan_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'TopicPlan', N'Тематично разпределение.'

exec school_books.spDescColumn N'TopicPlan', N'TopicPlanId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'TopicPlan', N'Name'                      , N'Наименование.'
exec school_books.spDescColumn N'TopicPlan', N'BasicClassId'              , N'Идентификатор на випуска. Номенклатура inst_nom.BasicClass.'
exec school_books.spDescColumn N'TopicPlan', N'SubjectId'                 , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'TopicPlan', N'SubjectTypeId'             , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'TopicPlan', N'TopicPlanPublisherId'      , N'Идентификатор на издателство. Номенклатура school_books.TopicPlanPublisher.'
exec school_books.spDescColumn N'TopicPlan', N'TopicPlanPublisherOther'   , N'Друго издателство.'

exec school_books.spDescColumn N'TopicPlan', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'TopicPlan', N'CreatedBySysUserId'        , N'Идентификатор на учител създал тематичното разпределение.'
exec school_books.spDescColumn N'TopicPlan', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'TopicPlan', N'Version'                   , N'Версия.'
