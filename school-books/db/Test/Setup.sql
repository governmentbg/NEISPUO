
USE [$(dbName)]
GO

:r "./Test/tSQLt/tSQLt.class.sql"
GO

EXEC tSQLt.NewTestClass 'tests';
GO
