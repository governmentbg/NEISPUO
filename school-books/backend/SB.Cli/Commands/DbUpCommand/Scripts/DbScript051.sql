PRINT 'Create vwStudentsWithClassBookData'
GO

CREATE VIEW [school_books].[vwStudentsWithClassBookData]
AS
    WITH StudentData AS (
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Absence]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Attendance]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[FirstGradeResult]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[GradeResult]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Grade]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Remark]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[IndividualWork]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[PgResult]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Sanction]
        UNION ALL
            SELECT [SchoolYear], [ClassBookId], [PersonId] FROM [school_books].[Schedule]
        UNION ALL
            SELECT n.[SchoolYear], n.[ClassBookId], ns.[PersonId]
            FROM
                [school_books].[Note] n
                INNER JOIN [school_books].[NoteStudent] ns ON n.[SchoolYear] = ns.[SchoolYear] AND n.[NoteId] = ns.[NoteId]
        UNION ALL
            SELECT s.[SchoolYear], s.[ClassBookId], ss.[PersonId]
            FROM
                [school_books].[Support] s
                INNER JOIN [school_books].[SupportStudent] ss ON s.[SchoolYear] = ss.[SchoolYear] AND s.[SupportId] = ss.[SupportId]
    )
    SELECT
        cb.SchoolYear,
        cb.InstId,
        cb.ClassBookId,
        p.PersonId,
        p.FirstName,
        p.MiddleName,
        p.LastName
    FROM
        [core].[Person] p
        CROSS APPLY (
            SELECT
                SchoolYear,
                InstId,
                ClassBookId
            FROM
                [school_books].[ClassBook] cb
            WHERE
                EXISTS (
                    SELECT 1 FROM StudentData sd
                    WHERE
                        sd.[SchoolYear] = cb.[SchoolYear] AND
                        sd.[ClassBookId] = cb.[ClassBookId] AND
                        sd.[PersonId] = p.[PersonId])
        ) cb
GO
