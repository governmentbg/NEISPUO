PRINT 'Create SchoolYearSettings table'
GO

EXEC [school_books].[spCreateIdSequence] N'SchoolYearSettings'
GO

CREATE TABLE [school_books].[SchoolYearSettings] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SchoolYearSettingsId]  INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [SchoolYearStartDate]   DATE             NULL,
    [FirstTermEndDate]      DATE             NULL,
    [SecondTermStartDate]   DATE             NULL,
    [SchoolYearEndDate]     DATE             NULL,
    [Description]           NVARCHAR(100)    NOT NULL,
    [HasFutureEntryLock]    BIT              NOT NULL,
    [PastMonthLockDay]      INT              NULL,
    [IsForAllClasses]       BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SchoolYearSettings] PRIMARY KEY ([SchoolYear], [SchoolYearSettingsId]),
    CONSTRAINT [CHK_SchoolYearSettings_PastMonthLockDay] CHECK ([PastMonthLockDay] BETWEEN 1 AND 31),

    -- external references
    CONSTRAINT [FK_SchoolYearSettings_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SchoolYearSettings_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SchoolYearSettings_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SchoolYearSettings] UNIQUE ([SchoolYear], [InstId], [SchoolYearSettingsId]),
    INDEX [UQ_SchoolYearSettings_SchoolYear_InstId_IsForAllClasses] ([SchoolYear], [InstId])
        INCLUDE ([SchoolYearSettingsId])
        WHERE [IsForAllClasses] = 1,
);
GO

exec school_books.spDescTable  N'SchoolYearSettings', N'Настройки на учебната година.'

exec school_books.spDescColumn N'SchoolYearSettings', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SchoolYearSettings', N'SchoolYearSettingsId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SchoolYearSettings', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'SchoolYearSettings', N'SchoolYearStartDate'       , N'Начало на учебната година'
exec school_books.spDescColumn N'SchoolYearSettings', N'FirstTermEndDate'          , N'Край на първи срок.'
exec school_books.spDescColumn N'SchoolYearSettings', N'SecondTermStartDate'       , N'Начало на втори срок.'
exec school_books.spDescColumn N'SchoolYearSettings', N'SchoolYearEndDate'         , N'Край на учебната година.'
exec school_books.spDescColumn N'SchoolYearSettings', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'SchoolYearSettings', N'HasFutureEntryLock'        , N'Забрана за въвеждане на оценки/отсъствия/теми в бъдеще време - Да/Не.'
exec school_books.spDescColumn N'SchoolYearSettings', N'PastMonthLockDay'          , N'Ден от месеца, след който се забранява въвеждането и редакцията на оценки/отсъствия/теми в предходни месеци.'
exec school_books.spDescColumn N'SchoolYearSettings', N'IsForAllClasses'           , N'Настройките важат за всички класове – Да/Не.'

exec school_books.spDescColumn N'SchoolYearSettings', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'SchoolYearSettings', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'SchoolYearSettings', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'SchoolYearSettings', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'SchoolYearSettings', N'Version'                   , N'Версия.'
