PRINT 'Create SupportDifficultyType table'
GO

CREATE TABLE [school_books].[SupportDifficultyType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_SupportDifficultyType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'SupportDifficultyType', N'Вид затруднение.'

exec school_books.spDescColumn N'SupportDifficultyType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'SupportDifficultyType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'SupportDifficultyType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'SupportDifficultyType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'SupportDifficultyType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'SupportDifficultyType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'SupportDifficultyType', N'SortOrd'         , N'Номер на подредба.'
