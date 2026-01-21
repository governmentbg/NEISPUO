PRINT 'Create IndividualWork table'
GO

EXEC [school_books].[spCreateIdSequence] N'IndividualWork'
GO

CREATE TABLE [school_books].[IndividualWork] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [IndividualWorkId]          INT              NOT NULL,

    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [Date]                      DATETIME2        NOT NULL,
    [IndividualWorkActivity]    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [ModifyDate]                DATETIME2        NOT NULL,
    [ModifiedBySysUserId]       INT              NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_IndividualWork] PRIMARY KEY NONCLUSTERED ([SchoolYear], [IndividualWorkId]),
    CONSTRAINT [UK_IndividualWork] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [IndividualWorkId]),

    CONSTRAINT [FK_IndividualWork_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_IndividualWork_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_IndividualWork_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_IndividualWork_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'IndividualWork', N'Индивидуална работа.'

exec school_books.spDescColumn N'IndividualWork', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'IndividualWork', N'IndividualWorkId'          , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'IndividualWork', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'IndividualWork', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'IndividualWork', N'Date'                      , N'Дата.'
exec school_books.spDescColumn N'IndividualWork', N'IndividualWorkActivity'    , N'Проведена индивидуална работа с ученика.'

exec school_books.spDescColumn N'IndividualWork', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'IndividualWork', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'IndividualWork', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'IndividualWork', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'IndividualWork', N'Version'                   , N'Версия.'
