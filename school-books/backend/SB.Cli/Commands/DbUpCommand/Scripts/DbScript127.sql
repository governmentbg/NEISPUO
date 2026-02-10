EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByClassesReport'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByClassesReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [RegularGradePointAverageByClassesReportId]         INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_RegularGradePointAverageByClassesReport] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByClassesReportId]),

    CONSTRAINT [CHK_RegularGradePointAverageByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_RegularGradePointAverageByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_RegularGradePointAverageByClassesReport] UNIQUE ([SchoolYear], [InstId], [RegularGradePointAverageByClassesReportId]),
);
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
