EXEC [school_books].[spCreateIdSequence] N'StudentsAtRiskOfDroppingOutReport'
GO

CREATE TABLE [school_books].[StudentsAtRiskOfDroppingOutReport] (
    [SchoolYear]                            SMALLINT         NOT NULL,
    [StudentsAtRiskOfDroppingOutReportId]   INT              NOT NULL,

    [InstId]                                INT              NOT NULL,
    [ReportDate]                            DATETIME2        NOT NULL,

    [CreateDate]                            DATETIME2        NOT NULL,
    [CreatedBySysUserId]                    INT              NOT NULL,
    [ModifyDate]                            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                   INT              NOT NULL,
    [Version]                               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_StudentsAtRiskOfDroppingOutReport] PRIMARY KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId]),

    -- external references
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_StudentsAtRiskOfDroppingOutReport] UNIQUE ([SchoolYear], [InstId], [StudentsAtRiskOfDroppingOutReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'StudentsAtRiskOfDroppingOutReportItem'
GO

CREATE TABLE [school_books].[StudentsAtRiskOfDroppingOutReportItem] (
    [SchoolYear]                                SMALLINT         NOT NULL,
    [StudentsAtRiskOfDroppingOutReportId]       INT              NOT NULL,
    [StudentsAtRiskOfDroppingOutReportItemId]   INT              NOT NULL,

    [PersonId]                                  INT              NOT NULL, -- no FK, we dont need a hard reference
    [PersonalId]                                NVARCHAR(100)    NOT NULL,
    [FirstName]                                 NVARCHAR(100)    NOT NULL,
    [MiddleName]                                NVARCHAR(100)    NOT NULL,
    [LastName]                                  NVARCHAR(100)    NOT NULL,
    [ClassBookName]                             NVARCHAR(100)    NOT NULL,
    [BasicClassId]                              INT              NULL,
    [UnexcusedAbsenceHoursCount]                DECIMAL(4,1)     NULL,
    [UnexcusedAbsenceDaysCount]                 INT              NULL,

    CONSTRAINT [PK_StudentsAtRiskOfDroppingOutReportItem] PRIMARY KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId], [StudentsAtRiskOfDroppingOutReportItemId]),
    CONSTRAINT [FK_StudentsAtRiskOfDroppingOutReportItem_StudentsAtRiskOfDroppingOutReport] FOREIGN KEY ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId]) REFERENCES [school_books].[StudentsAtRiskOfDroppingOutReport] ([SchoolYear], [StudentsAtRiskOfDroppingOutReportId])
)
WITH (DATA_COMPRESSION = PAGE);
GO
