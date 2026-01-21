PRINT 'Create Blobs partitioning'
GO

USE [master]
GO

DECLARE @SQL NVARCHAR(MAX) = '';
DECLARE @FileGroupsCount INT = 8;
DECLARE @i INT = 1;

WHILE (@i <= @FileGroupsCount)
BEGIN
    SELECT @SQL = @SQL + '
        ALTER DATABASE [$(dbName)] ADD FILEGROUP [FG_BlobContents' + CAST(@i AS NVARCHAR(MAX)) + '];

        ALTER DATABASE [$(dbName)] ADD FILE
        (
        NAME = [$(dbName)_BlobContents' + CAST(@i AS NVARCHAR(MAX)) + '],
        FILENAME = ''' + CAST(SERVERPROPERTY('INSTANCEDEFAULTDATAPATH') AS NVARCHAR(MAX)) + '$(dbName)_BlobContents' + CAST(@i AS NVARCHAR(MAX)) + '.ndf''
        )
        TO FILEGROUP [FG_BlobContents' + CAST(@i AS NVARCHAR(MAX)) + '];';

        SET @i  = @i  + 1;
END

EXEC sp_executesql @SQL;

USE [$(dbName)];

-- create blobs partition function

CREATE PARTITION FUNCTION pfBlobContents (INT)
AS RANGE RIGHT FOR VALUES (
    1,
    1000000,
    2000000,
    3000000,
    4000000,
    5000000,
    6000000,
    7000000,
    8000000 -- reserved, must be empty
);

-- create blobs partition scheme

CREATE PARTITION SCHEME psBlobContents
AS PARTITION pfBlobContents TO
(
    [FG_BlobContents1], -- < 1
    [FG_BlobContents1], -- 1 <= i < 1000000
    [FG_BlobContents2], -- 1000000 <= i < 2000000
    [FG_BlobContents3], -- 2000000 <= i < 3000000
    [FG_BlobContents4], -- 3000000 <= i < 4000000
    [FG_BlobContents5], -- 4000000 <= i < 5000000
    [FG_BlobContents6], -- 5000000 <= i < 6000000
    [FG_BlobContents7], -- 6000000 <= i < 7000000
    [FG_BlobContents8], -- 7000000 <= i < 8000000
    [FG_BlobContents8]  -- reserved
);
GO
