PRINT 'Create SessionStudentsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'SessionStudentsReportItem'
GO

CREATE TABLE [school_books].[SessionStudentsReportItem] (
    [SchoolYear]                                SMALLINT            NOT NULL,
    [SessionStudentsReportId]                   INT                 NOT NULL,
    [SessionStudentsReportItemId]               INT                 NOT NULL,

    [StudentNames]                              NVARCHAR(100)       NOT NULL,
    [IsTransferred]                             BIT                 NOT NULL,
    [ClassBookName]                             NVARCHAR(100)       NOT NULL,
    [Session1CurriculumNames]                   NVARCHAR(1000)      NULL,
    [Session2CurriculumNames]                   NVARCHAR(1000)      NULL,
    [Session3CurriculumNames]                   NVARCHAR(1000)      NULL,

    CONSTRAINT [PK_SessionStudentsReportItem] PRIMARY KEY ([SchoolYear], [SessionStudentsReportId], [SessionStudentsReportItemId]),
    CONSTRAINT [FK_SessionStudentsReportItem_SessionStudentsReport] FOREIGN KEY ([SchoolYear], [SessionStudentsReportId]) REFERENCES [school_books].[SessionStudentsReport] ([SchoolYear], [SessionStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'SessionStudentsReportItem', N'Справка ученици за поправителни изпити - елемент.'

exec school_books.spDescColumn N'SessionStudentsReportItem', N'SchoolYear'                                , N'Учебна година.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'SessionStudentsReportId'                   , N'Идентификатор на справка ученици за поправителни изпити.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'SessionStudentsReportItemId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SessionStudentsReportItem', N'StudentNames'                              , N'Три имена на ученика.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'IsTransferred'                             , N'Ученика е отписан - да/не.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'ClassBookName'                             , N'Име на класа на ученика.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'Session1CurriculumNames'                   , N'Имена на предметите за поправка от 1ва сесия.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'Session2CurriculumNames'                   , N'Имена на предметите за поправка от 2ра сесия.'
exec school_books.spDescColumn N'SessionStudentsReportItem', N'Session3CurriculumNames'                   , N'Имена на предметите за поправка от допълнителна сесия.'
