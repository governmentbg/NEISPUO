PRINT 'Create GradelessStudentsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'GradelessStudentsReportItem'
GO

CREATE TABLE [school_books].[GradelessStudentsReportItem] (
    [SchoolYear]                       SMALLINT         NOT NULL,
    [GradelessStudentsReportId]        INT              NOT NULL,
    [GradelessStudentsReportItemId]    INT              NOT NULL,

    [ClassBookName]                NVARCHAR(560)    NOT NULL,
    [StudentName]                  NVARCHAR(550)    NOT NULL,
    [CurriculumName]               NVARCHAR(550)    NOT NULL,

    CONSTRAINT [PK_GradelessStudentsReportItem] PRIMARY KEY ([SchoolYear], [GradelessStudentsReportId], [GradelessStudentsReportItemId]),
    CONSTRAINT [FK_GradelessStudentsReportItem_GradelessStudentsReport] FOREIGN KEY ([SchoolYear], [GradelessStudentsReportId]) REFERENCES [school_books].[GradelessStudentsReport] ([SchoolYear], [GradelessStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'GradelessStudentsReportItem', N'Справка ученици без оценки - елемент.'

exec school_books.spDescColumn N'GradelessStudentsReportItem', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'GradelessStudentsReportItem', N'GradelessStudentsReportId'         , N'Идентификатор на справка ученици без оценки.'
exec school_books.spDescColumn N'GradelessStudentsReportItem', N'GradelessStudentsReportItemId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'GradelessStudentsReportItem', N'ClassBookName'                     , N'Име на дневника.'
exec school_books.spDescColumn N'GradelessStudentsReportItem', N'StudentName'                       , N'Име на ученика.'
exec school_books.spDescColumn N'GradelessStudentsReportItem', N'CurriculumName'                    , N'Име на предмета.'
