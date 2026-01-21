DECLARE @schoolYear INT = 2022;

DECLARE @instIds TABLE (InstId INT);
INSERT INTO @instIds
SELECT InstId FROM (VALUES (300125)) AS v(InstId);

DECLARE @Term1StartDate DATE = '2022-09-15';
DECLARE @Term1EndDate DATE = '2023-01-31';
DECLARE @Term2StartDate DATE = '2023-02-06';
DECLARE @Term2EndDate DATE = '2023-06-30';

WITH ClassGroups_Regular_lvl1 AS (
    SELECT
        SchoolYear,
        InstitutionID,
        ClassID,
        BasicClassID,
        ClassTypeID,
        IsSpecNeed,
        ClassName,
        ParalellClassName,
        ClassIsLvl2 = 0
    FROM
        [inst_year].ClassGroup
    WHERE
        ParentClassID IS NULL AND
        BasicClassID IS NOT NULL AND
        ClassTypeID IS NOT NULL AND
        (IsCombined IS NULL OR IsCombined = 0)
),
ClassGroups_Combined_lvl2 AS (
    SELECT
        SchoolYear,
        InstitutionID,
        ClassID,
        BasicClassID,
        ClassTypeID,
        IsSpecNeed,
        ClassName,
        ParalellClassName,
        ClassIsLvl2 = 1
    FROM
        [inst_year].ClassGroup
    WHERE
        ParentClassID IS NOT NULL AND
        IsCombined = 1
),
Classes AS (
    SELECT
        SchoolYear,
        InstitutionID,
        ClassID,
        BasicClassID,
        ClassTypeID,
        IsSpecNeed,
        ClassName,
        ParalellClassName,
        ClassIsLvl2
    FROM (
        SELECT * FROM ClassGroups_Regular_lvl1
        UNION ALL
        SELECT * FROM ClassGroups_Combined_lvl2
    ) u
),
ClassBooks AS (
    SELECT
        c.[SchoolYear],
        [InstId] = c.[InstitutionID],
        c.[ClassId],
        c.[ClassIsLvl2],
        c.[BasicClassID],
        [BookType] = CASE
            WHEN c.[IsSpecNeed] = 1                              THEN 7 -- Book_CSOP
            WHEN c.[ClassTypeID] = 36                            THEN 6 -- Book_DPLR
            WHEN c.[ClassTypeID] = 38                            THEN 5 -- Book_CDO
            WHEN c.[ClassTypeID] = 39                            THEN 5 -- Book_CDO -- Група в общежитие
            WHEN c.[BasicClassID] IN (-6, -1)                    THEN 1 -- Book_PG
            WHEN c.[BasicClassID] IN (1, 2, 3)                   THEN 2 -- Book_I_III
            WHEN c.[BasicClassID] = 4                            THEN 3 -- Book_IV
            WHEN c.[BasicClassID] IN (5, 6, 7, 8, 9, 10, 11, 12) THEN 4 -- Book_V_XII
            ELSE NULL
        END,
        [BookName] = CASE
            WHEN c.[ParalellClassName] IS NOT NULL THEN c.[ParalellClassName]
            WHEN c.[ClassIsLvl2] = 1 THEN ''
            WHEN CHARINDEX(bc.[Name], c.[ClassName]) = 1 THEN TRIM('. ' FROM SUBSTRING(c.[ClassName], LEN(bc.[Name]) + 1, LEN(c.[ClassName])))
            ELSE c.[ClassName]
        END,
        [CreateDate] = GETDATE(),
        [CreatedBySysUserId] = 1,
        [ModifyDate] = GETDATE(),
        [ModifiedBySysUserId] =  1
    FROM
        Classes c
        INNER JOIN [inst_nom].[BasicClass] bc ON c.[BasicClassID] = bc.[BasicClassID]
)
INSERT INTO
    [school_books].[ClassBook] (
        [SchoolYear],
        [ClassBookId],
        [InstId],
        [ClassId],
        [ClassIsLvl2],
        [BookType],
        [BasicClassID],
        [BookName],
        [CreateDate],
        [CreatedBySysUserId],
        [ModifyDate],
        [ModifiedBySysUserId]
    )
