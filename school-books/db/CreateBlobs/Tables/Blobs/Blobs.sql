PRINT 'Create Blobs table'
GO

exec [blobs].[spCreateIdSequence] N'Blobs'
GO

CREATE TABLE [blobs].[Blobs] (
    [BlobId]        INT             NOT NULL,
    [BlobContentId] INT             NOT NULL,
    [FileName]      NVARCHAR(500)   NOT NULL,
    [CreateDate]    DATETIME2       NOT NULL,
    [Version]       ROWVERSION      NOT NULL,

    CONSTRAINT [PK_Blobs] PRIMARY KEY ([BlobId]),
    CONSTRAINT [FK_Blobs_BlobContents] FOREIGN KEY ([BlobContentId]) REFERENCES [blobs].[BlobContents]([BlobContentId])
);
GO
