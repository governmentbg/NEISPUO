GO
PRINT 'Insert ClassBook'
GO

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
        ClassName <> '5 г' AND -- skip a class for test purposes
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
        [BasicClassName] = bc.[Name],
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
        WHERE EXISTS (
              SELECT 1
              FROM [inst_year].[CurriculumClass] cc
              WHERE cc.[ClassID] = c.[ClassID] AND cc.[IsValid] = 1
          )
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
        [BasicClassName],
        [BookName],
        [IsFinalized],
        [IsValid],
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
    [BasicClassName],
    [BookName],
    [IsFinalized] = 0,
    [IsValid] = 1,
    [CreateDate],
    [CreatedBySysUserId],
    [ModifyDate],
    [ModifiedBySysUserId]
FROM
    ClassBooks cb
WHERE
    SchoolYear IN ($(ScriptSchoolYears)) AND
    InstId IN ($(TestClassBookDataInstitutions)) AND
    BookType IS NOT NULL AND
    NOT EXISTS (
        SELECT 1
        FROM [school_books].[ClassBook] cbi
        WHERE
            cbi.InstId = cb.InstId AND
            cbi.SchoolYear = cb.SchoolYear AND
            cbi.ClassId = cb.ClassId
    )
GO

INSERT INTO [school_books].[ClassBookSchoolYearSettings] (
    [SchoolYear],
    [ClassBookId],
    [SchoolYearSettingsId],
    [SchoolYearStartDateLimit],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [SchoolYearEndDateLimit],
    [HasFutureEntryLock],
    [PastMonthLockDay]
)
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [ClassBookId] = cb.[ClassBookId],
    [SchoolYearSettingsId] = NULL,
    [SchoolYearStartDateLimit] = sysd.[PgSchoolYearStartDateLimit],
    [SchoolYearStartDate] = sysd.[PgSchoolYearStartDate],
    [FirstTermEndDate] = sysd.[PgFirstTermEndDate],
    [SecondTermStartDate] = sysd.[PgSecondTermStartDate],
    [SchoolYearEndDate] = sysd.[PgSchoolYearEndDate],
    [SchoolYearEndDateLimit] = sysd.[PgSchoolYearEndDateLimit],
    [HasFutureEntryLock] = 0,
    [PastMonthLockDay] = NULL
FROM [school_books].[ClassBook] cb
JOIN [school_books].[SchoolYearSettingsDefault] sysd ON cb.[SchoolYear] = sysd.[SchoolYear]
WHERE cb.BasicClassId < 0 OR cb.BasicClassId = 21 OR cb.BasicClassId = 32

INSERT INTO [school_books].[ClassBookSchoolYearSettings] (
    [SchoolYear],
    [ClassBookId],
    [SchoolYearSettingsId],
    [SchoolYearStartDateLimit],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [SchoolYearEndDateLimit],
    [HasFutureEntryLock],
    [PastMonthLockDay]
)
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [ClassBookId] = cb.[ClassBookId],
    [SchoolYearSettingsId] = NULL,
    [SchoolYearStartDateLimit] = sysd.[SportSchoolYearStartDateLimit],
    [SchoolYearStartDate] = sysd.[SportSchoolYearStartDate],
    [FirstTermEndDate] = sysd.[SportFirstTermEndDate],
    [SecondTermStartDate] = sysd.[SportSecondTermStartDate],
    [SchoolYearEndDate] = sysd.[SportSchoolYearEndDate],
    [SchoolYearEndDateLimit] = sysd.[SportSchoolYearEndDateLimit],
    [HasFutureEntryLock] = 0,
    [PastMonthLockDay] = NULL
FROM [school_books].[ClassBook] cb
JOIN [school_books].[SchoolYearSettingsDefault] sysd ON cb.[SchoolYear] = sysd.[SchoolYear]
JOIN [core].[InstitutionSchoolYear] isy ON cb.[SchoolYear] = isy.[SchoolYear] AND cb.[InstId] = isy.[InstitutionId]
WHERE isy.[DetailedSchoolTypeId] IN (8, 114)
    AND NOT EXISTS (
        SELECT 1 FROM [school_books].[ClassBookSchoolYearSettings]
        WHERE [SchoolYear] = cb.[SchoolYear] AND [ClassBookId] = cb.[ClassBookId]
    )

INSERT INTO [school_books].[ClassBookSchoolYearSettings] (
    [SchoolYear],
    [ClassBookId],
    [SchoolYearSettingsId],
    [SchoolYearStartDateLimit],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [SchoolYearEndDateLimit],
    [HasFutureEntryLock],
    [PastMonthLockDay]
)
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [ClassBookId] = cb.[ClassBookId],
    [SchoolYearSettingsId] = NULL,
    [SchoolYearStartDateLimit] = sysd.[CplrSchoolYearStartDateLimit],
    [SchoolYearStartDate] = sysd.[CplrSchoolYearStartDate],
    [FirstTermEndDate] = sysd.[CplrFirstTermEndDate],
    [SecondTermStartDate] = sysd.[CplrSecondTermStartDate],
    [SchoolYearEndDate] = sysd.[CplrSchoolYearEndDate],
    [SchoolYearEndDateLimit] = sysd.[CplrSchoolYearEndDateLimit],
    [HasFutureEntryLock] = 0,
    [PastMonthLockDay] = NULL
FROM [school_books].[ClassBook] cb
JOIN [school_books].[SchoolYearSettingsDefault] sysd ON cb.[SchoolYear] = sysd.[SchoolYear]
JOIN [core].[InstitutionSchoolYear] isy ON cb.[SchoolYear] = isy.[SchoolYear] AND cb.[InstId] = isy.[InstitutionId]
JOIN [noms].[DetailedSchoolType] dst ON isy.[DetailedSchoolTypeId] = dst.[DetailedSchoolTypeId]
WHERE dst.[InstType] = 3
    AND NOT EXISTS (
        SELECT 1 FROM [school_books].[ClassBookSchoolYearSettings]
        WHERE [SchoolYear] = cb.[SchoolYear] AND [ClassBookId] = cb.[ClassBookId]
    )

INSERT INTO [school_books].[ClassBookSchoolYearSettings] (
    [SchoolYear],
    [ClassBookId],
    [SchoolYearSettingsId],
    [SchoolYearStartDateLimit],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [SchoolYearEndDateLimit],
    [HasFutureEntryLock],
    [PastMonthLockDay]
)
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [ClassBookId] = cb.[ClassBookId],
    [SchoolYearSettingsId] = NULL,
    [SchoolYearStartDateLimit] = sysd.[OtherSchoolYearStartDateLimit],
    [SchoolYearStartDate] = sysd.[OtherSchoolYearStartDate],
    [FirstTermEndDate] = sysd.[OtherFirstTermEndDate],
    [SecondTermStartDate] = sysd.[OtherSecondTermStartDate],
    [SchoolYearEndDate] = sysd.[OtherSchoolYearEndDate],
    [SchoolYearEndDateLimit] = sysd.[OtherSchoolYearEndDateLimit],
    [HasFutureEntryLock] = 0,
    [PastMonthLockDay] = NULL
FROM [school_books].[ClassBook] cb
JOIN [school_books].[SchoolYearSettingsDefault] sysd ON cb.[SchoolYear] = sysd.[SchoolYear]
WHERE NOT EXISTS (
    SELECT 1 FROM [school_books].[ClassBookSchoolYearSettings]
    WHERE [SchoolYear] = cb.[SchoolYear] AND [ClassBookId] = cb.[ClassBookId]
)
GO
