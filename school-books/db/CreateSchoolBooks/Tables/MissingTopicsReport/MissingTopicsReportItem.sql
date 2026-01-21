PRINT 'Create MissingTopicsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'MissingTopicsReportItem'
GO

CREATE TABLE [school_books].[MissingTopicsReportItem] (
    [SchoolYear]                   SMALLINT         NOT NULL,
    [MissingTopicsReportId]        INT              NOT NULL,
    [MissingTopicsReportItemId]    INT              NOT NULL,

    [Date]                         DATE             NOT NULL,
    [ClassBookName]                NVARCHAR(560)    NOT NULL,
    [CurriculumName]               NVARCHAR(550)    NOT NULL,

    CONSTRAINT [PK_MissingTopicsReportItem] PRIMARY KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId]),
    CONSTRAINT [FK_MissingTopicsReportItem_MissingTopicsReport] FOREIGN KEY ([SchoolYear], [MissingTopicsReportId]) REFERENCES [school_books].[MissingTopicsReport] ([SchoolYear], [MissingTopicsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'MissingTopicsReportItem', N'Справка невписани теми - елемент.'

exec school_books.spDescColumn N'MissingTopicsReportItem', N'SchoolYear'                    , N'Учебна година.'
exec school_books.spDescColumn N'MissingTopicsReportItem', N'MissingTopicsReportId'         , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'MissingTopicsReportItem', N'MissingTopicsReportItemId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'MissingTopicsReportItem', N'Date'                          , N'Дата.'
exec school_books.spDescColumn N'MissingTopicsReportItem', N'ClassBookName'                 , N'Име на дневника.'
exec school_books.spDescColumn N'MissingTopicsReportItem', N'CurriculumName'                , N'Име на предмета.'
