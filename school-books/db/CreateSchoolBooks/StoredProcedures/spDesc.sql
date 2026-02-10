PRINT 'Create spDescTable stored procedure'
GO

CREATE OR ALTER PROCEDURE [school_books].[spDescTable] @Table nvarchar(200), @value nvarchar(500)
AS
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=@value, @level0type=N'SCHEMA', @level0name=N'school_books', @level1type=N'TABLE', @level1name=@Table
GO

PRINT 'Create spDescTable stored procedure'
GO

CREATE OR ALTER PROCEDURE [school_books].[spDescColumn] @Table nvarchar(200), @column nvarchar(200), @value nvarchar(500)
AS
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=@value, @level0type=N'SCHEMA', @level0name=N'school_books', @level1type=N'TABLE', @level1name=@Table, @level2type=N'COLUMN', @level2name=@column
GO
