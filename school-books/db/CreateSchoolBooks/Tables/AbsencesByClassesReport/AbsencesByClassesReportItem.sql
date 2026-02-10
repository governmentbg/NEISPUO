PRINT 'Create AbsencesByClassesReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByClassesReportItem'
GO

CREATE TABLE [school_books].[AbsencesByClassesReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [AbsencesByClassesReportId]              INT              NOT NULL,
    [AbsencesByClassesReportItemId]          INT              NOT NULL,

    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [StudentsCount]                          INT              NOT NULL,
    [ExcusedAbsencesCount]                   INT              NOT NULL,
    [ExcusedAbsencesCountAverage]            DECIMAL(5,2)     NOT NULL,
    [UnexcusedAbsencesCount]                 DECIMAL(7,1)     NOT NULL,
    [UnexcusedAbsencesCountAverage]          DECIMAL(5,2)     NOT NULL,
    [IsTotal]                                BIT              NOT NULL,

    CONSTRAINT [PK_AbsencesByClassesReportItem] PRIMARY KEY ([SchoolYear], [AbsencesByClassesReportId], [AbsencesByClassesReportItemId]),
    CONSTRAINT [FK_AbsencesByClassesReportItem_AbsencesByClassesReport] FOREIGN KEY ([SchoolYear], [AbsencesByClassesReportId]) REFERENCES [school_books].[AbsencesByClassesReport] ([SchoolYear], [AbsencesByClassesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO


exec school_books.spDescTable  N'AbsencesByClassesReportItem', N'Справка отсъствия по ученици - елемент.'

exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'SchoolYear'                        , N'Учебна година.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'AbsencesByClassesReportId'         , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'AbsencesByClassesReportItemId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'ClassBookName'                     , N'Име на дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'StudentsCount'                     , N'Брой на учениците към дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'ExcusedAbsencesCount'              , N'Брой всички уважителни отсъствия към дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'ExcusedAbsencesCountAverage'       , N'Среден брой уважителни отсъствия към дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'UnexcusedAbsencesCount'            , N'Брой всички неуважителни отсъствия към дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'UnexcusedAbsencesCountAverage'     , N'Среден брой неуважителни отсъствия към дневника.'
exec school_books.spDescColumn N'AbsencesByClassesReportItem', N'IsTotal'                           , N'Идентификатор показващ дали записът е обобщаващ.'
