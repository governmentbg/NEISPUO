PRINT 'Create Exam table'
GO

EXEC [school_books].[spCreateIdSequence] N'Exam'
GO

CREATE TABLE [school_books].[Exam] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ExamId]                INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [Type]                  INT              NOT NULL,
    [CurriculumId]          INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Description]           NVARCHAR(1000)   NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Exam] PRIMARY KEY NONCLUSTERED ([SchoolYear], [ExamId]),
    CONSTRAINT [UK_Exam] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [ExamId]),

    CONSTRAINT [FK_Exam_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    CONSTRAINT [CHK_Exam_Type] CHECK ([Type] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_Exam_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_Exam_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Exam_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_Exam_CurriculumId] ([CurriculumId] ASC),
);
GO

exec school_books.spDescTable  N'Exam', N'Класна/контролна работа.'

exec school_books.spDescColumn N'Exam', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Exam', N'ExamId'                    , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Exam', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Exam', N'Type'                      , N'Вид - класна/контролна работа.'
exec school_books.spDescColumn N'Exam', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'Exam', N'Date'                      , N'Дата на класната/контролната работа.'
exec school_books.spDescColumn N'Exam', N'Description'               , N'Описание.'

exec school_books.spDescColumn N'Exam', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Exam', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Exam', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Exam', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Exam', N'Version'                   , N'Версия.'
