PRINT 'Create BookSyncProcedures stored procedures'
GO

CREATE OR ALTER PROCEDURE [school_books].[spStudentYearGradesAdded] @SysUserId INT, @SchoolYear SMALLINT, @InstitutionId INT, @PersonId INT
AS
BEGIN
    RETURN;
END
GO
