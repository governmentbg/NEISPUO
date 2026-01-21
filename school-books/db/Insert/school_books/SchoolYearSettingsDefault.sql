GO
PRINT 'Insert SchoolYearSettingsDefault'
GO

INSERT INTO [school_books].[SchoolYearSettingsDefault] (
    [SchoolYear],
    [PgSchoolYearStartDateLimit],
    [PgSchoolYearStartDate],
    [PgFirstTermEndDate],
    [PgSecondTermStartDate],
    [PgSchoolYearEndDate],
    [PgSchoolYearEndDateLimit],
    [SportSchoolYearStartDateLimit],
    [SportSchoolYearStartDate],
    [SportFirstTermEndDate],
    [SportSecondTermStartDate],
    [SportSchoolYearEndDate],
    [SportSchoolYearEndDateLimit],
    [CplrSchoolYearStartDateLimit],
    [CplrSchoolYearStartDate],
    [CplrFirstTermEndDate],
    [CplrSecondTermStartDate],
    [CplrSchoolYearEndDate],
    [CplrSchoolYearEndDateLimit],
    [OtherSchoolYearStartDateLimit],
    [OtherSchoolYearStartDate],
    [OtherFirstTermEndDate],
    [OtherSecondTermStartDate],
    [OtherSchoolYearEndDate],
    [OtherSchoolYearEndDateLimit]
)
VALUES (
    2021,
    -- Pg
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-15')),
    CONVERT(DATE, '2022-01-31'),
    CONVERT(DATE, '2022-02-02'),
    CONVERT(DATE, '2022-05-31'),
    CONVERT(DATE, '2022-09-14'),
    -- Sport
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-01')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-01')),
    CONVERT(DATE, '2022-01-31'),
    CONVERT(DATE, '2022-02-02'),
    CONVERT(DATE, '2022-06-30'),
    CONVERT(DATE, '2022-08-31'),
    -- Cplr
    CONVERT(DATE, '2021-09-15'),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-10-01')),
    CONVERT(DATE, '2022-01-31'),
    CONVERT(DATE, '2022-02-02'),
    CONVERT(DATE, '2022-09-30'),
    CONVERT(DATE, '2022-09-30'),
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2021-09-15')),
    CONVERT(DATE, '2022-01-31'),
    CONVERT(DATE, '2022-02-02'),
    CONVERT(DATE, '2022-06-30'),
    CONVERT(DATE, '2022-09-14')
), (
    2022,
    -- Pg
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-15')),
    CONVERT(DATE, '2023-01-31'),
    CONVERT(DATE, '2023-02-06'),
    CONVERT(DATE, '2023-05-31'),
    CONVERT(DATE, '2023-09-14'),
    -- Sport
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-01')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-01')),
    CONVERT(DATE, '2023-01-31'),
    CONVERT(DATE, '2023-02-06'),
    CONVERT(DATE, '2023-06-30'),
    CONVERT(DATE, '2023-08-31'),
    -- Cplr
    CONVERT(DATE, '2022-09-15'),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-10-01')),
    CONVERT(DATE, '2023-01-31'),
    CONVERT(DATE, '2023-02-06'),
    CONVERT(DATE, '2023-09-30'),
    CONVERT(DATE, '2023-09-30'),
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2022-09-15')),
    CONVERT(DATE, '2023-01-31'),
    CONVERT(DATE, '2023-02-06'),
    CONVERT(DATE, '2023-06-30'),
    CONVERT(DATE, '2023-09-14')
), (
    2023,
    -- Pg
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    CONVERT(DATE, '2024-01-31'),
    CONVERT(DATE, '2024-02-06'),
    CONVERT(DATE, '2024-05-31'),
    CONVERT(DATE, '2024-09-14'),
    -- Sport
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-01')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-01')),
    CONVERT(DATE, '2024-01-31'),
    CONVERT(DATE, '2024-02-06'),
    CONVERT(DATE, '2024-06-30'),
    CONVERT(DATE, '2024-08-31'),
    -- Cplr
    CONVERT(DATE, '2023-09-15'),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-10-01')),
    CONVERT(DATE, '2024-01-31'),
    CONVERT(DATE, '2024-02-06'),
    CONVERT(DATE, '2024-09-30'),
    CONVERT(DATE, '2024-09-30'),
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    CONVERT(DATE, '2024-01-31'),
    CONVERT(DATE, '2024-02-06'),
    CONVERT(DATE, '2024-06-30'),
    CONVERT(DATE, '2024-09-14')
), (
    2024,
    -- Pg
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-15')),
    CONVERT(DATE, '2025-01-31'),
    CONVERT(DATE, '2025-02-06'),
    CONVERT(DATE, '2025-05-31'),
    CONVERT(DATE, '2025-09-14'),
    -- Sport
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-01')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-01')),
    CONVERT(DATE, '2025-01-31'),
    CONVERT(DATE, '2025-02-06'),
    CONVERT(DATE, '2025-06-30'),
    CONVERT(DATE, '2025-08-31'),
    -- Cplr
    CONVERT(DATE, '2024-09-15'),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-10-01')),
    CONVERT(DATE, '2025-01-31'),
    CONVERT(DATE, '2025-02-06'),
    CONVERT(DATE, '2025-09-30'),
    CONVERT(DATE, '2025-09-30'),
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2024-09-15')),
    CONVERT(DATE, '2025-01-31'),
    CONVERT(DATE, '2025-02-06'),
    CONVERT(DATE, '2025-06-30'),
    CONVERT(DATE, '2025-09-14')
), (
    2025,
    -- Pg
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-15')),
    CONVERT(DATE, '2026-01-31'),
    CONVERT(DATE, '2026-02-06'),
    CONVERT(DATE, '2026-05-31'),
    CONVERT(DATE, '2026-09-14'),
    -- Sport
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-01')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-01')),
    CONVERT(DATE, '2026-01-31'),
    CONVERT(DATE, '2026-02-06'),
    CONVERT(DATE, '2026-06-30'),
    CONVERT(DATE, '2026-08-31'),
    -- Cplr
    CONVERT(DATE, '2025-09-15'),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-10-01')),
    CONVERT(DATE, '2026-01-31'),
    CONVERT(DATE, '2026-02-06'),
    CONVERT(DATE, '2026-09-30'),
    CONVERT(DATE, '2026-09-30'),
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2025-09-15')),
    CONVERT(DATE, '2026-01-31'),
    CONVERT(DATE, '2026-02-06'),
    CONVERT(DATE, '2026-06-30'),
    CONVERT(DATE, '2026-09-14')
);
GO
