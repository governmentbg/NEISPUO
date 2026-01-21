EXEC [school_books].[spCreateIdSequence] N'DateAbsencesReport'
GO

CREATE TABLE [school_books].[DateAbsencesReport] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [DateAbsencesReportId]              INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [ReportDate]                        DATETIME2        NOT NULL,
    [IsUnited]                          BIT              NOT NULL,
    [ClassBookNames]                    NVARCHAR(MAX)    NULL,
    [ShiftNames]                        NVARCHAR(MAX)    NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_DateAbsencesReport] PRIMARY KEY ([SchoolYear], [DateAbsencesReportId]),

    -- external references
    CONSTRAINT [FK_DateAbsencesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_DateAbsencesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_DateAbsencesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_DateAbsencesReport] UNIQUE ([SchoolYear], [InstId], [DateAbsencesReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'DateAbsencesReportItem'
GO

CREATE TABLE [school_books].[DateAbsencesReportItem] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [DateAbsencesReportId]                   INT              NOT NULL,
    [DateAbsencesReportItemId]               INT              NOT NULL,

    [ClassBookId]                            INT              NOT NULL, -- no FK, we dont need a hard reference
    [ClassBookName]                          NVARCHAR(560)    NOT NULL,
    [ShiftId]                                INT              NULL,     -- no FK, we dont need a hard reference
    [ShiftName]                              NVARCHAR(100)    NULL,
    [HourNumber]                             INT              NOT NULL,
    [AbsenceStudentNumbers]                  NVARCHAR(100)    NULL,
    [AbsenceStudentCount]                    INT              NOT NULL,
    [IsOffDay]                               BIT              NOT NULL,
    [HasScheduleDate]                        BIT              NOT NULL,

    CONSTRAINT [PK_DateAbsencesReportItem] PRIMARY KEY ([SchoolYear], [DateAbsencesReportId], [DateAbsencesReportItemId]),
    CONSTRAINT [FK_DateAbsencesReportItem_DateAbsencesReport] FOREIGN KEY ([SchoolYear], [DateAbsencesReportId]) REFERENCES [school_books].[DateAbsencesReport] ([SchoolYear], [DateAbsencesReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
