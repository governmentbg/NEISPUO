PRINT 'Create RegularGradePointAverageByStudentsReportItem table'
GO

EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByStudentsReportItem'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByStudentsReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [RegularGradePointAverageByStudentsReportId]            INT              NOT NULL,
    [RegularGradePointAverageByStudentsReportItemId]        INT              NOT NULL,

    [ClassBookName]                                         NVARCHAR(1000)   NOT NULL,
    [StudentNames]                                          NVARCHAR(1000)   NOT NULL,
    [IsTransferred]                                         BIT              NOT NULL,
    [CurriculumInfo]                                        NVARCHAR(1000)   NOT NULL,
    [GradePointAverage]                                     DECIMAL(3,2)     NOT NULL,
    [TotalGradesCount]                                      INT              NOT NULL,
    [PoorGradesCount]                                       INT              NOT NULL,
    [FairGradesCount]                                       INT              NOT NULL,
    [GoodGradesCount]                                       INT              NOT NULL,
    [VeryGoodGradesCount]                                   INT              NOT NULL,
    [ExcellentGradesCount]                                  INT              NOT NULL,
    [IsTotal]                                               BIT              NOT NULL,

    CONSTRAINT [PK_RegularGradePointAverageByStudentsReportItem] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByStudentsReportId], [RegularGradePointAverageByStudentsReportItemId]),
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReportItem_RegularGradePointAverageByStudentsReport] FOREIGN KEY ([SchoolYear], [RegularGradePointAverageByStudentsReportId]) REFERENCES [school_books].[RegularGradePointAverageByStudentsReport] ([SchoolYear], [RegularGradePointAverageByStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO


exec school_books.spDescTable  N'RegularGradePointAverageByStudentsReportItem', N'Справка среден успех от текущи оценки по ученици - елемент.'

exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'SchoolYear'                                       , N'Учебна година.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'RegularGradePointAverageByStudentsReportId'       , N'Идентификатор на справка отсъствия по ученици.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'RegularGradePointAverageByStudentsReportItemId'   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'ClassBookName'                                    , N'Име на дневника.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'StudentNames'                                     , N'Имена на ученика.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'IsTransferred'                                    , N'Ученика е отписан - да/не.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'CurriculumInfo'                                   , N'Име на учебният предмет и преподаватели към него.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'GradePointAverage'                                , N'Среден успех на оценките по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'TotalGradesCount'                                 , N'Брой всички оценки.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'PoorGradesCount'                                  , N'Брой оценки слаб(2) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'FairGradesCount'                                  , N'Брой оценки среден(3) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'GoodGradesCount'                                  , N'Брой оценки добър(4) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'VeryGoodGradesCount'                              , N'Брой оценки мн. добър(5) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'ExcellentGradesCount'                             , N'Брой оценки отличен(6) по учебният предмет.'
exec school_books.spDescColumn N'RegularGradePointAverageByStudentsReportItem', N'IsTotal'                                          , N'Идентификатор показващ дали записът е обобщаващ.'
