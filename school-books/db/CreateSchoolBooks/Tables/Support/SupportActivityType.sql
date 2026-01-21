PRINT 'Create SupportActivityType table'
GO

CREATE TABLE [school_books].[SupportActivityType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_SupportActivityType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'SupportActivityType', N'Вид дейност.'

exec school_books.spDescColumn N'SupportActivityType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'SupportActivityType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'SupportActivityType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'SupportActivityType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'SupportActivityType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'SupportActivityType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'SupportActivityType', N'SortOrd'         , N'Номер на подредба.'
