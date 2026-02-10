BEGIN TRANSACTION

CREATE TABLE [school_books].[temp_TopicTeacher_deleted] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,
);

WITH InvalidTopicTeachers AS (
    SELECT
        t.SchoolYear, t.TopicId, sp.PersonID
    FROM
        school_books.Topic t
        INNER JOIN school_books.ScheduleLesson sl ON t.SchoolYear = sl.SchoolYear AND t.ScheduleLessonId = sl.ScheduleLessonId
        INNER JOIN inst_year.CurriculumTeacher ct ON sl.CurriculumId = ct.CurriculumID
        INNER JOIN inst_basic.StaffPosition sp ON ct.StaffPositionID = sp.StaffPositionID
    WHERE
        t.SchoolYear = 2023 AND
        t.TeacherAbsenceId IS NULL AND
        t.CreateDate BETWEEN '2023-10-17 11:45' AND '2023-10-18 10:45' AND
        ct.IsValid = 0
)
DELETE tt
OUTPUT DELETED.* INTO school_books.temp_TopicTeacher_deleted
FROM
    school_books.TopicTeacher tt
    INNER JOIN InvalidTopicTeachers itt
        ON tt.SchoolYear = itt.SchoolYear AND tt.TopicId = itt.TopicId AND tt.PersonId = itt.PersonID

--COMMIT

BEGIN TRANSACTION

CREATE TABLE [school_books].[temp_TopicTeacher_inserted] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,
);

INSERT INTO
    [school_books].[TopicTeacher] (
        [SchoolYear],
        [TopicId],
        [PersonId],
        [IsReplTeacher]
    )
OUTPUT INSERTED.* INTO school_books.temp_TopicTeacher_inserted
SELECT
    d.[SchoolYear],
    d.[TopicId],
    d.[PersonId],
    d.[IsReplTeacher]
FROM
    school_books.temp_TopicTeacher_deleted d
    INNER JOIN school_books.Topic t ON d.SchoolYear = t.SchoolYear AND d.TopicId = t.TopicId
    INNER JOIN school_books.ScheduleLesson sl ON t.SchoolYear = sl.SchoolYear AND t.ScheduleLessonId = sl.ScheduleLessonId
    INNER JOIN inst_year.CurriculumTeacher ct ON sl.CurriculumId = ct.CurriculumID
    INNER JOIN inst_basic.StaffPosition sp ON ct.StaffPositionID = sp.StaffPositionID AND d.PersonId = sp.PersonID
WHERE
    ct.IsValid = 1

--COMMIT

BEGIN TRANSACTION

CREATE TABLE [school_books].[temp_TopicTeacher_inserted2] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,
);

WITH EmptyTopics AS (
    SELECT
        t.*
    FROM
        school_books.Topic t
        LEFT JOIN school_books.TopicTeacher tt ON t.TopicId = tt.TopicId
    WHERE
        t.SchoolYear = 2023 AND
        t.TeacherAbsenceId IS NULL AND
        tt.TopicId IS NULL
),
EmptyTopicsWithOnlyOneTeacherInDeleted AS (
    SELECT
        d.SchoolYear, d.TopicId
    FROM
        school_books.temp_TopicTeacher_deleted d
        INNER JOIN EmptyTopics et ON d.SchoolYear = et.SchoolYear AND d.TopicId = et.TopicId
    GROUP BY d.SchoolYear, d.TopicId, et.ScheduleLessonId
    HAVING count(*) = 1
)
INSERT INTO
    [school_books].[TopicTeacher] (
        [SchoolYear],
        [TopicId],
        [PersonId],
        [IsReplTeacher]
    )
OUTPUT INSERTED.*  INTO school_books.temp_TopicTeacher_inserted2
SELECT
    d.[SchoolYear],
    d.[TopicId],
    d.[PersonId],
    d.[IsReplTeacher]
FROM
    school_books.temp_TopicTeacher_deleted d
    INNER JOIN EmptyTopicsWithOnlyOneTeacherInDeleted t ON d.SchoolYear = t.SchoolYear AND d.TopicId = t.TopicId

--COMMIT

BEGIN TRANSACTION

CREATE TABLE [school_books].[temp_TopicTeacher_inserted3] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,
    [IsReplTeacher]             BIT             NOT NULL,
);

WITH EmptyTopics AS (
    SELECT
        t.*
    FROM
        school_books.Topic t
        LEFT JOIN school_books.TopicTeacher tt ON t.TopicId = tt.TopicId
    WHERE
        t.SchoolYear = 2023 AND
        t.TeacherAbsenceId IS NULL AND
        tt.TopicId IS NULL
)
INSERT INTO
    [school_books].[TopicTeacher] (
        [SchoolYear],
        [TopicId],
        [PersonId],
        [IsReplTeacher]
    )
OUTPUT INSERTED.*  INTO school_books.temp_TopicTeacher_inserted3
SELECT
    d.[SchoolYear],
    d.[TopicId],
    d.[PersonId],
    d.[IsReplTeacher]
FROM
    school_books.temp_TopicTeacher_deleted d
    INNER JOIN EmptyTopics et ON d.SchoolYear = et.SchoolYear AND d.TopicId = et.TopicId

--COMMIT
