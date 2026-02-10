PRINT 'Create ProtocolExamSubType table'
GO

CREATE TABLE [school_books].[ProtocolExamSubType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ProtocolExamSubType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'ProtocolExamSubType', N'Подвид на изпита.'

exec school_books.spDescColumn N'ProtocolExamSubType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'ProtocolExamSubType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'ProtocolExamSubType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'ProtocolExamSubType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'ProtocolExamSubType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'ProtocolExamSubType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'ProtocolExamSubType', N'SortOrd'         , N'Номер на подредба.'
