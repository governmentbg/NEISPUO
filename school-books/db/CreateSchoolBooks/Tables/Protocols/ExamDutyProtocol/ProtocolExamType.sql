PRINT 'Create ProtocolExamType table'
GO

CREATE TABLE [school_books].[ProtocolExamType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ProtocolExamType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'ProtocolExamType', N'Вид на изпита.'

exec school_books.spDescColumn N'ProtocolExamType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'ProtocolExamType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'ProtocolExamType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'ProtocolExamType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'ProtocolExamType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'ProtocolExamType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'ProtocolExamType', N'SortOrd'         , N'Номер на подредба.'
