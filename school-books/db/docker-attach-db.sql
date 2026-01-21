IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'$(dbName)')
BEGIN
    CREATE DATABASE [$(dbName)] ON
        (FILENAME = '/appdata/$(dbName).mdf')
    FOR ATTACH_REBUILD_LOG
END
GO
