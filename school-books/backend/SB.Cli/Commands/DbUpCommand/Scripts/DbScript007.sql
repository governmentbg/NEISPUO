ALTER TABLE [school_books].[PublicationFile] DROP CONSTRAINT [PK_PublicationFile]
ALTER TABLE [school_books].[PublicationFile] ADD CONSTRAINT [PK_PublicationFile] PRIMARY KEY ([SchoolYear], [PublicationId], [BlobId])
GO

ALTER TABLE [school_books].[PublicationFile] DROP COLUMN [PublicationFileId]
ALTER TABLE [school_books].[PublicationFile] DROP COLUMN [Description]
ALTER TABLE [school_books].[PublicationFile] ADD [FileName] NVARCHAR(500) NOT NULL
GO

DROP SEQUENCE [school_books].[PublicationFileIdSequence]
GO
