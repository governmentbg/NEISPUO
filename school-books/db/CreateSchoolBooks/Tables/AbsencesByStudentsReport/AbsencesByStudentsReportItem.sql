PRINT 'Create AbsencesByStudentsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByStudentsReportItem'
GO

CREATE TABLE [school_books].[AbsencesByStudentsReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [AbsencesByStudentsReportId]             INT              NOT NULL,
    [AbsencesByStudentsReportItemId]         INT              NOT NULL,

    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [StudentName]                            NVARCHAR(550)    NOT NULL,
    [IsTransferred]                          BIT              NOT NULL,
    [ExcusedAbsencesCount]                   INT              NOT NULL,
    [UnexcusedAbsencesCount]                 INT              NOT NULL,
    [LateAbsencesCount]                      INT              NOT NULL,
    [IsTotal]                                BIT              NOT NULL,

    CONSTRAINT [PK_AbsencesByStudentsReportItem] PRIMARY KEY ([SchoolYear], [AbsencesByStudentsReportId], [AbsencesByStudentsReportItemId]),
    CONSTRAINT [FK_AbsencesByStudentsReportItem_AbsencesByStudentsReport] FOREIGN KEY ([SchoolYear], [AbsencesByStudentsReportId]) REFERENCES [school_books].[AbsencesByStudentsReport] ([SchoolYear], [AbsencesByStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO

exec school_books.spDescTable  N'AbsencesByStudentsReportItem', N'Справка отсъствия по ученици - елемент.'

exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'AbsencesByStudentsReportId'        , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'AbsencesByStudentsReportItemId'    , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'ClassBookName'                     , N'Име на дневника.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'StudentName'                       , N'Име на ученика.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'IsTransferred'                     , N'Ученика е отписан - да/не.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'ExcusedAbsencesCount'              , N'Брой уважителни отсъствия.'
exec school_books.spDescColumn N'AbsencesByStudentsReportItem', N'UnexcusedAbsencesCount'            , N'Брой неуважителни отсъствия.'
