EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByStudentsReport'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByStudentsReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [RegularGradePointAverageByStudentsReportId]        INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_RegularGradePointAverageByStudentsReport] PRIMARY KEY ([SchoolYear], [RegularGradePointAverageByStudentsReportId]),

    CONSTRAINT [CHK_RegularGradePointAverageByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_RegularGradePointAverageByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_RegularGradePointAverageByStudentsReport] UNIQUE ([SchoolYear], [InstId], [RegularGradePointAverageByStudentsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'RegularGradePointAverageByStudentsReportItem'
GO

CREATE TABLE [school_books].[RegularGradePointAverageByStudentsReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [RegularGradePointAverageByStudentsReportId]            INT              NOT NULL,
    [RegularGradePointAverageByStudentsReportItemId]        INT              NOT NULL,

    [ClassBookName]                                         NVARCHAR(1000)   NOT NULL,
    [StudentNames]                                          NVARCHAR(1000)   NOT NULL,
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
