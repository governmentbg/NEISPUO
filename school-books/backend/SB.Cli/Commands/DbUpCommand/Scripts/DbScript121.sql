EXEC [school_books].[spCreateIdSequence] N'SessionStudentsReport'
GO

CREATE TABLE [school_books].[SessionStudentsReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [SessionStudentsReportId]               INT              NOT NULL,

    [InstId]                                INT              NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SessionStudentsReport] PRIMARY KEY ([SchoolYear], [SessionStudentsReportId]),

    -- external references
    CONSTRAINT [FK_SessionStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SessionStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SessionStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SessionStudentsReport] UNIQUE ([SchoolYear], [InstId], [SessionStudentsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'SessionStudentsReportItem'
GO

CREATE TABLE [school_books].[SessionStudentsReportItem] (
    [SchoolYear]                                SMALLINT            NOT NULL,
    [SessionStudentsReportId]                   INT                 NOT NULL,
    [SessionStudentsReportItemId]               INT                 NOT NULL,

    [StudentNames]                              NVARCHAR(100)       NOT NULL,
    [ClassBookName]                             NVARCHAR(100)       NOT NULL,
    [Session1CurriculumNames]                   NVARCHAR(1000)      NULL,
    [Session2CurriculumNames]                   NVARCHAR(1000)      NULL,
    [Session3CurriculumNames]                   NVARCHAR(1000)      NULL,

    CONSTRAINT [PK_SessionStudentsReportItem] PRIMARY KEY ([SchoolYear], [SessionStudentsReportId], [SessionStudentsReportItemId]),
    CONSTRAINT [FK_SessionStudentsReportItem_SessionStudentsReport] FOREIGN KEY ([SchoolYear], [SessionStudentsReportId]) REFERENCES [school_books].[SessionStudentsReport] ([SchoolYear], [SessionStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
