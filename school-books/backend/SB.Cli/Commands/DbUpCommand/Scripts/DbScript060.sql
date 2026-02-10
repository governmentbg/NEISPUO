GO

EXEC [school_books].[spCreateIdSequence] N'ClassBookTopicPlanItem'
GO

CREATE TABLE [school_books].[ClassBookTopicPlanItem] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [ClassBookTopicPlanItemId]  INT             NOT NULL,

    [ClassBookId]               INT             NOT NULL,
    [CurriculumId]              INT             NOT NULL,
    [Number]                    INT             NOT NULL,
    [Title]                     NVARCHAR (1000) NOT NULL,
    [Note]                      NVARCHAR (1000) NULL,
    [Taken]                     BIT             NOT NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [CreatedBySysUserId]        INT             NOT NULL,
    [ModifyDate]                DATETIME2       NOT NULL,
    [ModifiedBySysUserId]       INT             NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_ClassBookTopicPlanItem] PRIMARY KEY NONCLUSTERED ([SchoolYear], [ClassBookTopicPlanItemId]),
    CONSTRAINT [UK_ClassBookTopicPlanItem] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [CurriculumId], [ClassBookTopicPlanItemId]),
    CONSTRAINT [FK_ClassBookTopicPlanItem_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ClassBookTopicPlanItem_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ClassBookTopicPlanItem_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
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
);
GO

INSERT INTO [school_books].[TopicTitle] ([SchoolYear], [TopicId], [Index], [Title])
SELECT
    [SchoolYear],
    [TopicId],
    [Index] = 0,
    [Title]
FROM [school_books].[Topic]

ALTER TABLE [school_books].[Topic]
ALTER COLUMN [Title] NVARCHAR(1000) NULL
GO

CREATE TABLE [school_books].[TopicPlanPublisher] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_TopicPlanPublisher] PRIMARY KEY ([Id])
);
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

SET IDENTITY_INSERT [school_books].[TopicPlanPublisher] ON;

INSERT INTO [school_books].[TopicPlanPublisher] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, '[собствен материал]', '[собствен материал]', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Просвета', 'Просвета', 1,'2021-01-01 00:00:00.000', NULL),
(3, 3, 'Просвета Плюс', 'Просвета Плюс', 1,'2021-01-01 00:00:00.000', NULL),
(4, 4, 'Просвета АзБуки', 'Просвета АзБуки', 1,'2021-01-01 00:00:00.000', NULL),
(5, 5, 'Анубис', 'Анубис', 1,'2021-01-01 00:00:00.000', NULL),
(6, 6, 'Булвест 2000', 'Булвест 2000', 1,'2021-01-01 00:00:00.000', NULL),
(7, 7, 'Клет', 'Клет', 1,'2021-01-01 00:00:00.000', NULL),
(8, 8, 'Рива', 'Рива', 1,'2021-01-01 00:00:00.000', NULL),
(9, 9, 'Педагог 6', 'Педагог 6', 1,'2021-01-01 00:00:00.000', NULL),
(10, 10, 'Домино', 'Домино', 1,'2021-01-01 00:00:00.000', NULL),
(11, 11, 'Архимед 2', 'Архимед 2', 1,'2021-01-01 00:00:00.000', NULL),
(12, 12, 'OXFORD', 'OXFORD', 1,'2021-01-01 00:00:00.000', NULL),
(13, 13, 'Изкуства', 'Изкуства', 1,'2021-01-01 00:00:00.000', NULL),
(999, 999, 'Друго', 'Друго', 1,'2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[TopicPlanPublisher] OFF;

GO
