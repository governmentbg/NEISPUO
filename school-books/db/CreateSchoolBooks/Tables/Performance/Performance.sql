PRINT 'Create Performance table'
GO

EXEC [school_books].[spCreateIdSequence] N'Performance'
GO

CREATE TABLE [school_books].[Performance] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [PerformanceId]         INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PerformanceTypeId]     INT              NOT NULL,
    [Name]                  NVARCHAR(100)    NOT NULL,
    [Description]           NVARCHAR(MAX)    NOT NULL,
    [StartDate]             DATE             NOT NULL,
    [EndDate]               DATE             NOT NULL,
    [Location]              NVARCHAR(100)    NOT NULL,
    [StudentAwards]         NVARCHAR(MAX)    NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Performance] PRIMARY KEY NONCLUSTERED ([SchoolYear], [PerformanceId]),
    CONSTRAINT [UK_Performance] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [PerformanceId]),

    CONSTRAINT [FK_Performance_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_Performance_PerformanceType] FOREIGN KEY ([PerformanceTypeId]) REFERENCES [school_books].[PerformanceType] ([Id]),
    CONSTRAINT [FK_Performance_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Performance_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Performance', N'Изява.'

exec school_books.spDescColumn N'Performance', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Performance', N'PerformanceId'             , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Performance', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Performance', N'PerformanceTypeId'         , N'Вид на изявата. Номенклатура PerformanceType.'
exec school_books.spDescColumn N'Performance', N'Name'                      , N'Наименование.'
exec school_books.spDescColumn N'Performance', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'Performance', N'StartDate'                 , N'Период на провеждане от.'
exec school_books.spDescColumn N'Performance', N'EndDate'                   , N'Период на провеждане до.'
exec school_books.spDescColumn N'Performance', N'Location'                  , N'Място на провеждане.'
exec school_books.spDescColumn N'Performance', N'StudentAwards'             , N'Получени награди от учениците.'

exec school_books.spDescColumn N'Performance', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Performance', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Performance', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Performance', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Performance', N'Version'                   , N'Версия.'
