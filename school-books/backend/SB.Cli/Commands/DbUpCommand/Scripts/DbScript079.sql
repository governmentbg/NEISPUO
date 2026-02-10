GO

EXEC [school_books].[spCreateIdSequence] N'ClassBookStatusChange'
GO

CREATE TABLE [school_books].[ClassBookStatusChange] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [ClassBookStatusChangeId]   INT              NOT NULL,
    [Type]                      INT              NOT NULL,
    [ChangeDate]                DATETIME2        NOT NULL,
    [ChangedBySysUserId]        INT              NOT NULL,
    [Signees]                   NVARCHAR(1000)   NULL,
    [IsLast]                    BIT              NOT NULL,

    CONSTRAINT [PK_ClassBookStatusChange] PRIMARY KEY ([SchoolYear], [ClassBookId], [ClassBookStatusChangeId]),
    CONSTRAINT [FK_ClassBookStatusChange_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBookStatusChange_Type] CHECK ([Type] IN (
        1, -- Finalized
        2, -- Signed
        3, -- FinalizedAndSigned
        4  -- Unfinalized
    )),
    INDEX [UQ_ClassBookStatusChange_IsLast] UNIQUE ([SchoolYear], [ClassBookId], [IsLast]) WHERE [IsLast] = 1,

    -- external references
    CONSTRAINT [FK_ClassBookStatusChange_ChangedBySysUserId] FOREIGN KEY ([ChangedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO
