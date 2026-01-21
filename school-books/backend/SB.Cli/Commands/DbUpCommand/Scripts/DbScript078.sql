GO

ALTER TABLE [school_books].[ClassBookPrint]
ADD
    [ContentHash] NVARCHAR(40) NULL,
    [IsSigned] BIT NOT NULL CONSTRAINT [DF_ClassBookPrint_IsSigned] DEFAULT 0,
    [IsExternal] BIT NOT NULL CONSTRAINT [DF_ClassBookPrint_IsExternal] DEFAULT 0
GO

ALTER TABLE [school_books].[ClassBookPrint]
DROP
    CONSTRAINT [DF_ClassBookPrint_IsSigned],
    CONSTRAINT [DF_ClassBookPrint_IsExternal]
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
