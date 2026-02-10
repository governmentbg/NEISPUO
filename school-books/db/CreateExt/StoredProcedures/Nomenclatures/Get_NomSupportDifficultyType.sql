PRINT 'Create Get_NomSupportDifficultyType stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_NomSupportDifficultyType]
AS
    SELECT
        [Id],
        [Name],
        [Description],
        [IsValid],
        [SortOrd]
    FROM [school_books].[SupportDifficultyType]
GO