SELECT
    [SchoolYear],
    [ClassBookId] = NEXT VALUE FOR [school_books].[ClassBookIdSequence] OVER (ORDER BY [InstId], [BasicClassID], [BookName]),
    [InstId],
    [ClassId],
    [ClassIsLvl2],
    [BookType],
    [BasicClassID],
    [BookName],
    [CreateDate],
    [CreatedBySysUserId],
    [ModifyDate],
    [ModifiedBySysUserId]
FROM
    ClassBooks cb
WHERE
    cb.SchoolYear = @schoolYear AND
    cb.InstId IN (SELECT InstId FROM @instIds) AND
    BookType IS NOT NULL AND
    NOT EXISTS (
        SELECT 1
        FROM [school_books].[ClassBook] cbi
        WHERE
            cbi.InstId = cb.InstId AND
            cbi.SchoolYear = cb.SchoolYear AND
            cbi.ClassId = cb.ClassId
    )

INSERT INTO [school_books].[ClassBookSchoolYearDateInfo] (
    [SchoolYear],
    [ClassBookId],
    [SchoolYearStartDateLimit],
    [SchoolYearDateInfoId],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [SchoolYearEndDateLimit]
)
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [ClassBookId] = cb.[ClassBookId],
    [SchoolYearStartDateLimit] = did.[OtherSchoolYearStartDateLimit],
    [SchoolYearDateInfoId] = NULL,
    [SchoolYearStartDate] = did.[OtherSchoolYearStartDate],
    [FirstTermEndDate] = did.[OtherFirstTermEndDate],
    [SecondTermStartDate] = did.[OtherSecondTermStartDate],
    [SchoolYearEndDate] = did.[OtherSchoolYearEndDate],
    [SchoolYearEndDateLimit] = did.[OtherSchoolYearEndDateLimit]
FROM [school_books].[ClassBook] cb
JOIN [school_books].[SchoolYearDateInfoDefault] did
    ON cb.[SchoolYear] = did.[SchoolYear]
WHERE
    cb.SchoolYear = @schoolYear AND
    cb.InstId IN (SELECT InstId FROM @instIds);

WITH Terms AS (
    SELECT
        [Term] = 1,
        [StartDate] = @Term1StartDate,
        [EndDate] = @Term1EndDate
    UNION ALL
    SELECT
        [Term] = 2,
        [StartDate] = @Term2StartDate,
        [EndDate] = @Term2EndDate
)
INSERT INTO
    [school_books].[Schedule] (
        [SchoolYear],
        [ScheduleId],
        [ClassBookId],
        [IsIndividualSchedule],
        [Term],
        [StartDate],
        [EndDate],
        [ShiftId],
        [IsRziApproved],
        [CreateDate],
        [CreatedBySysUserId],
        [ModifyDate],
        [ModifiedBySysUserId]
    )
SELECT
    cb.[SchoolYear],
    [ScheduleId] = NEXT VALUE FOR [school_books].[ScheduleIdSequence],
    cb.[ClassBookId],
    [IsIndividualSchedule] = 0,
    [Term] = t.[Term],
    [StartDate] = t.[StartDate],
    [EndDate] = t.[EndDate],
    [ShiftId] = (SELECT TOP 1 ShiftId FROM [school_books].[Shift] WHERE [InstId] = cb.[InstId]),
    [IsRziApproved] = 1,
    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] = 1
FROM
    [school_books].[ClassBook] cb
    CROSS JOIN Terms t
WHERE
    cb.SchoolYear = @schoolYear AND
    cb.InstId IN (SELECT InstId FROM @instIds)

--create calendar table
CREATE TABLE #Calendar (
    [Date]          DATE    NOT NULL PRIMARY KEY,
    [Year]          INT     NOT NULL,
    [WeekNumber]    INT     NOT NULL,
    [Day]           INT     NOT NULL,
);

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
    cb.SchoolYear = @schoolYear AND
    cb.InstId IN (SELECT InstId FROM @instIds);

