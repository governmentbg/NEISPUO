PRINT 'Create ClassBookSchoolYearSettings table'
GO

CREATE TABLE [school_books].[ClassBookSchoolYearSettings] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,

    [SchoolYearSettingsId]      INT              NULL,
    [SchoolYearStartDateLimit]  DATE             NOT NULL,
    [SchoolYearStartDate]       DATE             NOT NULL,
    [FirstTermEndDate]          DATE             NOT NULL,
    [SecondTermStartDate]       DATE             NOT NULL,
    [SchoolYearEndDate]         DATE             NOT NULL,
    [SchoolYearEndDateLimit]    DATE             NOT NULL,
    [HasFutureEntryLock]        BIT              NOT NULL,
    [PastMonthLockDay]          INT              NULL,

    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ClassBookSchoolYearSettings] PRIMARY KEY ([SchoolYear], [ClassBookId]),

    CONSTRAINT [FK_ClassBookSchoolYearSettings_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_ClassBookSchoolYearSettings_SchoolYearSettings] FOREIGN KEY ([SchoolYear], [SchoolYearSettingsId]) REFERENCES [school_books].[SchoolYearSettings] ([SchoolYear], [SchoolYearSettingsId]),
);
GO

exec school_books.spDescTable  N'ClassBookSchoolYearSettings', N'Настройки на учебната година за дневник.'

exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYear'                    , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'ClassBookId'                   , N'Идентификатор на дневник.'

exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYearSettingsId'          , N'Идентификатор на настройки на учебната година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYearStartDateLimit'      , N'Лимит за начало на учебната година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYearStartDate'           , N'Начало на учебната година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'FirstTermEndDate'              , N'Край на първи срок.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SecondTermStartDate'           , N'Начало на втори срок.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYearEndDate'             , N'Край на учебната година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'SchoolYearEndDateLimit'        , N'Лимит за край на учебната година.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'HasFutureEntryLock'            , N'Забрана за въвеждане на оценки/отсъствия/теми в бъдеще време - Да/Не.'
exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'PastMonthLockDay'              , N'Ден от месеца, след който се забранява въвеждането и редакцията на оценки/отсъствия/теми в предходни месеци.'

exec school_books.spDescColumn N'ClassBookSchoolYearSettings', N'Version'                       , N'Версия.'
