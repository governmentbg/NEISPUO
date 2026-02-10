GO
PRINT 'Insert Shift'
GO

INSERT INTO
    [school_books].[Shift] (
        [SchoolYear],
        [ShiftId],
        [InstId],
        [Name],
        [IsMultiday],
        [IsAdhoc],
        [CreateDate],
        [CreatedBySysUserId],
        [ModifyDate],
        [ModifiedBySysUserId]
    )
SELECT
    [SchoolYear],
    [ShiftId] = NEXT VALUE FOR [school_books].[ShiftIdSequence] OVER (ORDER BY [InstitutionID]),
    [InstId] = [InstitutionID],
    [Name] = N'Първа смяна',
    [IsMultiday] = 0,
    [IsAdhoc] = 0,
    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] = 1
FROM
    [core].[InstitutionSchoolYear]
WHERE
    SchoolYear IN ($(ScriptSchoolYears)) AND
    [InstitutionID] IN ($(TestClassBookDataInstitutions))
GO

INSERT INTO
    [school_books].[ShiftHour] (
        [SchoolYear],
        [ShiftId],
        [Day],
        [HourNumber],
        [StartTime],
        [EndTime]
    )
SELECT
    s.[SchoolYear],
    s.[ShiftId],
    sd.[Day],
    sh.[HourNumber],
    sh.[StartTime],
    sh.[EndTime]
FROM
    [school_books].[Shift] s
    CROSS JOIN (
        VALUES (1), (2), (3), (4), (5), (6), (7)
    ) AS sd([Day])
    CROSS JOIN (
        VALUES
        (1, N'07:30', N'08:10'),
        (2, N'08:20', N'09:00'),
        (3, N'09:20', N'10:00'),
        (4, N'10:10', N'10:50'),
        (5, N'11:00', N'11:40'),
        (6, N'11:50', N'12:30'),
        (7, N'12:35', N'13:15')
    ) AS sh([HourNumber], [StartTime], [EndTime])
WHERE
    s.InstId IN ($(TestClassBookDataInstitutions))
GO
