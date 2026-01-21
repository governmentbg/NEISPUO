PRINT 'Create AbsenceReason table'
GO

CREATE TABLE [school_books].[AbsenceReason] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_AbsenceReason] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'AbsenceReason', N'Причини за отсъствието.'

exec school_books.spDescColumn N'AbsenceReason', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'AbsenceReason', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'AbsenceReason', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'AbsenceReason', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'AbsenceReason', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'AbsenceReason', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'AbsenceReason', N'SortOrd'         , N'Номер на подредба.'
