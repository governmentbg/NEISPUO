PRINT 'Create ClassBook table'
GO

EXEC [school_books].[spCreateIdSequence] N'ClassBook'
GO

CREATE TABLE [school_books].[ClassBook] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ClassBookId]           INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [ClassId]               INT              NOT NULL,
    [ClassIsLvl2]           BIT              NOT NULL,
    [BookType]              INT              NOT NULL,
    [BasicClassId]          INT              NULL,
    [BasicClassName]        NVARCHAR(255)    NULL,
    [BookName]              NVARCHAR(300)    NOT NULL,
    [SchoolYearProgram]     NVARCHAR(MAX)    NULL,
    [IsFinalized]           BIT              NOT NULL,
    [IsValid]               BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    [FullBookName] AS (
        CASE WHEN COALESCE(LEN([BookName]), 0) > 0
                AND COALESCE(LEN([BasicClassName]), 0) > 0
            THEN [BasicClassName] + ' - ' + [BookName]
            ELSE COALESCE([BasicClassName], [BookName])
        END
    ),

    CONSTRAINT [PK_ClassBook] PRIMARY KEY ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBook_BookType] CHECK ([BookType] IN (1, 2, 3, 4, 5, 6, 7)),

    -- external references
    CONSTRAINT [FK_ClassBook_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ClassBook_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_ClassBook_BasicClassId] FOREIGN KEY ([BasicClassId]) REFERENCES [inst_nom].[BasicClass] ([BasicClassID]),
    CONSTRAINT [FK_ClassBook_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ClassBook_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ClassBook] UNIQUE ([SchoolYear], [InstId], [ClassBookId]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ClassBook_ClassId] ([ClassId] ASC),
);
GO

CREATE UNIQUE INDEX UQ_ClassBook_SchoolYear_ClassId ON [school_books].[ClassBook] (SchoolYear, ClassId) WHERE IsValid = 1;
GO

exec school_books.spDescTable  N'ClassBook', N'Дневник.'

exec school_books.spDescColumn N'ClassBook', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBook', N'ClassBookId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ClassBook', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'ClassBook', N'ClassId'                   , N'Идентификатор на група/паралелка.'
exec school_books.spDescColumn N'ClassBook', N'ClassIsLvl2'               , N'Дневника е за група от слята паралелка – да/не.'
exec school_books.spDescColumn N'ClassBook', N'BookType'                  , N'Вид на дневника.'
exec school_books.spDescColumn N'ClassBook', N'BasicClassId'              , N'Идентификатор на випуска. Номенклатура inst_nom.BasicClass.'
exec school_books.spDescColumn N'ClassBook', N'BasicClassName'            , N'Наименование на випуска.'
exec school_books.spDescColumn N'ClassBook', N'BookName'                  , N'Наименование в менюто.'
exec school_books.spDescColumn N'ClassBook', N'FullBookName'              , N'Пълно наименование на паралелката/дневника.'
exec school_books.spDescColumn N'ClassBook', N'SchoolYearProgram'         , N'Годишна програма.'
exec school_books.spDescColumn N'ClassBook', N'IsFinalized'               , N'Дневника е приключен - да/не.'
exec school_books.spDescColumn N'ClassBook', N'IsValid'                   , N'Дневника е архивиран - да/не.'

exec school_books.spDescColumn N'ClassBook', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ClassBook', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ClassBook', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ClassBook', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'ClassBook', N'Version'                   , N'Версия.'
