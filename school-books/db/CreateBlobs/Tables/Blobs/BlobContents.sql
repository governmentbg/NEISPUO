PRINT 'Create BlobContents table'
GO

exec [blobs].[spCreateIdSequence] N'BlobContents'
GO

CREATE TABLE [blobs].[BlobContents] (
    [BlobContentId] INT             NOT NULL,
    [Hash]          NVARCHAR(64)    NULL,
    [Size]          BIGINT          NULL,
    [Content]       VARBINARY(MAX)  NULL,
    [CreateDate]    DATETIME2       NOT NULL,
    [ModifyDate]    DATETIME2       NOT NULL,
    [Version]       ROWVERSION      NOT NULL,

    CONSTRAINT [PK_BlobContents] PRIMARY KEY ([BlobContentId])
)
ON psBlobContents([BlobContentId])
GO

CREATE UNIQUE INDEX [UQ_BlobContents_Hash_Size]
    ON [blobs].[BlobContents]([Hash], [Size]) WHERE [Hash] IS NOT NULL AND [Size] IS NOT NULL
ON [PRIMARY]
GO
