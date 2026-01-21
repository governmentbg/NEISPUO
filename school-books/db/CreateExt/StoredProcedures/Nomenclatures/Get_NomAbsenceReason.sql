PRINT 'Create Get_NomAbsenceReason stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_NomAbsenceReason]
AS
    SELECT
        [Id],
        [Name],
        [Description],
        [IsValid],
        [SortOrd]
    FROM [school_books].[AbsenceReason]
GO
