GO
PRINT 'Insert Absence'
GO

INSERT INTO
    [school_books].[Absence] (
        [SchoolYear],
        [AbsenceId],
        [ClassBookId],
        [PersonId],
        [Term],
        [Type],
        [ExcusedReasonId],
        [ExcusedReasonComment],

        [ScheduleLessonId],
        [Date],
        [IsReadFromParent],

        [CreateDate],
        [CreatedBySysUserId],
        [ModifyDate],
        [ModifiedBySysUserId]
    )
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [AbsenceId] = NEXT VALUE FOR [school_books].[AbsenceIdSequence],
    [ClassBookId] = cb.[ClassBookId],
    [PersonId] = sc.[PersonId],
    [Term] = s.[Term],
    [Type] = sl.[AbsenceType],
    [ExcusedReasonId] = IIF(sl.[AbsenceType] = 3, ABS(CHECKSUM(NewId())) % 3 + 1, NULL),
    [ExcusedReasonComment] = IIF(sl.[AbsenceType] = 3, 'Excused comment ' + CONVERT(NVARCHAR, ABS(CHECKSUM(NewId()))), NULL),

    [ScheduleLessonId] = sl.[ScheduleLessonId],
    [Date] = sl.[Date],
    [IsReadFromParent] = 0,

    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1,
    [ModifyDate] = GETDATE(),
    [ModifiedBySysUserId] =  1
FROM
    [school_books].[ClassBook] cb
    INNER JOIN [inst_year].[ClassGroup] cg_lvl2 ON cb.[SchoolYear] = cg_lvl2.[SchoolYear] AND (
      (cb.[ClassIsLvl2] = 0 AND cb.[ClassId] = cg_lvl2.[ParentClassId]) OR
      (cb.[ClassIsLvl2] = 1 AND cb.[ClassId] = cg_lvl2.[ClassId])
    )
    INNER JOIN [student].[StudentClass] sc ON cg_lvl2.[SchoolYear] = sc.[SchoolYear] AND cg_lvl2.[ClassId] = sc.[ClassId]
    INNER JOIN [school_books].[Schedule] s ON cb.[SchoolYear] = s.[SchoolYear] AND cb.[ClassBookId] = s.[ClassBookId]
    INNER JOIN [inst_year].[CurriculumClass] cc ON cg_lvl2.[ParentClassId] = cc.[ClassID]
    CROSS APPLY (
        SELECT
            TOP (ABS(CHECKSUM(NewId())) % 2) -- random 0-1 absences per student
            sl.ScheduleLessonId,
            sl.Date,
            sl.CurriculumId,
            [AbsenceType] = ABS(CHECKSUM(NewId())) % 3 + 1
        FROM
            [school_books].[ScheduleLesson] sl
        WHERE
            sl.[SchoolYear] = s.[SchoolYear] AND sl.[ScheduleId] = s.[ScheduleId] AND sl.[CurriculumId] = cc.[CurriculumID]
            AND sl.Date < DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0)
        ORDER BY NEWID()
    ) sl
WHERE
    cb.[InstId] IN ($(TestClassBookDataInstitutions)) AND
    cb.[BookType] IN (2, 3, 4)
OPTION (NO_PERFORMANCE_SPOOL)

GO
