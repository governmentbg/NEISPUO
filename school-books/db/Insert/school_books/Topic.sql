GO
PRINT 'Insert Topic'
GO

INSERT INTO
    [school_books].[Topic] (
        [SchoolYear],
        [TopicId],
        [ClassBookId],
        [Date],
        [ScheduleLessonId],
        [CreateDate],
        [CreatedBySysUserId]
    )
SELECT
    [SchoolYear] = cb.[SchoolYear],
    [TopicId] = NEXT VALUE FOR [school_books].[TopicIdSequence],
    [ClassBookId] = cb.[ClassBookId],
    [Date] = sl.[Date],
    [ScheduleLessonId] = sl.[ScheduleLessonId],
    [CreateDate] = GETDATE(),
    [CreatedBySysUserId] = 1
FROM
    [school_books].[ClassBook] cb
    INNER JOIN [school_books].[Schedule] s ON cb.[SchoolYear] = s.[SchoolYear] AND cb.[ClassBookId] = s.[ClassBookId]
    INNER JOIN [school_books].[ScheduleLesson] sl ON sl.[SchoolYear] = s.[SchoolYear] AND sl.[ScheduleId] = s.[ScheduleId]
WHERE
    cb.[InstId] IN ($(TestClassBookDataInstitutions)) AND
    sl.[Date] < DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) AND
    cb.[BookType] IN (2, 3, 4)
OPTION (NO_PERFORMANCE_SPOOL)
GO

INSERT INTO
    [school_books].[TopicTitle] (
        [SchoolYear],
        [TopicId],
        [Index],
        [Title]
    )
SELECT
    [SchoolYear] = t.[SchoolYear],
    [TopicId] = t.[TopicId],
    [Index] = 0,
    [Title] = N'Тема ' + CONVERT(NVARCHAR, sl.[Date], 23) + ' #' + CAST(sl.HourNumber AS NVARCHAR)
FROM
    [school_books].[Topic] t
    INNER JOIN [school_books].[ScheduleLesson] sl ON t.[SchoolYear] = sl.[SchoolYear] AND t.[ScheduleLessonId] = sl.[ScheduleLessonId]
GO

INSERT INTO
    [school_books].[TopicTeacher] (
        [SchoolYear],
        [TopicId],
        [PersonId],
        [IsReplTeacher]
    )
SELECT
    DISTINCT -- TODO remove after data refresh
    [SchoolYear] = t.[SchoolYear],
    [TopicId] = t.[TopicId],
    [PersonId] = sp.[PersonId],
    [IsReplTeacher] = 0
FROM
    [school_books].[Topic] t
    INNER JOIN [school_books].[ScheduleLesson] sl ON t.[SchoolYear] = sl.[SchoolYear] AND t.[ScheduleLessonId] = sl.[ScheduleLessonId]
    INNER JOIN [inst_year].[CurriculumTeacher] ct ON sl.CurriculumId = ct.CurriculumID AND ct.IsValid = 1
    INNER JOIN [inst_basic].[StaffPosition] sp ON ct.StaffPositionId = sp.StaffPositionId
GO
