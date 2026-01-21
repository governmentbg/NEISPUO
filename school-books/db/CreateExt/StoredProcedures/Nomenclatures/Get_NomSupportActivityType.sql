PRINT 'Create Get_NomSupportActivityType stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_NomSupportActivityType]
AS
    SELECT
        [Id],
        [Name],
        [Description],
        [IsValid],
        [SortOrd]
    FROM [school_books].[SupportActivityType]
GO
