CREATE TABLE [school_books].[ReplrParticipationType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ReplrParticipationType] PRIMARY KEY ([Id])
);
GO

EXEC [school_books].[spCreateIdSequence] N'ReplrParticipation'
GO

CREATE TABLE [school_books].[ReplrParticipation] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ReplrParticipationId]      INT              NOT NULL,

    [ClassBookId]               INT              NOT NULL,
    [ReplrParticipationTypeId]  INT              NOT NULL,
    [Topic]                     NVARCHAR(MAX)    NULL,
    [Date]                      DATE             NOT NULL,
    [Attendees]                 NVARCHAR(MAX)    NOT NULL,
    [InstId]                    INT              NULL,

    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [ModifyDate]                DATETIME2        NOT NULL,
    [ModifiedBySysUserId]       INT              NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ReplrParticipation] PRIMARY KEY NONCLUSTERED ([SchoolYear], [ReplrParticipationId]),
    CONSTRAINT [UK_ReplrParticipation] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [ReplrParticipationId]),

    CONSTRAINT [FK_ReplrParticipation_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ReplrParticipation_ReplrParticipationType] FOREIGN KEY ([ReplrParticipationTypeId]) REFERENCES [school_books].[ReplrParticipationType] ([Id]),
    CONSTRAINT [FK_ReplrParticipation_Institution] FOREIGN KEY ([InstId]) REFERENCES [core].[Institution] ([InstitutionID]),
    CONSTRAINT [FK_ReplrParticipation_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ReplrParticipation_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

SET IDENTITY_INSERT [school_books].[ReplrParticipationType] ON;

INSERT INTO [school_books].[ReplrParticipationType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Оценка от екип за подкрепа на личностното развитие (ЕПЛР) в детската градина/училището', 'Оценка от екип за подкрепа на личностното развитие (ЕПЛР) в детската градина/училището', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Оценка на деца и ученици със специални образователни потребности при невъзможност за формиране на ЕПЛР в детската градина/училището', 'Оценка на деца и ученици със специални образователни потребности при невъзможност за формиране на ЕПЛР в детската градина/училището', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Повторна оценка при несъгласие на родителя с оценката от ЕПЛР', 'Повторна оценка при несъгласие на родителя с оценката от ЕПЛР', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'Оценка за насочване за обучение в специалните училища', 'Оценка за насочване за обучение в специалните училища', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'Оценка за насочване за обучение в училища с подкрепа на ЦСОП', 'Оценка за насочване за обучение в училища с подкрепа на ЦСОП', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'Методическа подкрепа', 'На международно ниво', 1, '2021-01-01 00:00:00.000', NULL),
(7, 7, 'Преценка за обучението на ученик, настанен в болница', 'Преценка за обучението на ученик, настанен в болница', 1, '2021-01-01 00:00:00.000', NULL),
(8, 8, 'Оценка на деца със специални образователни потребности във връзка с чл.14 от Наредба за прием на деца в общинските детски градини на територията на Столична община', 'Оценка на деца със специални образователни потребности във връзка с чл.14 от Наредба за прием на деца в общинските детски градини на територията на Столична община', 1, '2021-01-01 00:00:00.000', NULL),
(9, 9, 'Други дейности', 'Други дейности', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[ReplrParticipationType] OFF;

GO

