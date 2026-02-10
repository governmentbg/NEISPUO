PRINT 'Create vwClassBooks'
GO

CREATE OR ALTER VIEW [school_books].[vwClassBooks]
WITH SCHEMABINDING
AS
    SELECT
        cb.SchoolYear,
        cb.ClassBookId,
        cb.InstId,
        cb.FullBookName,
        bt.BookTypeName
    FROM
        [school_books].[ClassBook] cb
        INNER JOIN (
            VALUES
                (1, '3-5 Дневник на група/подготвителна група'),
                (2, '3-14 Дневник I - III клас'),
                (3, '3-16 Дневник IV клас'),
                (4, '3-87 Дневник V - XII клас'),
                (5, '3-63 Дневник на група'),
                (6, '3-63.1 Дневник за дейности за подкрепа за личностно развитие'),
                (7, '3-62 Дневник за група/паралелка в център за специална образователна подкрепа')
        ) AS bt([BookType], [BookTypeName]) ON cb.BookType = bt.BookType
GO
