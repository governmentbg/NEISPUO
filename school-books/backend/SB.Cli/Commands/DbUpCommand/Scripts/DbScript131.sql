EXEC [school_books].[spCreateIdSequence] N'ScheduleAndAbsencesByTermAllClassesReport'
GO

CREATE TABLE [school_books].[ScheduleAndAbsencesByTermAllClassesReport] (
    [SchoolYear]                                    SMALLINT         NOT NULL,
    [ScheduleAndAbsencesByTermAllClassesReportId]   INT              NOT NULL,

    [InstId]                                        INT              NOT NULL,
    [Term]                                          INT              NOT NULL,
    [BlobId]                                        INT              NOT NULL,

    [CreateDate]                                    DATETIME2        NOT NULL,
    [CreatedBySysUserId]                            INT              NOT NULL,
    [ModifyDate]                                    DATETIME2        NOT NULL,
    [ModifiedBySysUserId]                           INT              NOT NULL,
    [Version]                                       ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ScheduleAndAbsencesByTermAllClassesReport] PRIMARY KEY ([SchoolYear], [ScheduleAndAbsencesByTermAllClassesReportId]),

    CONSTRAINT [CHK_ScheduleAndAbsencesByTermAllClassesReport_Term] CHECK ([Term] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_ScheduleAndAbsencesByTermAllClassesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermAllClassesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ScheduleAndAbsencesByTermAllClassesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ScheduleAndAbsencesByTermAllClassesReport] UNIQUE ([SchoolYear], [InstId], [ScheduleAndAbsencesByTermAllClassesReportId]),
);
GO
