EXEC [school_books].[spCreateIdSequence] N'Performance'
GO

CREATE TABLE [school_books].[PerformanceType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_PerformanceType] PRIMARY KEY ([Id])
);
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

SET IDENTITY_INSERT [school_books].[PerformanceType] ON;

INSERT INTO [school_books].[PerformanceType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Организирана от ЦПЛР', 'Организирана от ЦПЛР', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'На общинско ниво', 'На общинско ниво', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'На регионално ниво', 'На регионално ниво', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'На национално ниво', 'На национално ниво', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'На международно ниво', 'На международно ниво', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[PerformanceType] OFF;

GO
