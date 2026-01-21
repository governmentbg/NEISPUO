GO
PRINT 'Insert Grade'
GO

INSERT INTO
    [school_books].[Grade] (
        [SchoolYear],
        [GradeId],
        [ClassBookId],
        [PersonId],
        [Type],
        [Term],
        [ScheduleLessonId],
        [Date],
        [CurriculumId],
        [Category],
        [DecimalGrade],
        [QualitativeGrade],
        [SpecialGrade],
        [IsReadFromParent],
        [CreateDate],
        [CreatedBySysUserId],
        [ModifyDate],
        [ModifiedBySysUserId]
    )
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [GradeId] = NEXT VALUE FOR [school_books].[GradeIdSequence],
    [ClassBookId] = cb.[ClassBookId],
    [PersonId] = sc.[PersonId],
    [Type] = 1,
    [Term] = s.[Term],

    [ScheduleLessonId] = sl.[ScheduleLessonId],
    [Date] = sl.[Date],
    [CurriculumId] = sl.[CurriculumId],

    [Category] = CASE
        WHEN sc.[PersonId] % 15 = 0 THEN 2 -- Special with 1/15 change
        WHEN [BookType] = 2 THEN 3 -- Qualitative
        WHEN [BookType] IN (3, 4) THEN 1 -- Decimal
        ELSE NULL
    END,
    [DecimalGrade] = CASE
        WHEN sc.[PersonId] % 15 <> 0 AND [BookType] IN (3, 4) THEN ABS(CHECKSUM(NewId())) % 5 + 2
        ELSE NULL
    END,
    [QualitativeGrade] = CASE
        WHEN sc.[PersonId] % 15 <> 0 AND [BookType] = 2 THEN ABS(CHECKSUM(NewId())) % 5 + 2
        ELSE NULL
    END,
    [SpecialGrade] = CASE
        WHEN sc.[PersonId] % 15 = 0 THEN ABS(CHECKSUM(NewId())) % 3 + 1
        ELSE NULL
    END,

    [IsReadFromParent] = 0,

    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] = 1
FROM
    [school_books].[ClassBook] cb
    INNER JOIN [inst_year].[ClassGroup] cg_lvl2 ON cb.[SchoolYear] = cg_lvl2.[SchoolYear] AND (
      (cb.[ClassIsLvl2] = 0 AND cb.[ClassId] = cg_lvl2.[ParentClassId]) OR
      (cb.[ClassIsLvl2] = 1 AND cb.[ClassId] = cg_lvl2.[ClassId])
    )
    INNER JOIN [student].[StudentClass] sc ON cg_lvl2.[SchoolYear] = sc.[SchoolYear] AND cg_lvl2.[ClassId] = sc.[ClassId]
    INNER JOIN [school_books].[Schedule] s ON cb.[SchoolYear] = s.[SchoolYear] AND cb.[ClassBookId] = s.[ClassBookId]
    INNER JOIN [inst_year].[CurriculumClass] cc ON cg_lvl2.[ParentClassId] = cc.[ClassID]
    INNER JOIN [inst_year].[Curriculum] c ON cc.[CurriculumID] = c.[CurriculumID]
    CROSS APPLY (
        SELECT
            TOP 4 -- 4 grades per student
            sl.ScheduleLessonId,
            sl.Date,
            sl.CurriculumId
        FROM
            [school_books].[ScheduleLesson] sl
        WHERE
            sl.[SchoolYear] = s.[SchoolYear] AND sl.[ScheduleId] = s.[ScheduleId] AND sl.[CurriculumId] = cc.[CurriculumID]
            AND sl.Date < DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0)
        ORDER BY NEWID()
    ) sl
WHERE
    cb.[InstId] IN ($(TestClassBookDataInstitutions)) AND
    cb.[BookType] IN (2, 3, 4) AND
    c.[SubjectTypeId] NOT IN (152, 153, 154, 155, 156) -- profiling subject
OPTION (NO_PERFORMANCE_SPOOL)

GO