DROP TABLE #Calendar;

WITH Numbers100 AS (
    SELECT TOP 100
        [Num] = ROW_NUMBER () OVER (
            ORDER BY object_id
        )
    FROM sys.all_objects
),
ScheduleHours AS (
    SELECT
        s.[SchoolYear],
        s.[ScheduleId],
        [ClassId] = IIF(cb.[ClassIsLvl2] = 0, cg.[ClassId], cg.[ParentClassId]),
        d.[Day],
        h.[HourNumber],
        [HourIndex] = ROW_NUMBER () OVER (
            PARTITION BY s.[SchoolYear], s.[ScheduleId]
            ORDER BY d.[Day], h.[HourNumber]
        )
    FROM
        [school_books].[Schedule] s
        INNER JOIN [school_books].[ClassBook] cb ON s.[ClassBookId] = cb.[ClassBookId]
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.[ClassId] = cg.[ClassId]
        CROSS JOIN (VALUES (1), (2), (3), (4), (5)) AS d([Day])
        CROSS JOIN (VALUES (1), (2), (3), (4), (5), (6), (7)) AS h([HourNumber])
    WHERE
        cb.SchoolYear = @schoolYear AND
        cb.InstId IN (SELECT InstId FROM @instIds)
),
CurriculumIndexed AS (
    SELECT
        c.[SchoolYear],
        c.[CurriculumID],
        cc.[ClassID],
        [HourIndex] = ROW_NUMBER () OVER (
            PARTITION BY c.[SchoolYear], c.[InstitutionID],	cc.[ClassID]
            ORDER BY n.[Num]
        )
    FROM
        [inst_year].[Curriculum] c
        INNER JOIN [inst_year].[CurriculumClass] cc ON c.[CurriculumID] = cc.[CurriculumID]
        INNER JOIN [inst_year].[ClassGroup] cg ON cc.[ClassID] = cg.[ClassID]
        CROSS JOIN Numbers100 n
    WHERE
        cg.SchoolYear = @schoolYear AND
        cg.InstitutionID IN (SELECT InstId FROM @instIds)
)
INSERT INTO
    [school_books].[ScheduleHour] (
        [SchoolYear],
        [ScheduleId],
        [Day],
        [HourNumber],
        [CurriculumId]
    )
SELECT
    sh.[SchoolYear],
    sh.[ScheduleId],
    sh.[Day],
    sh.[HourNumber],
    ci.[CurriculumID]
FROM
    ScheduleHours sh
    INNER JOIN CurriculumIndexed ci ON
        sh.[SchoolYear] = ci.[SchoolYear] AND
        sh.[ClassId] = ci.[ClassId] AND
        sh.[HourIndex] = ci.[HourIndex]

INSERT INTO
    [school_books].[ScheduleLesson] (
        [SchoolYear],
        [ScheduleLessonId],
        [ScheduleId],
        [Date],
        [Day],
        [HourNumber],
        [CurriculumId]
    )
SELECT
    sh.[SchoolYear],
    [ScheduleLessonId] = NEXT VALUE FOR [school_books].[ScheduleLessonIdSequence],
    sh.[ScheduleId],
    sd.[Date],
    sh.[Day],
    sh.[HourNumber],
    sh.[CurriculumID]
FROM
    [school_books].[ScheduleDate] sd
    INNER JOIN [school_books].[Schedule] s ON
        sd.SchoolYear = s.SchoolYear AND sd.ScheduleId = s.ScheduleId
    INNER JOIN [school_books].[ClassBook] cb ON
        s.SchoolYear = cb.SchoolYear AND s.ClassBookId = cb.ClassBookId
    INNER JOIN [school_books].[ScheduleHour] sh ON
        sh.[SchoolYear] = sd.[SchoolYear] AND
        sh.[ScheduleId] = sd.[ScheduleId] AND
        sh.[Day] = sd.[Day]
WHERE
    cb.SchoolYear = @schoolYear AND
    cb.InstId IN (SELECT InstId FROM @instIds)
GO
