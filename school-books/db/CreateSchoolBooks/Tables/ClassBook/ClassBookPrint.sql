PRINT 'Create ClassBookPrint table'
GO

EXEC [school_books].[spCreateIdSequence] N'ClassBookPrint'
GO

CREATE TABLE [school_books].[ClassBookPrint] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [ClassBookId]               INT              NOT NULL,
    [ClassBookPrintId]          INT              NOT NULL,
    [Status]                    INT              NOT NULL,
    [CreateDate]                DATETIME2        NOT NULL,
    [StatusDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [BlobId]                    INT              NULL,
    [ContentHash]               NVARCHAR(40)     NULL,
    [IsFinal]                   BIT              NOT NULL,
    [IsExternal]                BIT              NOT NULL,
    [IsSigned]                  BIT              NOT NULL,

    CONSTRAINT [PK_ClassBookPrint] PRIMARY KEY ([SchoolYear], [ClassBookId], [ClassBookPrintId]),
    CONSTRAINT [FK_ClassBookPrint_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [CHK_ClassBookPrint_Status] CHECK ([Status] IN (
        1,  -- Pending
        2,  -- Processed
        3  -- Errored
    )),
    INDEX [UQ_ClassBookPrint_IsFinal] UNIQUE ([SchoolYear], [ClassBookId], [IsFinal]) WHERE [IsFinal] = 1,

    -- external references
    CONSTRAINT [FK_ClassBookPrint_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'ClassBookPrint', N'Дневник - резултат от принтиране на дневника.'

exec school_books.spDescColumn N'ClassBookPrint', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookPrint', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookPrint', N'ClassBookPrintId'          , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ClassBookPrint', N'Status'                    , N'Статус. 1 - В процес на обработка. 2 - Обработено. 3 - Завършило с грешка.'
exec school_books.spDescColumn N'ClassBookPrint', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ClassBookPrint', N'StatusDate'                , N'Дата на последната промяна на статуса.'
exec school_books.spDescColumn N'ClassBookPrint', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ClassBookPrint', N'BlobId'                    , N'Идентификатор на файловото съдържане.'
exec school_books.spDescColumn N'ClassBookPrint', N'ContentHash'               , N'Хеш на съдържанието на документа.'
exec school_books.spDescColumn N'ClassBookPrint', N'IsFinal'                   , N'Флаг, че е финална версия.'
exec school_books.spDescColumn N'ClassBookPrint', N'IsExternal'                , N'Флаг, че е външно принтирана.'
exec school_books.spDescColumn N'ClassBookPrint', N'IsSigned'                  , N'Флаг, че е подписана версия.'
