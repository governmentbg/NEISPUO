PRINT 'Create HisMedicalNoticeBatch table'
GO

CREATE TABLE [school_books].[HisMedicalNoticeBatch] (
    [HisMedicalNoticeBatchId]   INT             NOT NULL IDENTITY(1,1),
    [CreateDate]                DATETIME2       NOT NULL,
    [RequestId]                 NVARCHAR(50)    NOT NULL,
    [Error]                     NVARCHAR(MAX)   NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeBatch] PRIMARY KEY ([HisMedicalNoticeBatchId]),
);
GO

exec school_books.spDescTable  N'HisMedicalNoticeBatch', N'Пакет от медицински бележки изпратени от НЗИС.'

exec school_books.spDescColumn N'HisMedicalNoticeBatch', N'HisMedicalNoticeBatchId'     , N'Уникален системно генериран идентификатор.'
exec school_books.spDescColumn N'HisMedicalNoticeBatch', N'CreateDate'                  , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'HisMedicalNoticeBatch', N'RequestId'                   , N'Идентификатор на HTTP заявката.'
exec school_books.spDescColumn N'HisMedicalNoticeBatch', N'Error'                       , N'Текст на грешката.'
exec school_books.spDescColumn N'HisMedicalNoticeBatch', N'Version'                     , N'Версия.'
