EXEC [school_books].[spCreateIdSequence] N'ClassBookStudentPrint'
GO

CREATE TABLE [school_books].[ClassBookStudentPrint] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [ClassBookStudentPrintId]   INT              NOT NULL,
    [PersonId]                  INT              NOT NULL,
    [Status]                    INT              NOT NULL,
    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [BlobId]                    INT              NULL,

    CONSTRAINT [PK_ClassBookStudentPrint] PRIMARY KEY ([SchoolYear], [ClassBookId], [ClassBookStudentPrintId]),
    CONSTRAINT [FK_ClassBookStudentPrint_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBookStudentPrint_Status] CHECK ([Status] IN (
        1,  -- Pending
        2,  -- Processed
        3  -- Errored
    )),

    -- external references
    CONSTRAINT [FK_ClassBookStudentPrint_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

ALTER TABLE [school_books].[QueueMessage] DROP CONSTRAINT [CHK_QueueMessage_Type]
GO

ALTER TABLE [school_books].[QueueMessage] ADD CONSTRAINT [CHK_QueueMessage_Type] CHECK ([Type] IN (1, 2, 3, 4))
GO
