GO

ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ADD
    [CplrSchoolYearStartDateLimit] DATE NULL,
    [CplrSchoolYearStartDate]      DATE NULL,
    [CplrFirstTermEndDate]         DATE NULL,
    [CplrSecondTermStartDate]      DATE NULL,
    [CplrSchoolYearEndDate]        DATE NULL,
    [CplrSchoolYearEndDateLimit]   DATE NULL;
GO

UPDATE sysd
SET
    CplrSchoolYearStartDateLimit = cplrDefaults.CplrSchoolYearStartDateLimit,
    CplrSchoolYearStartDate = cplrDefaults.CplrSchoolYearStartDate,
    CplrFirstTermEndDate = cplrDefaults.CplrFirstTermEndDate,
    CplrSecondTermStartDate = cplrDefaults.CplrSecondTermStartDate,
    CplrSchoolYearEndDate = cplrDefaults.CplrSchoolYearEndDate,
    CplrSchoolYearEndDateLimit = cplrDefaults.CplrSchoolYearEndDateLimit
FROM [school_books].[SchoolYearSettingsDefault] sysd
JOIN (
    VALUES (
        2021,
        CONVERT(DATE, '2021-09-15'),
        [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-10-01')),
        CONVERT(DATE, '2022-01-31'),
        CONVERT(DATE, '2022-02-02'),
        CONVERT(DATE, '2022-09-30'),
        CONVERT(DATE, '2022-09-30')
    ), (
        2022,
        CONVERT(DATE, '2022-09-15'),
        [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-10-01')),
        CONVERT(DATE, '2023-01-31'),
        CONVERT(DATE, '2023-02-06'),
        CONVERT(DATE, '2023-09-30'),
        CONVERT(DATE, '2023-09-30')
    ), (
        2023,
        CONVERT(DATE, '2023-09-15'),
        [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-10-01')),
        CONVERT(DATE, '2024-01-31'),
        CONVERT(DATE, '2024-02-06'),
        CONVERT(DATE, '2024-09-30'),
        CONVERT(DATE, '2024-09-30')
    )) AS cplrDefaults(
        [SchoolYear],
        [CplrSchoolYearStartDateLimit],
        [CplrSchoolYearStartDate],
        [CplrFirstTermEndDate],
        [CplrSecondTermStartDate],
        [CplrSchoolYearEndDate],
        [CplrSchoolYearEndDateLimit]
    ) ON sysd.SchoolYear = cplrDefaults.SchoolYear
WHERE sysd.SchoolYear = cplrDefaults.SchoolYear
GO

ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrSchoolYearStartDateLimit] DATE NOT NULL;
ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrSchoolYearStartDate] DATE NOT NULL;
ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrFirstTermEndDate] DATE NOT NULL;
ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrSecondTermStartDate] DATE NOT NULL;
ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrSchoolYearEndDate] DATE NOT NULL;
ALTER TABLE [school_books].[SchoolYearSettingsDefault]
ALTER COLUMN [CplrSchoolYearEndDateLimit] DATE NOT NULL;
GO
