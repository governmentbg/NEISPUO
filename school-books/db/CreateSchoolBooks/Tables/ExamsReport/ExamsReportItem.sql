PRINT 'Create ExamsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'ExamsReportItem'
GO

CREATE TABLE [school_books].[ExamsReportItem] (
    [SchoolYear]                                SMALLINT            NOT NULL,
    [ExamsReportId]                             INT                 NOT NULL,
    [ExamsReportItemId]                         INT                 NOT NULL,

    [Date]                                      DATE                NOT NULL,
    [ClassBookName]                             NVARCHAR(100)       NOT NULL,
    [BookExamType]                              INT                 NOT NULL,
    [CurriculumName]                            NVARCHAR(550)       NOT NULL,
    [CreatedBySysUserName]                      NVARCHAR(550)       NOT NULL,

    CONSTRAINT [PK_ExamsReportItem] PRIMARY KEY ([SchoolYear], [ExamsReportId], [ExamsReportItemId]),
    CONSTRAINT [FK_ExamsReportItem_ExamsReport] FOREIGN KEY ([SchoolYear], [ExamsReportId]) REFERENCES [school_books].[ExamsReport] ([SchoolYear], [ExamsReportId]),

    CONSTRAINT [CHK_ExamsReportItem_BookExamType] CHECK ([BookExamType] IN (1, 2)),
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'ExamsReportItem', N'Справка контролни/класни - елемент.'

exec school_books.spDescColumn N'ExamsReportItem', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'ExamsReportItem', N'ExamsReportId'                     , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'ExamsReportItem', N'ExamsReportItemId'                 , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ExamsReportItem', N'Date'                              , N'Дата на класната/контролната работа.'
exec school_books.spDescColumn N'ExamsReportItem', N'ClassBookName'                     , N'Име на дневника.'
exec school_books.spDescColumn N'ExamsReportItem', N'BookExamType'                      , N'Вид - класна/контролна работа.'
exec school_books.spDescColumn N'ExamsReportItem', N'CurriculumName'                    , N'Име на предмета.'
exec school_books.spDescColumn N'ExamsReportItem', N'CreatedBySysUserName'              , N'Въвдена от.'
