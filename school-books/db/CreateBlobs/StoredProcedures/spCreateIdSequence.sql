PRINT 'Create spCreateIdSequence stored procedure'
GO

CREATE OR ALTER PROCEDURE [blobs].[spCreateIdSequence] @tableName NVARCHAR(200)
AS
    DECLARE @start INT = 1000
    DECLARE @incrementBy INT = 1
    DECLARE @sql NVARCHAR(MAX) = N'CREATE SEQUENCE [blobs].[' + @tableName + 'IdSequence] AS INT START WITH ' + STR(@start) + ' INCREMENT BY ' + STR(@incrementBy) + ' NO CYCLE'
    EXEC SP_EXECUTESQL @sql
GO
