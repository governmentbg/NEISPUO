CREATE OR ALTER VIEW [school_books].[vwStudentClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwNonCombinedStudentClassBooks
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwCombinedStudentClassBooks
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooks
    UNION
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherAbsenceHourClassBooks
    UNION
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherSupportClassBooks
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherClassBooksUnionAll]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooks
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherAbsenceHourClassBooks
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherSupportClassBooks
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherCurriculumClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooksForLvl1
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherCurriculumClassBooksForLvl2
GO

CREATE OR ALTER VIEW [school_books].[vwTeacherScheduleClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherScheduleLessonClassBooks
    UNION
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwTeacherAbsenceHourClassBooks
GO
