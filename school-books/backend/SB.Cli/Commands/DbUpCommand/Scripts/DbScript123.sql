EXEC [school_books].[spCreateIdSequence] N'AbsencesByClassesReport'
GO

CREATE TABLE [school_books].[AbsencesByClassesReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [AbsencesByClassesReportId]         INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [Period]                            INT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_AbsencesByClassesReport] PRIMARY KEY ([SchoolYear], [AbsencesByClassesReportId]),

    CONSTRAINT [CHK_AbsencesByClassesReport_Period] CHECK ([Period] IN (1, 2, 3)),

    -- external references
    CONSTRAINT [FK_AbsencesByClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_AbsencesByClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_AbsencesByClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_AbsencesByClassesReport] UNIQUE ([SchoolYear], [InstId], [AbsencesByClassesReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'AbsencesByClassesReportItem'
GO

CREATE TABLE [school_books].[AbsencesByClassesReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [AbsencesByClassesReportId]              INT              NOT NULL,
    [AbsencesByClassesReportItemId]          INT              NOT NULL,

    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [StudentsCount]                          INT              NOT NULL,
    [ExcusedAbsencesCount]                   INT              NOT NULL,
    [ExcusedAbsencesCountAverage]            DECIMAL(5,2)     NOT NULL,
    [UnexcusedAbsencesCount]                 DECIMAL(7,1)     NOT NULL,
    [UnexcusedAbsencesCountAverage]          DECIMAL(5,2)     NOT NULL,
    [IsTotal]                                BIT              NOT NULL,

    CONSTRAINT [PK_AbsencesByClassesReportItem] PRIMARY KEY ([SchoolYear], [AbsencesByClassesReportId], [AbsencesByClassesReportItemId]),
    CONSTRAINT [FK_AbsencesByClassesReportItem_AbsencesByClassesReport] FOREIGN KEY ([SchoolYear], [AbsencesByClassesReportId]) REFERENCES [school_books].[AbsencesByClassesReport] ([SchoolYear], [AbsencesByClassesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
