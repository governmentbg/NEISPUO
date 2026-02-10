CREATE OR ALTER VIEW [school_books].[vwStudentClassBooks]
AS
    WITH NonCombinedStudentClassBooks AS (
        SELECT
            sc.PersonId,
            cb.SchoolYear,
            cb.InstId,
            cb.ClassBookId,
            cb.ClassId
        FROM
            [school_books].[ClassBook] cb
            INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassId
            INNER JOIN [student].[StudentClass] sc ON cg.SchoolYear = sc.SchoolYear AND cg.ClassId = sc.ClassId
        WHERE
            cb.ClassIsLvl2 = 0
    ),
    CombinedStudentClassBooks AS (
        SELECT
            sc.PersonId,
            cb.SchoolYear,
            cb.InstId,
            cb.ClassBookId,
            cb.ClassId
        FROM
            [school_books].[ClassBook] cb
            INNER JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND cb.ClassId = sc.ClassId
        WHERE
            cb.ClassIsLvl2 = 1
    )
    SELECT
        u.PersonId,
        u.SchoolYear,
        u.InstId,
        u.ClassBookId,
        u.ClassId
    FROM (
      SELECT * FROM NonCombinedStudentClassBooks
      UNION ALL
      SELECT * FROM CombinedStudentClassBooks
    ) u
GO
