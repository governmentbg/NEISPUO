GO

ALTER TABLE [school_books].[ClassBook]
ADD
    [BasicClassName] NVARCHAR(255) NULL,
    [FullBookName] AS (
        CASE WHEN COALESCE(LEN([BookName]), 0) > 0
                AND COALESCE(LEN([BasicClassName]), 0) > 0
            THEN [BasicClassName] + ' - ' + [BookName]
            ELSE COALESCE([BasicClassName], [BookName])
        END
    );
GO

UPDATE cb
SET [BasicClassName] = bc.[Name]
FROM [school_books].[ClassBook] cb
JOIN [inst_nom].[BasicClass] bc ON cb.[BasicClassId] = bc.[BasicClassId]
WHERE cb.[BasicClassId] IS NOT NULL
GO
