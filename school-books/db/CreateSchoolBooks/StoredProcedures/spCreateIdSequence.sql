PRINT 'Create spCreateIdSequence stored procedure'
GO

CREATE OR ALTER PROCEDURE [school_books].[spCreateIdSequence] @tableName NVARCHAR(200)
AS
    DECLARE @start INT = 100000
    DECLARE @incrementBy INT = 1
    DECLARE @sql NVARCHAR(MAX) = N'CREATE SEQUENCE [school_books].[' + @tableName + 'IdSequence] AS INT START WITH ' + STR(@start) + ' INCREMENT BY ' + STR(@incrementBy) + ' NO CYCLE'
    EXEC SP_EXECUTESQL @sql
GO
