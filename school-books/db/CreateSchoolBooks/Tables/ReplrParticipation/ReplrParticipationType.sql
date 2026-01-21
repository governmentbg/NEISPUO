PRINT 'Create ReplrParticipationType table'
GO

CREATE TABLE [school_books].[ReplrParticipationType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_ReplrParticipationType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'ReplrParticipationType', N'Вид на участието в РЕПЛР.'

exec school_books.spDescColumn N'ReplrParticipationType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'ReplrParticipationType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'ReplrParticipationType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'ReplrParticipationType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'ReplrParticipationType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'ReplrParticipationType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'ReplrParticipationType', N'SortOrd'         , N'Номер на подредба.'
