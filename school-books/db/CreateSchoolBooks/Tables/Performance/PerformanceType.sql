PRINT 'Create PerformanceType table'
GO

CREATE TABLE [school_books].[PerformanceType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_PerformanceType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'PerformanceType', N'Вид на изявата.'

exec school_books.spDescColumn N'PerformanceType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'PerformanceType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'PerformanceType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'PerformanceType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'PerformanceType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'PerformanceType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'PerformanceType', N'SortOrd'         , N'Номер на подредба.'
