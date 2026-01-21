PRINT 'Create UpdateScript table'
GO

CREATE TABLE [school_books].[UpdateScript] (
    [ScriptName]    NVARCHAR(50)    NOT NULL UNIQUE,
    [Applied]       DATETIME2       NOT NULL,

    CONSTRAINT [PK_UpdateScript] PRIMARY KEY ([ScriptName])
);
GO

exec school_books.spDescTable  N'UpdateScript', N'Скрипт за промяна по school_books базата.'

exec school_books.spDescColumn N'UpdateScript', N'ScriptName'            , N'Име на скрипта.'
exec school_books.spDescColumn N'UpdateScript', N'Applied'               , N'Дата и час, когато скрипта е приложен.'
