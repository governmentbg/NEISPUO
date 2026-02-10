PRINT 'Create database $(dbName)'
GO

USE [master]
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'$(dbName)')
BEGIN
    ALTER DATABASE [$(dbName)] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [$(dbName)]
END
GO

DECLARE @SQL NVARCHAR(MAX) = ''

SELECT @SQL = @SQL + '
  CREATE DATABASE [$(dbName)] ON
  PRIMARY
  (
    NAME = [$(dbName)],
    FILENAME = ''' + CAST(SERVERPROPERTY('INSTANCEDEFAULTDATAPATH') AS NVARCHAR(MAX)) + '$(dbName).mdf''
  )
  LOG ON
  (
    NAME = [$(dbName)_log],
    FILENAME = '''  + CAST(SERVERPROPERTY('INSTANCEDEFAULTLOGPATH') AS NVARCHAR(MAX)) + '$(dbName)_log.ldf''
  )
  COLLATE Cyrillic_General_CI_AS';

EXEC sp_executesql @SQL
GO

ALTER DATABASE [$(dbName)] SET ALLOW_SNAPSHOT_ISOLATION ON
GO

ALTER DATABASE [$(dbName)] SET READ_COMMITTED_SNAPSHOT ON
GO

ALTER DATABASE [$(dbName)] SET RECOVERY SIMPLE
GO

-- create blobs partitioning

:r "./CreateBlobsPartitioning.Dev.sql"

USE [$(dbName)]
GO

-- create blobs schema

CREATE SCHEMA [blobs]
GO

:r "./CreateBlobs/Create.sql"

-- create logs partitioning

:r "./CreateLogsPartitioning.Dev.sql"

USE [$(dbName)]
GO

-- create logs schema

CREATE SCHEMA [logs]
GO

:r "./CreateLogs/Create.sql"

-- create other schemas

:r "./CreateOtherSchemas/Create.sql"

-- create school_books schema

CREATE SCHEMA [school_books]
GO

:r "./CreateSchoolBooks/Create.sql"
GO

-- create ext schema

CREATE SCHEMA [ext]
GO

:r "./CreateExt/Create.sql"
GO

-- Insert test data

-- Set sql env variables here, because in package.json scripts does not work correctly on linux environment
:setvar TestClassBookDataInstitutions "2206409,300125,2999963,200277,607055,1690180"
:setvar ScriptSchoolYears "2021,2022,2023,2024,2025"

:r "./Insert/Insert.sql"
