PRINT 'Create Note table'
GO

EXEC [school_books].[spCreateIdSequence] N'Note'
GO

CREATE TABLE [school_books].[Note] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [NoteId]                INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [Description]           NVARCHAR(MAX)    NOT NULL,
    [IsForAllStudents]      BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Note] PRIMARY KEY NONCLUSTERED ([SchoolYear], [NoteId]),
    CONSTRAINT [UK_Note] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [NoteId]),

    CONSTRAINT [FK_Note_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_Note_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Note_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Note', N'Бележка.'

exec school_books.spDescColumn N'Note', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Note', N'NoteId'                    , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Note', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Note', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'Note', N'IsForAllStudents'          , N'Бележката важи за всички ученици от групата - да/не.'

exec school_books.spDescColumn N'Note', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Note', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Note', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Note', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Note', N'Version'                   , N'Версия.'
