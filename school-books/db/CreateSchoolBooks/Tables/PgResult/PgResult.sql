PRINT 'Create PgResult table'
GO

EXEC [school_books].[spCreateIdSequence] N'PgResult'
GO

CREATE TABLE [school_books].[PgResult] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [PgResultId]                INT              NOT NULL,

    [ClassBookId]               INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [SubjectId]                 INT              NULL,
    [CurriculumId]              INT              NULL,
    [StartSchoolYearResult]     NVARCHAR(MAX)    NULL,
    [EndSchoolYearResult]       NVARCHAR(MAX)    NULL,

    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [ModifyDate]                DATETIME2        NOT NULL,
    [ModifiedBySysUserId]       INT              NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_PgResult] PRIMARY KEY NONCLUSTERED ([SchoolYear], [PgResultId]),
    CONSTRAINT [UK_PgResult] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [PgResultId]),
    CONSTRAINT [UK_PgResult_SchoolYear_ClassBookId_PersonId_SubjectId] UNIQUE ([SchoolYear], [ClassBookId], [PersonId], [SubjectId]),

    CONSTRAINT [FK_PgResult_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_PgResult_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_PgResult_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_PgResult_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_PgResult_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_PgResult_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    -- Curriculum deletetion helper indexes
    INDEX [IX_PgResult_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'PgResult', N'Резултат от предучилищната подготовка по направления.'

exec school_books.spDescColumn N'PgResult', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'PgResult', N'PgResultId'                , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'PgResult', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'PgResult', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'PgResult', N'SubjectId'                 , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'PgResult', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'PgResult', N'StartSchoolYearResult'     , N'Секция "В началото на учебната година".'
exec school_books.spDescColumn N'PgResult', N'EndSchoolYearResult'       , N'Секция "В края на учебната година".'

exec school_books.spDescColumn N'PgResult', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'PgResult', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'PgResult', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'PgResult', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'PgResult', N'Version'                   , N'Версия.'
