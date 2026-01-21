EXEC [school_books].[spCreateIdSequence] N'AbsencesByStudentsReport'
GO

CREATE TABLE [school_books].[AbsencesByStudentsReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [AbsencesByStudentsReportId]        INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [Period]                            INT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_AbsencesByStudentsReport] PRIMARY KEY ([SchoolYear], [AbsencesByStudentsReportId]),

    CONSTRAINT [CHK_AbsencesByStudentsReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_AbsencesByStudentsReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_AbsencesByStudentsReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_AbsencesByStudentsReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_AbsencesByStudentsReport] UNIQUE ([SchoolYear], [InstId], [AbsencesByStudentsReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByStudentsReportItem'
GO

CREATE TABLE [school_books].[AbsencesByStudentsReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [AbsencesByStudentsReportId]             INT              NOT NULL,
    [AbsencesByStudentsReportItemId]         INT              NOT NULL,

    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [StudentName]                            NVARCHAR(550)    NOT NULL,
    [ExcusedAbsencesCount]                   INT              NOT NULL,
    [UnexcusedAbsencesCount]                 INT              NOT NULL,
    [LateAbsencesCount]                      INT              NOT NULL,
    [IsTotal]                                BIT              NOT NULL,

    CONSTRAINT [PK_AbsencesByStudentsReportItem] PRIMARY KEY ([SchoolYear], [AbsencesByStudentsReportId], [AbsencesByStudentsReportItemId]),
    CONSTRAINT [FK_AbsencesByStudentsReportItem_AbsencesByStudentsReport] FOREIGN KEY ([SchoolYear], [AbsencesByStudentsReportId]) REFERENCES [school_books].[AbsencesByStudentsReport] ([SchoolYear], [AbsencesByStudentsReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
