PRINT 'Create ReplrParticipation table'
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

exec school_books.spDescTable  N'ReplrParticipation', N'Участие в РЕПЛР.'

exec school_books.spDescColumn N'ReplrParticipation', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ReplrParticipation', N'ReplrParticipationId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ReplrParticipation', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ReplrParticipation', N'ReplrParticipationTypeId'  , N'Вид на изявата. Номенклатура ReplrParticipationType.'
exec school_books.spDescColumn N'ReplrParticipation', N'Topic'                     , N'Тема.'
exec school_books.spDescColumn N'ReplrParticipation', N'Date'                      , N'Дата.'
exec school_books.spDescColumn N'ReplrParticipation', N'Attendees'                 , N'Присъстващи.'
exec school_books.spDescColumn N'ReplrParticipation', N'InstId'                    , N'Идентификатор на институцията.'

exec school_books.spDescColumn N'ReplrParticipation', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ReplrParticipation', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ReplrParticipation', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ReplrParticipation', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'ReplrParticipation', N'Version'                   , N'Версия.'
