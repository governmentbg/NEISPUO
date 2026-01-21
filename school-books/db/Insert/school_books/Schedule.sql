GO
PRINT 'Insert Schedule'
GO

WITH Terms AS (
    SELECT
        [Term] = 1,
        [SchoolYear] = sysd.[SchoolYear],
        [StartDate] = sysd.[OtherSchoolYearStartDate],  -- TODO fix hardcoded other school type
        [EndDate] = sysd.[OtherFirstTermEndDate]
    FROM [school_books].[SchoolYearSettingsDefault] sysd
    UNION ALL
    SELECT
        [Term] = 2,
        [SchoolYear] = sysd.[SchoolYear],
        [StartDate] = sysd.[OtherSecondTermStartDate], -- TODO fix hardcoded other school type
        [EndDate] = sysd.[OtherSchoolYearEndDate]
    FROM [school_books].[SchoolYearSettingsDefault] sysd
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
        [IncludesWeekend],
        [IsSplitting],
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
    [ShiftId] = (SELECT TOP 1 ShiftId FROM [school_books].[Shift] WHERE [SchoolYear] = cb.[SchoolYear] AND [InstId] = cb.[InstId]),
    [IsRziApproved] = 1,
    [IncludesWeekend] = 0,
    [IsSplitting] = 0,
    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] = 1
FROM
    [school_books].[ClassBook] cb
    INNER JOIN Terms t ON cb.[SchoolYear] = t.[SchoolYear]
WHERE
    cb.[SchoolYear] IN ($(ScriptSchoolYears)) AND
    cb.[InstId] IN ($(TestClassBookDataInstitutions))
GO
