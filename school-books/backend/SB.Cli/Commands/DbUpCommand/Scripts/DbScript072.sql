DROP VIEW [school_books].[vwTeacherClassBooks]
GO

DROP VIEW [school_books].[vwTeacherClassBooksUnionAll]
GO

DROP VIEW [school_books].[vwTeacherScheduleClassBooks]
GO

DROP INDEX [UQ_vwTeacherScheduleLessonClassBooks] ON [school_books].[vwTeacherScheduleLessonClassBooks]
GO

DROP VIEW [school_books].[vwTeacherScheduleLessonClassBooks]
GO

DROP INDEX [UQ_vwTeacherAbsenceHourClassBooks] ON [school_books].[vwTeacherAbsenceHourClassBooks]
GO

DROP VIEW [school_books].[vwTeacherAbsenceHourClassBooks]
GO

DROP INDEX [UQ_vwTeacherSupportClassBooks] ON [school_books].[vwTeacherSupportClassBooks]
GO

DROP VIEW [school_books].[vwTeacherSupportClassBooks]
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherCurriculumClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooksForLvl1 WITH (NOEXPAND)
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooksForLvl2 WITH (NOEXPAND)
GO

CREATE OR ALTER VIEW [school_books].[vwStudentClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwNonCombinedStudentClassBooks WITH (NOEXPAND)
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwCombinedStudentClassBooks WITH (NOEXPAND)
GO


DROP INDEX [IX_ScheduleLesson_CurriculumId] ON [school_books].[ScheduleLesson]
GO

CREATE NONCLUSTERED INDEX [IX_ScheduleLesson_CurriculumId] ON [school_books].[ScheduleLesson] ([CurriculumId] ASC) INCLUDE ([ScheduleId])
GO

CREATE NONCLUSTERED INDEX [IX_SupportTeacher_PersonId] ON [school_books].[SupportTeacher] ([SchoolYear], [PersonId])
GO

CREATE NONCLUSTERED INDEX [IX_TeacherAbsenceHour_ReplTeacherPersonId] ON [school_books].[TeacherAbsenceHour] ([SchoolYear], [ReplTeacherPersonId])
GO
