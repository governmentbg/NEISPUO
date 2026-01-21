PRINT 'Create FinalGradePointAverageByStudentsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByStudentsReportItem'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByStudentsReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [FinalGradePointAverageByStudentsReportId]              INT              NOT NULL,
    [FinalGradePointAverageByStudentsReportItemId]          INT              NOT NULL,

    [ClassBookName]                                         NVARCHAR(560)    NOT NULL,
    [StudentNames]                                          NVARCHAR(550)    NOT NULL,
    [IsTransferred]                                         BIT              NOT NULL,
    [FinalGradePointAverage]                                DECIMAL(3,2)     NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByStudentsReportItem] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId], [FinalGradePointAverageByStudentsReportItemId]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReportItem_FinalGradePointAverageByStudentsReport] FOREIGN KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId]) REFERENCES [school_books].[FinalGradePointAverageByStudentsReport] ([SchoolYear], [FinalGradePointAverageByStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO


exec school_books.spDescTable  N'FinalGradePointAverageByStudentsReportItem', N'Справка среден успех от срочни/годишни оценки по ученици - елемент.'

exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'SchoolYear'                                       , N'Учебна година.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'FinalGradePointAverageByStudentsReportId'         , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'FinalGradePointAverageByStudentsReportItemId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'ClassBookName'                                    , N'Име на дневника.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'StudentNames'                                     , N'Имена на ученика.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'IsTransferred'                                    , N'Ученика е отписан - да/не.'
exec school_books.spDescColumn N'FinalGradePointAverageByStudentsReportItem', N'FinalGradePointAverage'                           , N'Среден успех на ученика.'
