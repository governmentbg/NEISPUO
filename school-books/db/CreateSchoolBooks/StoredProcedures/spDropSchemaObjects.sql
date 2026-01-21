PRINT 'Create spDropSchemaObjects stored procedure'
GO

CREATE OR ALTER PROCEDURE [school_books].[spDropSchemaObjects] @Schema NVARCHAR(200)
AS

DECLARE @Sql NVARCHAR(MAX) = '';

--constraints
SELECT @Sql = @Sql + 'ALTER TABLE ' + QUOTENAME(@Schema) + '.' + QUOTENAME(t.name) + ' DROP CONSTRAINT ' + QUOTENAME(f.name)  + ';' + CHAR(13)
FROM sys.tables t 
    inner join sys.foreign_keys f on f.parent_object_id = t.object_id 
    inner join sys.schemas s on t.schema_id = s.schema_id
WHERE s.name = @Schema
ORDER BY t.name;

--views
SELECT @Sql = @Sql + 'DROP VIEW ' + QUOTENAME(@Schema) +'.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @Schema AND TABLE_TYPE = 'VIEW'
ORDER BY TABLE_NAME

--tables
SELECT @Sql = @Sql + 'DROP TABLE ' + QUOTENAME(@Schema) +'.' + QUOTENAME(TABLE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = @Schema AND TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME

--procedures
SELECT @Sql = @Sql + 'DROP PROCEDURE ' + QUOTENAME(@Schema) +'.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @Schema AND ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME

--functions
SELECT @Sql = @Sql + 'DROP FUNCTION ' + QUOTENAME(@Schema) +'.' + QUOTENAME(ROUTINE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = @Schema AND ROUTINE_TYPE = 'FUNCTION'
ORDER BY ROUTINE_NAME

--sequences
SELECT @Sql = @Sql + 'DROP SEQUENCE ' + QUOTENAME(@Schema) +'.' + QUOTENAME(SEQUENCE_NAME) + ';' + CHAR(13)
FROM INFORMATION_SCHEMA.SEQUENCES
WHERE SEQUENCE_SCHEMA = @Schema
ORDER BY SEQUENCE_NAME

--types
SELECT @Sql = @Sql + 'DROP TYPE ' + QUOTENAME(@Schema) + '.' + QUOTENAME(t.name) + ';' + CHAR(13)
FROM sys.types t
    INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.is_user_defined = 1 AND
    s.name  = @Schema
ORDER BY s.name

EXECUTE sp_executesql @Sql

GO
