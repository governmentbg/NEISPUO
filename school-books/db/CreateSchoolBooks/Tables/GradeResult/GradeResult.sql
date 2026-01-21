PRINT 'Create GradeResult table'
GO

EXEC [school_books].[spCreateIdSequence] N'GradeResult'
GO

CREATE TABLE [school_books].[GradeResult] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [GradeResultId]         INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [InitialResultType]     INT              NOT NULL,
    [FinalResultType]       INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_GradeResult] PRIMARY KEY NONCLUSTERED ([SchoolYear], [GradeResultId]),
    CONSTRAINT [UK_GradeResult] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [GradeResultId]),
    CONSTRAINT [UK_GradeResult_ClassBookId_PersonId] UNIQUE ([SchoolYear], [ClassBookId], [PersonId]),

    CONSTRAINT [FK_GradeResult_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_GradeResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_GradeResult_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_GradeResult_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'GradeResult', N'Годишен резултат.'

exec school_books.spDescColumn N'GradeResult', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'GradeResult', N'GradeResultId'             , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'GradeResult', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'GradeResult', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'GradeResult', N'InitialResultType'         , N'Годишен резултат. 1 - Завършва, 2 - Полага изпити, 3 - Повтаря'
exec school_books.spDescColumn N'GradeResult', N'FinalResultType'           , N'Годишен резултат след сесиите. Попълва се само ако годишния резултат е "Полага изпити".'

exec school_books.spDescColumn N'GradeResult', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'GradeResult', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'GradeResult', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'GradeResult', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'GradeResult', N'Version'                   , N'Версия.'
