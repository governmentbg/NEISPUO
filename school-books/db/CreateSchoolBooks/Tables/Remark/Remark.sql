PRINT 'Create Remark table'
GO

EXEC [school_books].[spCreateIdSequence] N'Remark'
GO

CREATE TABLE [school_books].[Remark] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [RemarkId]              INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [Type]                  INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [CurriculumId]          INT              NOT NULL,
    [Description]           NVARCHAR(1000)   NOT NULL,
    [IsReadFromParent]      BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Remark] PRIMARY KEY NONCLUSTERED ([SchoolYear], [RemarkId]),
    CONSTRAINT [UK_Remark] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [RemarkId]),

    CONSTRAINT [FK_Remark_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    CONSTRAINT [CHK_Remark_Type] CHECK ([Type] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_Remark_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Remark_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_Remark_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Remark_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    -- Curriculum deletetion helper index
    INDEX [IX_Remark_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'Remark', N'Отзив.'

exec school_books.spDescColumn N'Remark', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Remark', N'RemarkId'                  , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Remark', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Remark', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Remark', N'Type'                      , N'Вид на отзива. 1 - Похвала, 2 - Забележка.'
exec school_books.spDescColumn N'Remark', N'Date'                      , N'Дата на отзива.'
exec school_books.spDescColumn N'Remark', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'Remark', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'Remark', N'IsReadFromParent'          , N'Прочетено от родител – да/не.'

exec school_books.spDescColumn N'Remark', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Remark', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Remark', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Remark', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Remark', N'Version'                   , N'Версия.'
