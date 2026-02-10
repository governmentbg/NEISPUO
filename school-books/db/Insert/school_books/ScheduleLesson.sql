GO
PRINT 'Insert ScheduleLesson'
GO

INSERT INTO
    [school_books].[ScheduleLesson] (
        [SchoolYear],
        [ScheduleLessonId],
        [ScheduleId],
        [Date],
        [Day],
        [HourNumber],
        [CurriculumId],
        [IsVerified]
    )
SELECT
    sh.[SchoolYear],
    [ScheduleLessonId] = NEXT VALUE FOR [school_books].[ScheduleLessonIdSequence],
    sh.[ScheduleId],
    sd.[Date],
    sh.[Day],
    sh.[HourNumber],
    sh.[CurriculumID],
    [IsVerified] = 0
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
    cb.InstId IN ($(TestClassBookDataInstitutions))
GO

