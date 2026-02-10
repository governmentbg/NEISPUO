PRINT 'Create MissingTopicsReportItemTeacher table'
GO

EXEC [school_books].[spCreateIdSequence] N'MissingTopicsReportItemTeacher'
GO

CREATE TABLE [school_books].[MissingTopicsReportItemTeacher] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [MissingTopicsReportId]             INT              NOT NULL,
    [MissingTopicsReportItemId]         INT              NOT NULL,
    [MissingTopicsReportItemTeacherId]  INT              NOT NULL,
    [PersonName]                        NVARCHAR(550)    NOT NULL,

    CONSTRAINT [PK_MissingTopicsReportItemTeacher] PRIMARY KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId], [MissingTopicsReportItemTeacherId]),
    CONSTRAINT [FK_MissingTopicsReportItemTeacher_MissingTopicsReportItem]
        FOREIGN KEY ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId])
        REFERENCES [school_books].[MissingTopicsReportItem] ([SchoolYear], [MissingTopicsReportId], [MissingTopicsReportItemId]),
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'MissingTopicsReportItemTeacher', N'Елемент от справка невписани теми - учител.'

exec school_books.spDescColumn N'MissingTopicsReportItemTeacher', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'MissingTopicsReportItemTeacher', N'MissingTopicsReportId'              , N'Идентификатор на справка невписани теми.'
exec school_books.spDescColumn N'MissingTopicsReportItemTeacher', N'MissingTopicsReportItemId'          , N'Идентификатор на елемент от справка невписани теми.'
exec school_books.spDescColumn N'MissingTopicsReportItemTeacher', N'MissingTopicsReportItemTeacherId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'MissingTopicsReportItemTeacher', N'PersonName'                         , N'Име на учител.'
