PRINT 'Create vwStudentClassBooks'
GO

CREATE OR ALTER VIEW [school_books].[vwStudentClassBooks]
AS
    SELECT * FROM vwNonCombinedStudentClassBooks
    UNION ALL
    SELECT * FROM vwCombinedStudentClassBooks
GO

PRINT 'Create vwTeacherCurriculumClassBooks'
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooksForLvl1]
WITH SCHEMABINDING
AS
    SELECT
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[CurriculumClass] cc ON cb.ClassId = cc.ClassId
        INNER JOIN [inst_year].[CurriculumTeacher] t ON cc.CurriculumId = t.CurriculumId
        INNER JOIN [inst_basic].[StaffPosition] sp ON t.StaffPositionId = sp.StaffPositionId
    WHERE
        cb.ClassIsLvl2 = 0 AND
        cc.IsValid = 1 AND
        t.IsValid = 1
    GROUP BY
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwTeacherCurriculumClassBooksForLvl1]
    ON [school_books].[vwTeacherCurriculumClassBooksForLvl1] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooksForLvl2]
WITH SCHEMABINDING
AS
    SELECT
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
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
    GROUP BY
        sp.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwTeacherCurriculumClassBooksForLvl2]
    ON [school_books].[vwTeacherCurriculumClassBooksForLvl2] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE VIEW [school_books].[vwTeacherCurriculumClassBooks]
AS
    SELECT * FROM vwTeacherCurriculumClassBooksForLvl1
    UNION ALL
    SELECT * FROM vwTeacherCurriculumClassBooksForLvl2
GO

PRINT 'Create vwTeacherSupportClassBooks'
GO

CREATE VIEW [school_books].[vwTeacherSupportClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        st.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [school_books].[Support] s ON cb.SchoolYear = s.SchoolYear AND cb.ClassBookId = s.ClassBookId
        INNER JOIN [school_books].[SupportTeacher] st ON s.SchoolYear = st.SchoolYear AND s.SupportId = st.SupportId
    GROUP BY
        st.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwTeacherSupportClassBooks]
    ON [school_books].[vwTeacherSupportClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

PRINT 'Create vwTeacherClassBooks'
GO

CREATE VIEW [school_books].[vwTeacherClassBooks]
AS
    SELECT * FROM vwTeacherCurriculumClassBooks
    UNION
    SELECT * FROM vwTeacherAbsenceHourClassBooks
    UNION
    SELECT * FROM vwTeacherSupportClassBooks
GO

PRINT 'Create vwTeacherClassBooksUnionAll'
GO

CREATE VIEW [school_books].[vwTeacherClassBooksUnionAll]
AS
    SELECT * FROM vwTeacherCurriculumClassBooks
    UNION ALL
    SELECT * FROM vwTeacherAbsenceHourClassBooks
    UNION ALL
    SELECT * FROM vwTeacherSupportClassBooks
GO
