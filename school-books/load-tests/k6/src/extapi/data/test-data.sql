WITH NonCombinedStudents AS (
    SELECT
        sc.PersonId,
        sc.ClassId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassId
        INNER JOIN [student].[StudentClass] sc ON cg.SchoolYear = sc.SchoolYear AND cg.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 0 AND
        sc.Status = 1
),
CombinedStudents AS (
    SELECT
        sc.PersonId,
        sc.ClassId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND cb.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 1 AND
        sc.Status = 1
),
Students AS (
    SELECT
        u.PersonId,
        u.ClassId,
        u.SchoolYear,
        u.InstId,
        u.ClassBookId
    FROM (
        SELECT * FROM NonCombinedStudents
        UNION ALL
        SELECT * FROM CombinedStudents
    ) u
)
SELECT
    cb.SchoolYear AS schoolYear,
    cb.InstId AS instId,
    cb.ClassBookId AS classBookId,
    cb.BookType as bookType,
    (SELECT
        s.PersonId AS personId,
        s.Classid AS classId
    FROM Students s
    WHERE s.SchoolYear = cb.SchoolYear
        AND s.InstId = cb.InstId
        AND s.ClassBookId = cb.ClassBookId
    FOR JSON PATH
    ) AS students,
    (SELECT
        sl.ScheduleLessonId AS scheduleLessonId,
        sl.CurriculumId AS curriculumId,
        sl.[Date] AS [date]
    FROM [school_books].[Schedule] sch
    JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
    WHERE sch.SchoolYear = cb.SchoolYear AND sch.ClassBookId = cb.ClassBookId
    FOR JSON PATH
    ) AS scheduleLessons
FROM [school_books].[ClassBook] cb
WHERE
    cb.SchoolYear IN (2022)
    AND cb.InstId IN (300125, 2206409)
    AND EXISTS (
        SELECT 1
        FROM Students s
        WHERE s.SchoolYear = cb.SchoolYear
            AND s.InstId = cb.InstId
            AND s.ClassBookId = cb.ClassBookId
    ) AND EXISTS (
        SELECT 1
        FROM [school_books].[Schedule] sch
        JOIN [school_books].[ScheduleLesson] sl ON sch.SchoolYear = sl.SchoolYear AND sch.ScheduleId = sl.ScheduleId
        WHERE sch.SchoolYear = cb.SchoolYear AND sch.ClassBookId = cb.ClassBookId
    )
FOR JSON PATH
