-- Transfer topic students as attendances in DPLR classbooks
INSERT INTO [school_books].[Absence] (
    [SchoolYear],
    [AbsenceId],
    [ClassBookId],
    [PersonId],
    [Date],
    [Type],
    [Term],
    [ExcusedReasonId],
    [ExcusedReasonComment],
    [ScheduleLessonId],
    [TeacherAbsenceId],
    [HisMedicalNoticeId],
    [IsReadFromParent],
    [CreateDate],
    [CreatedBySysUserId],
    [ModifyDate],
    [ModifiedBySysUserId]
)
SELECT
    ts.[SchoolYear],
    NEXT VALUE FOR [school_books].[AbsenceIdSequence] AS [AbsenceId],
    t.[ClassBookId],
    ts.[PersonId],
    t.[Date],
    5 AS [Type],
    1 AS [Term],
    NULL AS [ExcusedReasonId],
    NULL AS [ExcusedReasonComment],
    t.[ScheduleLessonId],
    t.[TeacherAbsenceId],
    NULL AS [HisMedicalNoticeId],
    0 AS [IsReadFromParent],
    t.[CreateDate],
    t.[CreatedBySysUserId],
    t.[CreateDate] AS [ModifyDate],
    t.[CreatedBySysUserId] AS [ModifiedBySysUserId]
FROM
    [school_books].[TopicStudent] ts
INNER JOIN
    [school_books].[Topic] t
    ON ts.[SchoolYear] = t.[SchoolYear] AND ts.[TopicId] = t.[TopicId]
WHERE NOT EXISTS (
    SELECT 1
    FROM [school_books].[Absence] a
    WHERE
        a.[SchoolYear] = t.[SchoolYear]
        AND a.[ScheduleLessonId] = t.[ScheduleLessonId]
        AND a.[PersonId] = ts.[PersonId]
)
GO

DROP TABLE [school_books].[TopicStudent]
GO

-- Created for DPLR students reports in module Students
CREATE OR ALTER VIEW [school_books].[vwEplrHoursTaken] AS
(
    SELECT a.PersonId AS StudentPersonId, a.SchoolYear, cb.InstId, a.ClassBookId, sl.CurriculumId, tt.PersonId AS TeacherPersonId, sl.Date, sl.HourNumber FROM [school_books].[Absence] as a
    INNER JOIN [school_books].[ClassBook] cb ON a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId
    INNER JOIN [school_books].[ScheduleLesson] sl ON a.SchoolYear = sl.SchoolYear AND a.ScheduleLessonId = sl.ScheduleLessonId
    INNER JOIN [school_books].[Topic] t ON a.SchoolYear = t.SchoolYear AND a.ClassBookId = t.ClassBookId AND a.ScheduleLessonId = t.ScheduleLessonId
    INNER JOIN [school_books].[TopicTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicId = tt.TopicId
    WHERE a.Type = 5 AND a.Date > '2024-11-12' -- Date from which the new refinements start

    UNION ALL

    SELECT ts.PersonId AS StudentPersonId, t.SchoolYear, cb.InstId, t.ClassBookId, t.CurriculumId,
           tt.PersonId AS TeacherPersonId, t.Date, t.HourNumber
    FROM [school_books].[TopicDplr] t
    INNER JOIN [school_books].[ClassBook] cb ON t.SchoolYear = cb.SchoolYear AND t.ClassBookId = cb.ClassBookId
    INNER JOIN [school_books].[TopicDplrStudent] ts ON t.SchoolYear = ts.SchoolYear AND t.TopicDplrId = ts.TopicDplrId
    INNER JOIN [school_books].[TopicDplrTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicDplrId = tt.TopicDplrId
    WHERE t.Date > '2024-11-12' -- Date from which the new refinements start
);
GO
