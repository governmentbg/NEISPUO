PRINT 'Create Support table'
GO

EXEC [school_books].[spCreateIdSequence] N'Support'
GO

CREATE TABLE [school_books].[Support] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SupportId]             INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [EndDate]               DATE             NOT NULL,
    [Description]           NVARCHAR(MAX)    NULL,
    [ExpectedResult]        NVARCHAR(MAX)    NULL,
    [IsForAllStudents]      BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Support] PRIMARY KEY NONCLUSTERED ([SchoolYear], [SupportId]),
    CONSTRAINT [UK_Support] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [SupportId]),

    CONSTRAINT [FK_Support_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_Support_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Support_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Support', N'Подкрепа.'

exec school_books.spDescColumn N'Support', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Support', N'SupportId'                 , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Support', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Support', N'EndDate'                   , N'Краен срок.'
exec school_books.spDescColumn N'Support', N'Description'               , N'Описание на проблема.'
exec school_books.spDescColumn N'Support', N'ExpectedResult'            , N'Очакван резултат.'
exec school_books.spDescColumn N'Support', N'IsForAllStudents'          , N'Подкрепата важи за всички ученици от групата – Да/Не.'

exec school_books.spDescColumn N'Support', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Support', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Support', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Support', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Support', N'Version'                   , N'Версия.'
