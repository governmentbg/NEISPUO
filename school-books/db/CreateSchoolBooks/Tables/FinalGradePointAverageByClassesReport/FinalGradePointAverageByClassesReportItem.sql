PRINT 'Create FinalGradePointAverageByClassesReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByClassesReportItem'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByClassesReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [FinalGradePointAverageByClassesReportId]               INT              NOT NULL,
    [FinalGradePointAverageByClassesReportItemId]           INT              NOT NULL,

    [ClassBookName]                                         NVARCHAR(1000)   NOT NULL,
    [CurriculumInfo]                                        NVARCHAR(1000)   NOT NULL,
    [StudentsCount]                                         INT              NOT NULL,
    [StudentsWithGradesCount]                               INT              NOT NULL,
    [StudentsWithGradesPercentage]                          DECIMAL(5,2)     NOT NULL,
    [GradePointAverage]                                     DECIMAL(3,2)     NOT NULL,
    [TotalGradesCount]                                      INT              NOT NULL,
    [PoorGradesCount]                                       INT              NOT NULL,
    [FairGradesCount]                                       INT              NOT NULL,
    [GoodGradesCount]                                       INT              NOT NULL,
    [VeryGoodGradesCount]                                   INT              NOT NULL,
    [ExcellentGradesCount]                                  INT              NOT NULL,
    [IsTotal]                                               BIT              NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByClassesReportItem] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByClassesReportId], [FinalGradePointAverageByClassesReportItemId]),
    CONSTRAINT [FK_FinalGradePointAverageByClassesReportItem_FinalGradePointAverageByClassesReport] FOREIGN KEY ([SchoolYear], [FinalGradePointAverageByClassesReportId]) REFERENCES [school_books].[FinalGradePointAverageByClassesReport] ([SchoolYear], [FinalGradePointAverageByClassesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO


exec school_books.spDescTable  N'FinalGradePointAverageByClassesReportItem', N'Справка среден успех от срочни/годишни оценки по класове - елемент.'

exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'SchoolYear'                                       , N'Учебна година.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'FinalGradePointAverageByClassesReportId'          , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'FinalGradePointAverageByClassesReportItemId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'ClassBookName'                                    , N'Име на дневника.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'CurriculumInfo'                                   , N'Име на учебният предмет и преподаватели към него.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'StudentsCount'                                    , N'Брой на учениците.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'StudentsWithGradesCount'                          , N'Брой на учениците с оценка.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'StudentsWithGradesPercentage'                     , N'Процент на изпитаните ученици.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'GradePointAverage'                                , N'Среден успех на оценките по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'TotalGradesCount'                                 , N'Брой всички оценки.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'PoorGradesCount'                                  , N'Брой оценки слаб(2) по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'FairGradesCount'                                  , N'Брой оценки среден(3) по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'GoodGradesCount'                                  , N'Брой оценки добър(4) по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'VeryGoodGradesCount'                              , N'Брой оценки мн. добър(5) по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'ExcellentGradesCount'                             , N'Брой оценки отличен(6) по учебният предмет.'
exec school_books.spDescColumn N'FinalGradePointAverageByClassesReportItem', N'IsTotal'                                          , N'Идентификатор показващ дали записът е обобщаващ.'
