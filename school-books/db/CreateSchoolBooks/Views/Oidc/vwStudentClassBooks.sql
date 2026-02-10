PRINT 'Create vwStudentClassBooks'
GO

CREATE VIEW [school_books].[vwNonCombinedStudentClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassId
        INNER JOIN [student].[StudentClass] sc ON cg.SchoolYear = sc.SchoolYear AND cg.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 0 AND
        cg.IsValid = 1 AND
        sc.IsNotPresentForm = 0
    GROUP BY
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwNonCombinedStudentClassBooks]
    ON [school_books].[vwNonCombinedStudentClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE VIEW [school_books].[vwCombinedStudentClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId,
        COUNT_BIG(*) AS COUNT
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND cb.ClassId = sc.ClassId
    WHERE
        cb.ClassIsLvl2 = 1 AND
        sc.IsNotPresentForm = 0
    GROUP BY
        sc.PersonId,
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        cb.ClassId
GO

CREATE UNIQUE CLUSTERED INDEX [UQ_vwCombinedStudentClassBooks]
    ON [school_books].[vwCombinedStudentClassBooks] ([PersonId], [SchoolYear], [InstId], [ClassBookId], [ClassId])
GO

CREATE VIEW [school_books].[vwStudentClassBooks]
AS
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwNonCombinedStudentClassBooks WITH (NOEXPAND)
    UNION ALL
    SELECT PersonId, SchoolYear, InstId, ClassBookId, ClassId FROM vwCombinedStudentClassBooks WITH (NOEXPAND)
GO
