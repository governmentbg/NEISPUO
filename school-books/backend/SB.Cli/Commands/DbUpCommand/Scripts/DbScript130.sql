EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByStudentsReport'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByStudentsReport] (
    [SchoolYear]                                        SMALLINT         NOT NULL,
    [FinalGradePointAverageByStudentsReportId]          INT              NOT NULL,

    [InstId]                                            INT              NOT NULL,
    [Period]                                            INT              NOT NULL,
    [ClassBookNames]                                    NVARCHAR(MAX)    NOT NULL,
    [MinimumGradePointAverage]                          DECIMAL(3,2)     NULL,

    [CreateDate]                                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                                INT              NOT NULL,
    [ModifyDate]                                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                               INT              NOT NULL,
    [Version]                                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByStudentsReport] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId]),

    CONSTRAINT [CHK_FinalGradePointAverageByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_FinalGradePointAverageByStudentsReport] UNIQUE ([SchoolYear], [InstId], [FinalGradePointAverageByStudentsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'FinalGradePointAverageByStudentsReportItem'
GO

CREATE TABLE [school_books].[FinalGradePointAverageByStudentsReportItem] (
    [SchoolYear]                                            SMALLINT         NOT NULL,
    [FinalGradePointAverageByStudentsReportId]              INT              NOT NULL,
    [FinalGradePointAverageByStudentsReportItemId]          INT              NOT NULL,

    [ClassBookName]                                         NVARCHAR(560)    NOT NULL,
    [StudentNames]                                          NVARCHAR(550)    NOT NULL,
    [FinalGradePointAverage]                                DECIMAL(3,2)     NOT NULL,

    CONSTRAINT [PK_FinalGradePointAverageByStudentsReportItem] PRIMARY KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId], [FinalGradePointAverageByStudentsReportItemId]),
    CONSTRAINT [FK_FinalGradePointAverageByStudentsReportItem_FinalGradePointAverageByStudentsReport] FOREIGN KEY ([SchoolYear], [FinalGradePointAverageByStudentsReportId]) REFERENCES [school_books].[FinalGradePointAverageByStudentsReport] ([SchoolYear], [FinalGradePointAverageByStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
