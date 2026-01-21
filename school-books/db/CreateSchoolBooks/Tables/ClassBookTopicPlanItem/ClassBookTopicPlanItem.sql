PRINT 'Create ClassBookTopicPlanItem table'
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

exec school_books.spDescTable  N'ClassBookTopicPlanItem', N'Тема от тематичното разпределение за предмет в дневник.'

exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'ClassBookTopicPlanItemId'  , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'Number'                    , N'Тема номер.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'Title'                     , N'Тема.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'Note'                      , N'Забележки.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'Taken'                     , N'Взета - Да/Не.'

exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'ClassBookTopicPlanItem', N'Version'                   , N'Версия.'
