PRINT 'Create vwTeacherCurriculumClassBooks'
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooksForLvl1]
AS
    SELECT
        DISTINCT
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[CurriculumClass] cc ON cb.ClassId = cc.ClassId
        INNER JOIN [inst_year].[CurriculumTeacher] t ON cc.CurriculumId = t.CurriculumId
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionId = sp.StaffPositionId
    WHERE
        cb.ClassIsLvl2 = 0 AND
        cc.IsValid = 1 AND
        t.IsValid = 1
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooksForLvl2]
AS
    SELECT
        DISTINCT
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.ClassId = cg.ClassId
        INNER JOIN [inst_year].[CurriculumClass] cc ON cg.ParentClassId = cc.ClassId
        INNER JOIN [inst_year].[CurriculumTeacher] t ON cc.CurriculumId = t.CurriculumId
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionId = sp.StaffPositionId
    WHERE
        cb.ClassIsLvl2 = 1 AND
        cc.IsValid = 1 AND
        t.IsValid = 1
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM [school_books].[vwTeacherCurriculumClassBooksForLvl1]
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM [school_books].[vwTeacherCurriculumClassBooksForLvl2]
GO
