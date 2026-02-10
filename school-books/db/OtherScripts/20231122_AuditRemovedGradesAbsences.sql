-- create work table
select *
into #AuditWork
from logs.Audit
where
    AuditModuleId = 301 and
    DateUtc > '2023-09-15' and
    InstId = 1601307

--drop table #AuditWork


-- analyze create/remove pairs
-- most objects should be with 2 occurences
-- except:
-- 1 - missing creates due to some problem with logging
-- 3 - create - excuse - remove
-- 3 - create - convert to late - remove

select CONCAT_WS('$', ObjectName, ObjectId), count(*) from #AuditWork where
    CONCAT_WS('$', ObjectName, ObjectId) IN (
        select
            CONCAT_WS('$', ObjectName, ObjectId)
        from #AuditWork
        where
            (
            [Action] like 'RemoveGrade%' or
            [Action] like 'RemoveAbsence%'
            )
    )
group by
    CONCAT_WS('$', ObjectName, ObjectId)
order by 2

-- removed grades
WITH CreatedGradesJson AS (
    SELECT
        DateUtc,
        AuditCorrelationId,
        ObjectId,
        ROW_NUMBER() OVER (PARTITION BY AuditCorrelationId ORDER BY ObjectId) AS RowNumber,
        Data
    FROM #AuditWork
    WHERE
        Action = 'CreateGradeCommand executed'
),
CreatedGrades AS (
    SELECT
        DateUtc,
        AuditCorrelationId,
        RowNumber,
        ObjectId AS GradeId,
        JSON_VALUE(Data, '$.ClassBookId') AS ClassBookId,
        JSON_VALUE(Data, '$.CurriculumId') AS CurriculumId,
        JSON_VALUE(Data, '$.Type') AS Type,
        JSON_VALUE(Data, '$.Date') AS Date,
        JSON_VALUE(Data, '$.Term') AS Term,
        JSON_VALUE(Data, '$.ScheduleLessonId') AS ScheduleLessonId,
        JSON_VALUE(Data, '$.TeacherAbsenceId') AS TeacherAbsenceId,
        JSON_VALUE(Data, '$.Students[' + CAST((RowNumber - 1) AS NVARCHAR) + '].PersonId') AS PersonId,
        JSON_VALUE(Data, '$.Students[' + CAST((RowNumber - 1) AS NVARCHAR) + '].DecimalGrade') AS DecimalGrade,
        JSON_VALUE(Data, '$.Students[' + CAST((RowNumber - 1) AS NVARCHAR) + '].QualitativeGrade') AS QualitativeGrade,
        JSON_VALUE(Data, '$.Students[' + CAST((RowNumber - 1) AS NVARCHAR) + '].SpecialGrade') AS SpecialGrade,
        JSON_VALUE(Data, '$.Students[' + CAST((RowNumber - 1) AS NVARCHAR) + '].Comment') AS Comment,
        Data
    FROM
        CreatedGradesJson
),
RemovedGrades AS (
    SELECT
        DateUtc,
        IIF(DateUtc < '2023-10-29 04:00:00', -- DST Ends
            CONVERT(DATETIME, SWITCHOFFSET(CONVERT(DATETIMEOFFSET, DateUtc), '+03:00')),
            CONVERT(DATETIME, SWITCHOFFSET(CONVERT(DATETIMEOFFSET, DateUtc), '+02:00'))
        ) AS Date,
        SysUserId,
        Username,
        ObjectId AS GradeId
    FROM #AuditWork
    WHERE
        [Action] LIKE 'RemoveGrade%'
)
SELECT
    rg.DateUtc,
    rg.Date,
    rg.SysUserId,
    rg.Username,
    cb.FullBookName,
    cg.*,
    p.PersonalID,
    p.FirstName,
    p.MiddleName,
    p.LastName,
    s.SubjectName,
    st.Name
FROM
    RemovedGrades rg
    INNER JOIN CreatedGrades cg on rg.GradeId = cg.GradeId
    INNER JOIN school_books.ClassBook cb on cg.ClassBookId = cb.ClassBookId
    INNER JOIN core.Person p on cg.PersonId = p.PersonID
    INNER JOIN inst_year.Curriculum c on c.CurriculumID = cg.CurriculumId
    INNER JOIN inst_nom.Subject s on c.SubjectID = s.SubjectID
    INNER JOIN inst_nom.SubjectType st on c.SubjectTypeID = st.SubjectTypeID
ORDER BY
    cb.FullBookName,
    rg.[Date]

-- removed absences
WITH CreatedAbsencesJson AS (
    SELECT
        DateUtc,
        AuditCorrelationId,
        ObjectId,
        ROW_NUMBER() OVER (PARTITION BY AuditCorrelationId ORDER BY ObjectId) AS RowNumber,
        Data
    from #AuditWork
    where
        Action = 'CreateAbsenceCommand executed'
),
CreatedAbsences AS (
    SELECT
        DateUtc,
        AuditCorrelationId,
        RowNumber,
        ObjectId AS AbsenceId,
        JSON_VALUE(Data, '$.ClassBookId') AS ClassBookId,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].PersonId') AS PersonId,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].Type') AS Type,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].Date') AS Date,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].ScheduleLessonId') AS ScheduleLessonId,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].TeacherAbsenceId') AS TeacherAbsenceId,
        JSON_VALUE(Data, '$.Absences[' + CAST((RowNumber - 1) AS NVARCHAR) + '].ConvertToLateId') AS ConvertToLateId,
        Data
    FROM
        CreatedAbsencesJson
),
RemovedAbsences AS (
    SELECT
        DateUtc,
        IIF(DateUtc < '2023-10-29 04:00:00', -- DST Ends
            CONVERT(DATETIME, SWITCHOFFSET(CONVERT(DATETIMEOFFSET, DateUtc), '+03:00')),
            CONVERT(DATETIME, SWITCHOFFSET(CONVERT(DATETIMEOFFSET, DateUtc), '+02:00'))
        ) AS Date,
        SysUserId,
        Username,
        ObjectId AS AbsenceId
    FROM #AuditWork
    WHERE
        [Action] LIKE 'RemoveAbsence%'
)
SELECT
    ra.DateUtc,
    ra.Date,
    ra.SysUserId,
    ra.Username,
    cb.FullBookName,
    ca.*,
    p.PersonalID,
    p.FirstName,
    p.MiddleName,
    p.LastName,
    sl.Date,
    sl.HourNumber,
    s.SubjectName,
    st.Name
FROM
    RemovedAbsences ra
    INNER JOIN CreatedAbsences ca on ra.AbsenceId = ca.AbsenceId
    INNER JOIN school_books.ClassBook cb on ca.ClassBookId = cb.ClassBookId
    INNER JOIN core.Person p on ca.PersonId = p.PersonID
    INNER JOIN school_books.ScheduleLesson sl on ca.ScheduleLessonId = sl.ScheduleLessonId and sl.SchoolYear = 2023
    INNER JOIN inst_year.Curriculum c on c.CurriculumID = sl.CurriculumId
    INNER JOIN inst_nom.Subject s on c.SubjectID = s.SubjectID
    INNER JOIN inst_nom.SubjectType st on c.SubjectTypeID = st.SubjectTypeID
ORDER BY
    cb.FullBookName,
    ra.[Date]
