GO
PRINT 'Insert SchoolYearSettings'
GO

INSERT INTO [school_books].[SchoolYearSettings] (
    [SchoolYear],
    [SchoolYearSettingsId],
    [InstId],
    [SchoolYearStartDate],
    [FirstTermEndDate],
    [SecondTermStartDate],
    [SchoolYearEndDate],
    [Description],
    [HasFutureEntryLock],
    [PastMonthLockDay],
    [IsForAllClasses],
    [CreateDate],
    [CreatedBySysUserId],
    [ModifyDate],
    [ModifiedBySysUserId]
)
SELECT
    inst.[SchoolYear],
    [SchoolYearSettingsId] = NEXT VALUE FOR [school_books].[SchoolYearSettingsIdSequence],
    [InstId] = inst.[InstitutionID],
    [SchoolYearStartDate] = NULL,
    [FirstTermEndDate] =  NULL,
    [SecondTermStartDate] = NULL,
    [SchoolYearEndDate] = CONVERT(DATE, CONCAT(inst.[SchoolYear] + 1, '-06-30')),
    [Description] = 'всички класове',
    [HasFutureEntryLock] = 0,
    [PastMonthLockDay] = NULL,
    [IsForAllClasses] = 1,
    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] = 1
FROM [core].[InstitutionSchoolYear] inst
WHERE
    inst.[SchoolYear] IN ($(ScriptSchoolYears))
GO
