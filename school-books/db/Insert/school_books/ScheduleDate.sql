GO
PRINT 'Insert ScheduleDate'
GO

--create calendar table
CREATE TABLE #Calendar (
    [Date]          DATE    NOT NULL PRIMARY KEY,
    [Year]          INT     NOT NULL,
    [WeekNumber]    INT     NOT NULL,
    [Day]           INT     NOT NULL,
);
GO

-- rebuild calendar table
DECLARE @MinDate DATE = '20100101',
        @MaxDate DATE = '20491231';

WITH Dates AS (
    SELECT
        TOP (DATEDIFF(DAY, @MinDate, @MaxDate) + 1)
        DATEADD(DAY, ROW_NUMBER() OVER(ORDER BY a.object_id) - 1, @MinDate) AS Date
    FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
),
IsoDates AS (
    SELECT
        Date,
        DATEPART(yyyy, DATEADD(day, 26 - DATEPART(isowk, Date), Date)) AS Year,
        DATEPART(isowk, Date) AS WeekNumber,
        (DATEPART(dw, Date) + (@@DATEFIRST - 1) - 1) % 7 + 1 AS Day
    FROM
        Dates
)
INSERT INTO #Calendar
    ([Date], [Year], [WeekNumber], [Day])
SELECT
    [Date], [Year], [WeekNumber], [Day]
FROM
    IsoDates
GO

INSERT INTO
    [school_books].[ScheduleDate] (
        [SchoolYear],
        [ScheduleId],
        [Date],
        [Year],
        [WeekNumber],
        [Day]
    )
SELECT
    s.[SchoolYear],
    s.[ScheduleId],
    c.[Date],
    c.[Year],
    c.[WeekNumber],
    c.[Day]
FROM
    [school_books].[Schedule] s
    INNER JOIN [school_books].[ClassBook] cb ON
        s.SchoolYear = cb.SchoolYear AND s.ClassBookId = cb.ClassBookId
    INNER JOIN #Calendar c ON
        (c.Date BETWEEN s.StartDate AND s.EndDate) AND
        (c.Day NOT IN (6, 7))
WHERE
    cb.InstId IN ($(TestClassBookDataInstitutions))
GO

DROP TABLE #Calendar
GO
