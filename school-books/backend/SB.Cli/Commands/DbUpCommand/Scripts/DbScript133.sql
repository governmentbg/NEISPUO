
EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByClassesReport'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByClassesReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [FinalGradePointAverageByClassesReportId]           INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByClassesReport] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByClassesReportId]),

    CONSTRAINT [CHK_FinalGradePointAverageByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_FinalGradePointAverageByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_FinalGradePointAverageByClassesReport] UNIQUE ([SchoolYear], [InstId], [FinalGradePointAverageByClassesReportId]),
);
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
