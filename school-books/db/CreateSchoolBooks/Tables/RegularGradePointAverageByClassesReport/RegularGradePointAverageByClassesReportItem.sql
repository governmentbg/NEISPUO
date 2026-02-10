PRINT 'Create RegularGradePointAverageByClassesReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByClassesReportItem'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByClassesReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [RegularGradePointAverageByClassesReportId]             INT              NOT NULL,
    [RegularGradePointAverageByClassesReportItemId]         INT              NOT NULL,

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

    CONSTRAINT [PK_RegularGradePointAverageByClassesReportItem] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByClassesReportId], [RegularGradePointAverageByClassesReportItemId]),
    CONSTRAINT [FK_RegularGradePointAverageByClassesReportItem_RegularGradePointAverageByClassesReport] FOREIGN KEY ([SchoolYear], [RegularGradePointAverageByClassesReportId]) REFERENCES [school_books].[RegularGradePointAverageByClassesReport] ([SchoolYear], [RegularGradePointAverageByClassesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO


exec school_books.spDescTable  N'RegularGradePointAverageByClassesReportItem', N'Справка среден успех от текущи оценки по класове - елемент.'

exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'SchoolYear'                                       , N'Учебна година.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'RegularGradePointAverageByClassesReportId'        , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'RegularGradePointAverageByClassesReportItemId'    , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'ClassBookName'                                    , N'Име на дневника.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'CurriculumInfo'                                   , N'Име на учебният предмет и преподаватели към него.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'StudentsCount'                                    , N'Брой на учениците.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'StudentsWithGradesCount'                          , N'Брой на учениците с оценка.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'StudentsWithGradesPercentage'                     , N'Процент на изпитаните ученици.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'GradePointAverage'                                , N'Среден успех на оценките по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'TotalGradesCount'                                 , N'Брой всички оценки.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'PoorGradesCount'                                  , N'Брой оценки слаб(2) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'FairGradesCount'                                  , N'Брой оценки среден(3) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'GoodGradesCount'                                  , N'Брой оценки добър(4) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'VeryGoodGradesCount'                              , N'Брой оценки мн. добър(5) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'ExcellentGradesCount'                             , N'Брой оценки отличен(6) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByClassesReportItem', N'IsTotal'                                          , N'Идентификатор показващ дали записът е обобщаващ.'
