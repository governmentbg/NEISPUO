PRINT 'Create vwEplrHoursTaken'
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
