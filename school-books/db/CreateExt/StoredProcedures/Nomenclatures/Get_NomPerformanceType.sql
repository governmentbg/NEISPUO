PRINT 'Create Get_NomPerformanceType stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_NomPerformanceType]
AS
    SELECT
        [Id],
        [Name],
        [Description],
        [IsValid],
        [SortOrd]
    FROM [school_books].[PerformanceType]
GO
