PRINT 'Create SchoolYearSettingsClass table'
GO

CREATE TABLE [school_books].[SchoolYearSettingsClass] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [SchoolYearSettingsId]  INT              NOT NULL,
    [BasicClassId]          INT              NOT NULL,

    CONSTRAINT [PK_SchoolYearSettingsClass] PRIMARY KEY ([SchoolYear], [SchoolYearSettingsId], [BasicClassId]),

    CONSTRAINT [FK_SchoolYearSettingsClass_SchoolYearSettings] FOREIGN KEY ([SchoolYear], [SchoolYearSettingsId]) REFERENCES [school_books].[SchoolYearSettings] ([SchoolYear], [SchoolYearSettingsId]),
    CONSTRAINT [FK_SchoolYearSettingsClass_BasicClass] FOREIGN KEY ([BasicClassId]) REFERENCES [inst_nom].[BasicClass] ([BasicClassId]),
);
GO

exec school_books.spDescTable  N'SchoolYearSettingsClass', N'Настройки на учебната година - клас.'

exec school_books.spDescColumn N'SchoolYearSettingsClass', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'SchoolYearSettingsClass', N'SchoolYearSettingsId'      , N'Идентификатор на настройки на учебната година.'
exec school_books.spDescColumn N'SchoolYearSettingsClass', N'BasicClassId'              , N'Идентификатор на випуска. Номенклатура inst_nom.BasicClass.'
