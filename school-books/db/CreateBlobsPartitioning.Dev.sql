PRINT 'Create Blobs partitioning'
GO

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
AS PARTITION pfBlobContents ALL TO
(
    [PRIMARY]
);
GO
