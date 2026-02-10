GO
PRINT 'Insert ScheduleHour'
GO

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
        cb.InstId IN ($(TestClassBookDataInstitutions))
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
        cg.InstitutionID IN ($(TestClassBookDataInstitutions)) AND
        c.[SubjectTypeId] NOT IN (152, 153, 154, 155, 156) -- profiling subject
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
GO
