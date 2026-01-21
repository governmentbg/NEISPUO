PRINT 'Create ClassBookStudentPrint table'
GO

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

exec school_books.spDescTable  N'ClassBookStudentPrint', N'Дневник - резултат от принтиране на ученически бележник.'

exec school_books.spDescColumn N'ClassBookStudentPrint', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'ClassBookId'               , N'Индентификатор на дневник.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'ClassBookStudentPrintId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ClassBookStudentPrint', N'PersonId'                  , N'Индентификатор на ученик.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'Status'                    , N'Статус. 1 - В процес на обработка. 2 - Обработено. 3 - Завършило с грешка.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ClassBookStudentPrint', N'BlobId'                    , N'Индентификатор на файловото съдържане.'
