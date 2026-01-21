PRINT 'Create SchoolYearSettingsClassBook table'
GO

CREATE TABLE [school_books].[SchoolYearSettingsClassBook] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SchoolYearSettingsId]  INT              NOT NULL,
    [ClassBookId]           INT              NOT NULL,

    CONSTRAINT [PK_SchoolYearSettingsClassBook] PRIMARY KEY ([SchoolYear], [SchoolYearSettingsId], [ClassBookId]),

    CONSTRAINT [FK_SchoolYearSettingsClassBook_SchoolYearSettings] FOREIGN KEY ([SchoolYear], [SchoolYearSettingsId]) REFERENCES [school_books].[SchoolYearSettings] ([SchoolYear], [SchoolYearSettingsId]),
    CONSTRAINT [FK_SchoolYearSettingsClassBook_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
);
GO

exec school_books.spDescTable  N'SchoolYearSettingsClassBook', N'Настройки на учебната година - дневник.'

exec school_books.spDescColumn N'SchoolYearSettingsClassBook', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SchoolYearSettingsClassBook', N'SchoolYearSettingsId'      , N'Идентификатор на настройки на учебната година.'
exec school_books.spDescColumn N'SchoolYearSettingsClassBook', N'ClassBookId'               , N'Идентификатор на дневник.'
