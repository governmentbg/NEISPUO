PRINT 'Create PublicationFile table'
GO

CREATE TABLE [school_books].[PublicationFile] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [PublicationId]           INT              NOT NULL,
    [BlobId]                  INT              NOT NULL,
    [FileName]                NVARCHAR(500)    NOT NULL,

    CONSTRAINT [PK_PublicationFile] PRIMARY KEY ([SchoolYear], [PublicationId], [BlobId]),
    CONSTRAINT [FK_PublicationFile_Publication] FOREIGN KEY ([SchoolYear], [PublicationId]) REFERENCES [school_books].[Publication] ([SchoolYear], [PublicationId]),
);
GO

exec school_books.spDescTable  N'PublicationFile', N'Публикация - прикачен файл.'

exec school_books.spDescColumn N'PublicationFile', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'PublicationFile', N'PublicationId'             , N'Идентификатор на публикация.'
exec school_books.spDescColumn N'PublicationFile', N'BlobId'                    , N'Идентификатор на файловото съдържане.'
exec school_books.spDescColumn N'PublicationFile', N'FileName'                  , N'Име на файла.'
