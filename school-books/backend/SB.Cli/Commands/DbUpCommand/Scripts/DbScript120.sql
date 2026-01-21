EXEC [school_books].[spCreateIdSequence] N'GradelessStudentsReport'
GO

CREATE TABLE [school_books].[GradelessStudentsReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradelessStudentsReportId]         INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [OnlyFinalGrades]                   BIT              NOT NULL,
    [Term]                              INT              NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_GradelessStudentsReport] PRIMARY KEY ([SchoolYear], [GradelessStudentsReportId]),

    CONSTRAINT [CHK_GradelessStudentsReport_Term] CHECK ([Term] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_GradelessStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_GradelessStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_GradelessStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_GradelessStudentsReport] UNIQUE ([SchoolYear], [InstId], [GradelessStudentsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'GradelessStudentsReportItem'
GO

CREATE TABLE [school_books].[GradelessStudentsReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [GradelessStudentsReportId]              INT              NOT NULL,
    [GradelessStudentsReportItemId]          INT              NOT NULL,

    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [StudentName]                            NVARCHAR(550)    NOT NULL,
    [CurriculumName]                         NVARCHAR(550)    NOT NULL,

    CONSTRAINT [PK_GradelessStudentsReportItem] PRIMARY KEY ([SchoolYear], [GradelessStudentsReportId], [GradelessStudentsReportItemId]),
    CONSTRAINT [FK_GradelessStudentsReportItem_GradelessStudentsReport] FOREIGN KEY ([SchoolYear], [GradelessStudentsReportId]) REFERENCES [school_books].[GradelessStudentsReport] ([SchoolYear], [GradelessStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
