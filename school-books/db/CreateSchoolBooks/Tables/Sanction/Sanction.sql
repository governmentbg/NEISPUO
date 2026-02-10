PRINT 'Create Sanction table'
GO

EXEC [school_books].[spCreateIdSequence] N'Sanction'
GO

CREATE TABLE [school_books].[Sanction] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SanctionId]            INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [SanctionTypeId]        INT              NOT NULL,
    [OrderNumber]           NVARCHAR(100)    NOT NULL,
    [OrderDate]             DATE             NOT NULL,
    [StartDate]             DATE             NOT NULL,
    [EndDate]               DATE             NULL,
    [Description]           NVARCHAR(1000)   NULL,
    [CancelOrderNumber]     NVARCHAR(100)    NULL,
    [CancelOrderDate]       DATE             NULL,
    [CancelReason]          NVARCHAR(1000)   NULL,
    [RuoOrderNumber]        NVARCHAR(100)    NULL,
    [RuoOrderDate]          DATE             NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Sanction] PRIMARY KEY NONCLUSTERED ([SchoolYear], [SanctionId]),
    CONSTRAINT [UK_Sanction] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [SanctionId]),

    CONSTRAINT [FK_Sanction_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_Sanction_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Sanction_SanctionType] FOREIGN KEY ([SanctionTypeId]) REFERENCES [student].[SanctionType] ([Id]),
    CONSTRAINT [FK_Sanction_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Sanction_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Sanction', N'Санкция.'

exec school_books.spDescColumn N'Sanction', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Sanction', N'SanctionId'                , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Sanction', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Sanction', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Sanction', N'SanctionTypeId'            , N'Вид на санкцията. Номенклатура student.SanctionType.'
exec school_books.spDescColumn N'Sanction', N'OrderNumber'               , N'Номер на заповед.'
exec school_books.spDescColumn N'Sanction', N'OrderDate'                 , N'Дата на заповед.'
exec school_books.spDescColumn N'Sanction', N'StartDate'                 , N'Начална дата.'
exec school_books.spDescColumn N'Sanction', N'EndDate'                   , N'Крайна дата.'
exec school_books.spDescColumn N'Sanction', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'Sanction', N'CancelOrderNumber'         , N'Номер на заповед за отмяна.'
exec school_books.spDescColumn N'Sanction', N'CancelOrderDate'           , N'Дата на заповед за отмяна.'
exec school_books.spDescColumn N'Sanction', N'CancelReason'              , N'Причина за заповедта за отмяна.'
exec school_books.spDescColumn N'Sanction', N'RuoOrderNumber'            , N'Номер на заповед на началника на РУО.'
exec school_books.spDescColumn N'Sanction', N'RuoOrderDate'              , N'Дата на заповед на началника на РУО.'

exec school_books.spDescColumn N'Sanction', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Sanction', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Sanction', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Sanction', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Sanction', N'Version'                   , N'Версия.'
