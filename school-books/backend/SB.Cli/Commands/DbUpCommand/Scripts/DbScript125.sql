EXEC [school_books].[spCreateIdSequence] N'ExamsReport'
GO

CREATE TABLE [school_books].[ExamsReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [ExamsReportId]                         INT              NOT NULL,

    [InstId]                                INT              NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ExamsReport] PRIMARY KEY ([SchoolYear], [ExamsReportId]),

    -- external references
    CONSTRAINT [FK_ExamsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ExamsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ExamsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ExamsReport] UNIQUE ([SchoolYear], [InstId], [ExamsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'ExamsReportItem'
GO

CREATE TABLE [school_books].[ExamsReportItem] (
    [SchoolYear]                                SMALLINT            NOT NULL,
    [ExamsReportId]                             INT                 NOT NULL,
    [ExamsReportItemId]                         INT                 NOT NULL,

    [Date]                                      DATE                NOT NULL,
    [ClassBookName]                             NVARCHAR(100)       NOT NULL,
    [BookExamType]                              INT                 NOT NULL,
    [CurriculumName]                            NVARCHAR(550)       NOT NULL,
    [CreatedBySysUserName]                      NVARCHAR(550)       NOT NULL,

    CONSTRAINT [PK_ExamsReportItem] PRIMARY KEY ([SchoolYear], [ExamsReportId], [ExamsReportItemId]),
    CONSTRAINT [FK_ExamsReportItem_ExamsReport] FOREIGN KEY ([SchoolYear], [ExamsReportId]) REFERENCES [school_books].[ExamsReport] ([SchoolYear], [ExamsReportId]),

    CONSTRAINT [CHK_ExamsReportItem_BookExamType] CHECK ([BookExamType] IN (1, 2)),
)
WITH (DATA_COMPRESSION = PAGE);
GO
