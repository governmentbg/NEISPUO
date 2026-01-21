PRINT 'Create Publication table'
GO

EXEC [school_books].[spCreateIdSequence] N'Publication'
GO

CREATE TABLE [school_books].[Publication] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [PublicationId]           INT              NOT NULL,

    [InstId]                  INT              NOT NULL,
    [Type]                    INT              NOT NULL,
    [Status]                  INT              NOT NULL,
    [Date]                    DATE             NOT NULL,
    [Title]                   NVARCHAR(100)    NOT NULL,
    [Content]                 NVARCHAR(1000)   NOT NULL,

    [CreateDate]              DATETIME2        NOT NULL,
    [CreatedBySysUserId]      INT              NOT NULL,
    [ModifyDate]              DATETIME2        NOT NULL,
    [ModifiedBySysUserId]     INT              NOT NULL,
    [Version]                 ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Publication] PRIMARY KEY ([SchoolYear], [PublicationId]),

    CONSTRAINT [CHK_Publication_Type] CHECK ([Type] IN (1, 2)),
    CONSTRAINT [CHK_Publication_Status] CHECK ([Status] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_Publication_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_Publication_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Publication_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Publication', N'Публикация.'

exec school_books.spDescColumn N'Publication', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Publication', N'PublicationId'             , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Publication', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'Publication', N'Type'                      , N'Тип на публикацията. 1 - Вътрешна, 2 - Публична.'
exec school_books.spDescColumn N'Publication', N'Status'                    , N'Статус. 1 - Чернова, 2 - Публикувана, 3 - Архивирана.'
exec school_books.spDescColumn N'Publication', N'Date'                      , N'Дата на публикуване.'
exec school_books.spDescColumn N'Publication', N'Title'                     , N'Заглавие.'
exec school_books.spDescColumn N'Publication', N'Content'                   , N'Описание.'

exec school_books.spDescColumn N'Publication', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Publication', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Publication', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Publication', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Publication', N'Version'                   , N'Версия.'
