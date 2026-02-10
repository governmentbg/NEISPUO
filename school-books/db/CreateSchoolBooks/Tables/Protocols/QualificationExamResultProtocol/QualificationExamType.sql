PRINT 'Create QualificationExamType table'
GO

CREATE TABLE [school_books].[QualificationExamType] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_QualificationExamType] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'QualificationExamType', N'Вид изпит за професионална квалификация.'

exec school_books.spDescColumn N'QualificationExamType', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'QualificationExamType', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'QualificationExamType', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'QualificationExamType', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'QualificationExamType', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'QualificationExamType', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'QualificationExamType', N'SortOrd'         , N'Номер на подредба.'
