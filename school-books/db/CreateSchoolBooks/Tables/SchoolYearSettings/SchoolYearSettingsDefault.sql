PRINT 'Create SchoolYearSettingsDefault table'
GO

CREATE TABLE [school_books].[SchoolYearSettingsDefault] (
    [SchoolYear]                    SMALLINT    NOT NULL,

    [PgSchoolYearStartDateLimit]    DATE        NOT NULL,
    [PgSchoolYearStartDate]         DATE        NOT NULL,
    [PgFirstTermEndDate]            DATE        NOT NULL,
    [PgSecondTermStartDate]         DATE        NOT NULL,
    [PgSchoolYearEndDate]           DATE        NOT NULL,
    [PgSchoolYearEndDateLimit]      DATE        NOT NULL,

    [SportSchoolYearStartDateLimit] DATE        NOT NULL,
    [SportSchoolYearStartDate]      DATE        NOT NULL,
    [SportFirstTermEndDate]         DATE        NOT NULL,
    [SportSecondTermStartDate]      DATE        NOT NULL,
    [SportSchoolYearEndDate]        DATE        NOT NULL,
    [SportSchoolYearEndDateLimit]   DATE        NOT NULL,

    [CplrSchoolYearStartDateLimit]  DATE        NOT NULL,
    [CplrSchoolYearStartDate]       DATE        NOT NULL,
    [CplrFirstTermEndDate]          DATE        NOT NULL,
    [CplrSecondTermStartDate]       DATE        NOT NULL,
    [CplrSchoolYearEndDate]         DATE        NOT NULL,
    [CplrSchoolYearEndDateLimit]    DATE        NOT NULL,

    [OtherSchoolYearStartDateLimit] DATE        NOT NULL,
    [OtherSchoolYearStartDate]      DATE        NOT NULL,
    [OtherFirstTermEndDate]         DATE        NOT NULL,
    [OtherSecondTermStartDate]      DATE        NOT NULL,
    [OtherSchoolYearEndDate]        DATE        NOT NULL,
    [OtherSchoolYearEndDateLimit]   DATE        NOT NULL,

    CONSTRAINT [PK_SchoolYearSettingsDefault] PRIMARY KEY ([SchoolYear])
);
GO

exec school_books.spDescTable  N'SchoolYearSettingsDefault', N'Настройки на учебната година по подразбиране за различните типове училища.'

exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SchoolYear'                      , N'Учебна година.'

exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgSchoolYearStartDateLimit'      , N'Лимит за начало на учебната година за ПГ училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgSchoolYearStartDate'           , N'Начало на учебната година за ПГ училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgFirstTermEndDate'              , N'Край на първи срок за ПГ училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgSecondTermStartDate'           , N'Начало на втори срок за ПГ училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgSchoolYearEndDate'             , N'Край на учебната година за ПГ училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'PgSchoolYearEndDateLimit'        , N'Лимит за край на учебната година за ПГ училища.'

exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportSchoolYearStartDateLimit'   , N'Лимит за начало на учебната година за спортни училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportSchoolYearStartDate'        , N'Начало на учебната година за спортни училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportFirstTermEndDate'           , N'Край на първи срок за спортни училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportSecondTermStartDate'        , N'Начало на втори срок за спортни училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportSchoolYearEndDate'          , N'Край на учебната година за спортни училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'SportSchoolYearEndDateLimit'     , N'Лимит за край на учебната година за спортни училища.'

exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrSchoolYearStartDateLimit'    , N'Лимит за начало на учебната година за ЦПЛР.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrSchoolYearStartDate'         , N'Начало на учебната година за ЦПЛР.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrFirstTermEndDate'            , N'Край на първи срок за ЦПЛР.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrSecondTermStartDate'         , N'Начало на втори срок за ЦПЛР.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrSchoolYearEndDate'           , N'Край на учебната година за ЦПЛР.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'CplrSchoolYearEndDateLimit'      , N'Лимит за край на учебната година за ЦПЛР.'

exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherSchoolYearStartDateLimit'   , N'Лимит за начало на учебната година за други училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherSchoolYearStartDate'        , N'Начало на учебната година за други училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherFirstTermEndDate'           , N'Край на първи срок за други училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherSecondTermStartDate'        , N'Начало на втори срок за други училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherSchoolYearEndDate'          , N'Край на учебната година за други училища.'
exec school_books.spDescColumn N'SchoolYearSettingsDefault', N'OtherSchoolYearEndDateLimit'     , N'Лимит за край на учебната година за други училища.'
