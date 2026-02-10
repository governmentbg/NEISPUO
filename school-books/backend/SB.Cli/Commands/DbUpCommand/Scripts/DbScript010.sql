EXEC [school_books].[spCreateIdSequence] N'ClassBookPrint'
GO

CREATE TABLE [school_books].[ClassBookPrint] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [ClassBookPrintId]          INT              NOT NULL,
    [Status]                    INT              NOT NULL,
    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [BlobId]                    INT              NULL,

    CONSTRAINT [PK_ClassBookPrint] PRIMARY KEY ([SchoolYear], [ClassBookId], [ClassBookPrintId]),
    CONSTRAINT [FK_ClassBookPrint_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBookPrint_Status] CHECK ([Status] IN (
        1,  -- Pending
        2,  -- Processed
        3  -- Errored
    )),

    -- external references
    CONSTRAINT [FK_ClassBookPrint_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO
