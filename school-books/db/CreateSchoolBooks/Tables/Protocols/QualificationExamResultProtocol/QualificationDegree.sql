PRINT 'Create QualificationDegree table'
GO

CREATE TABLE [school_books].[QualificationDegree] (
    [Id]                    INT              NOT NULL IDENTITY,

    [Name]                  NVARCHAR(255)    NOT NULL,
    [Description]           NVARCHAR(2048)   NULL,
    [IsValid]               BIT              NOT NULL,
    [ValidFrom]             DATETIME2        NULL,
    [ValidTo]               DATETIME2        NULL,
    [SortOrd]               INT              NULL,

    CONSTRAINT [PK_QualificationDegree] PRIMARY KEY ([Id])
);
GO

exec school_books.spDescTable  N'QualificationDegree', N'Степен на професионална квалификация.'

exec school_books.spDescColumn N'QualificationDegree', N'Id'              , N'Уникален идентификатор.'

exec school_books.spDescColumn N'QualificationDegree', N'Name'            , N'Наименование.'
exec school_books.spDescColumn N'QualificationDegree', N'Description'     , N'Описание.'
exec school_books.spDescColumn N'QualificationDegree', N'IsValid'         , N'Валидност – да/не.'
exec school_books.spDescColumn N'QualificationDegree', N'ValidFrom'       , N'Валидност от.'
exec school_books.spDescColumn N'QualificationDegree', N'ValidTo'         , N'Валидност до.'
exec school_books.spDescColumn N'QualificationDegree', N'SortOrd'         , N'Номер на подредба.'
