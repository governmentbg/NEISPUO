PRINT 'Create FirstGradeResult table'
GO

EXEC [school_books].[spCreateIdSequence] N'FirstGradeResult'
GO

CREATE TABLE [school_books].[FirstGradeResult] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [FirstGradeResultId]    INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [QualitativeGrade]      INT              NULL,
    [SpecialGrade]          INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_FirstGradeResult] PRIMARY KEY NONCLUSTERED ([SchoolYear], [FirstGradeResultId]),
    CONSTRAINT [UK_FirstGradeResult] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [FirstGradeResultId]),
    CONSTRAINT [UK_FirstGradeResult_ClassBookId_PersonId] UNIQUE ([SchoolYear], [ClassBookId], [PersonId]),

    CONSTRAINT [FK_FirstGradeResult_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_FirstGradeResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_FirstGradeResult_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_FirstGradeResult_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'FirstGradeResult', N'Общ годишен успех за първи клас.'

exec school_books.spDescColumn N'FirstGradeResult', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'FirstGradeResult', N'FirstGradeResultId'        , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'FirstGradeResult', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'FirstGradeResult', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'FirstGradeResult', N'QualitativeGrade'          , N'Качествена оценка.'
exec school_books.spDescColumn N'FirstGradeResult', N'SpecialGrade'              , N'СОП оценка.'

exec school_books.spDescColumn N'FirstGradeResult', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'FirstGradeResult', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'FirstGradeResult', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'FirstGradeResult', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'FirstGradeResult', N'Version'                   , N'Версия.'
