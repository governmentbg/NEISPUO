CREATE OR ALTER FUNCTION [school_books].fn_closest_weekday (@date DATE)
RETURNS DATE
AS
BEGIN
    DECLARE @weekday INT;
    SELECT @weekday = ((DATEPART(WEEKDAY, @date) + @@DATEFIRST + 5) % 7) + 1

    DECLARE @offset INT;
    SELECT @offset =
      CASE @weekday
         WHEN 6 THEN 2
         WHEN 7 THEN 1
         ELSE 0
      END

    RETURN DATEADD(DAY, @offset, @date);
END
GO

ALTER TABLE [school_books].[ClassBookSchoolYearDateInfo]
ADD
    [SchoolYearStartDateLimit]  DATE NULL,
    [SchoolYearEndDateLimit]    DATE NULL;
GO

UPDATE [school_books].[ClassBookSchoolYearDateInfo]
SET
    [SchoolYearStartDateLimit]= CONVERT(DATE, '2021-09-15'),
    [SchoolYearEndDateLimit]  = CONVERT(DATE, '2022-09-14'),
    [SchoolYearStartDate] = IIF([SchoolYearStartDate] > CONVERT(DATE, '2021-09-15'), [SchoolYearStartDate], CONVERT(DATE, '2021-09-15')),
    [SchoolYearEndDate]   = IIF([SchoolYearEndDate] < CONVERT(DATE, '2022-09-14'), [SchoolYearEndDate], CONVERT(DATE, '2022-09-14'))
WHERE [SchoolYear] = 2021
GO

UPDATE [school_books].[ClassBookSchoolYearDateInfo]
SET
    [SchoolYearStartDateLimit]= CONVERT(DATE, '2022-09-15'),
    [SchoolYearEndDateLimit]  = CONVERT(DATE, '2023-09-14'),
    [SchoolYearStartDate] = IIF([SchoolYearStartDate] > CONVERT(DATE, '2022-09-15'), [SchoolYearStartDate], CONVERT(DATE, '2022-09-15')),
    [SchoolYearEndDate]   = IIF([SchoolYearEndDate] < CONVERT(DATE, '2023-09-14'), [SchoolYearEndDate], CONVERT(DATE, '2023-09-14'))
WHERE [SchoolYear] = 2022
GO

ALTER TABLE [school_books].[ClassBookSchoolYearDateInfo]
ALTER COLUMN [SchoolYearStartDateLimit] DATE NOT NULL;
ALTER TABLE [school_books].[ClassBookSchoolYearDateInfo]
ALTER COLUMN [SchoolYearEndDateLimit] DATE NOT NULL;
GO

UPDATE [school_books].[SchoolYearDateInfo]
SET
    [SchoolYearStartDate] = IIF([SchoolYearStartDate] > CONVERT(DATE, '2021-09-15'), [SchoolYearStartDate], CONVERT(DATE, '2021-09-15')),
    [SchoolYearEndDate]   = IIF([SchoolYearEndDate] < CONVERT(DATE, '2022-09-14'), [SchoolYearEndDate], CONVERT(DATE, '2022-09-14'))
WHERE [SchoolYear] = 2021
GO

UPDATE [school_books].[SchoolYearDateInfo]
SET
    [SchoolYearStartDate] = IIF([SchoolYearStartDate] > CONVERT(DATE, '2022-09-15'), [SchoolYearStartDate], CONVERT(DATE, '2022-09-15')),
    [SchoolYearEndDate]   = IIF([SchoolYearEndDate] < CONVERT(DATE, '2023-09-14'), [SchoolYearEndDate], CONVERT(DATE, '2023-09-14'))
WHERE [SchoolYear] = 2022
GO

DROP TABLE [school_books].[SchoolYearDateInfoDefault];
GO

CREATE TABLE [school_books].[SchoolYearDateInfoDefault] (
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

    [OtherSchoolYearStartDateLimit] DATE        NOT NULL,
    [OtherSchoolYearStartDate]      DATE        NOT NULL,
    [OtherFirstTermEndDate]         DATE        NOT NULL,
    [OtherSecondTermStartDate]      DATE        NOT NULL,
    [OtherSchoolYearEndDate]        DATE        NOT NULL,
    [OtherSchoolYearEndDateLimit]   DATE        NOT NULL,

    CONSTRAINT [PK_SchoolYearDateInfoDefault] PRIMARY KEY ([SchoolYear])
);
GO

INSERT INTO [school_books].[SchoolYearDateInfoDefault] (
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
    -- Other
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    [school_books].[fn_closest_weekday](CONVERT(DATE, '2023-09-15')),
    CONVERT(DATE, '2024-01-31'),
    CONVERT(DATE, '2024-02-06'),
    CONVERT(DATE, '2024-06-30'),
    CONVERT(DATE, '2024-09-14')
);
GO
