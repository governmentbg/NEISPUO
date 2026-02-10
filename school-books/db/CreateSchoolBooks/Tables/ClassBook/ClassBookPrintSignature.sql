PRINT 'Create ClassBookPrintSignature table'
GO

CREATE TABLE [school_books].[ClassBookPrintSignature] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [ClassBookPrintId]          INT              NOT NULL,
    [Index]                     INT              NOT NULL,

    [Issuer]                    NVARCHAR(MAX)    NOT NULL,
    [Subject]                   NVARCHAR(MAX)    NOT NULL,
    [Thumbprint]                NVARCHAR(40)     NOT NULL,
    [ValidFrom]                 DATETIME2        NOT NULL,
    [ValidTo]                   DATETIME2        NOT NULL,

    CONSTRAINT [PK_ClassBookPrintSignature] PRIMARY KEY ([SchoolYear], [ClassBookId], [ClassBookPrintId], [Index]),
    CONSTRAINT [FK_ClassBookPrintSignature_ClassBookPrint] FOREIGN KEY ([SchoolYear], [ClassBookId], [ClassBookPrintId])
        REFERENCES [school_books].[ClassBookPrint] ([SchoolYear], [ClassBookId], [ClassBookPrintId]),
);
GO

exec school_books.spDescTable  N'ClassBookPrintSignature', N'Дневник - подпис към резултат от принтиране на дневника.'

exec school_books.spDescColumn N'ClassBookPrintSignature', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'ClassBookPrintId'          , N'Идентификатор на принтиране.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'Index'                     , N'Пореден номер.'

exec school_books.spDescColumn N'ClassBookPrintSignature', N'Issuer'                    , N'Издател на подписа.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'Subject'                   , N'Подписано от.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'Thumbprint'                , N'Идентификатор на сертификата.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'ValidFrom'                 , N'Сертификата е валиден от.'
exec school_books.spDescColumn N'ClassBookPrintSignature', N'ValidTo'                   , N'Сертификата е валиден до.'
