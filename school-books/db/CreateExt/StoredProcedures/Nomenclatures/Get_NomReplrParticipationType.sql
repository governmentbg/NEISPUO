PRINT 'Create Get_NomReplrParticipationType stored procedure'
GO

CREATE OR ALTER PROCEDURE [ext].[Get_NomReplrParticipationType]
AS
    SELECT
        [Id],
        [Name],
        [Description],
        [IsValid],
        [SortOrd]
    FROM [school_books].[ReplrParticipationType]
GO
