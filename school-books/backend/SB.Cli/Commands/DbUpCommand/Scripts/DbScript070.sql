EXEC [school_books].[spCreateIdSequence] N'LectureSchedulesReport'
GO

CREATE TABLE [school_books].[LectureSchedulesReport] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [LectureSchedulesReportId]  INT              NOT NULL,

    [InstId]                    INT              NOT NULL,
    [Period]                    INT              NOT NULL,
    [Year]                      INT              NULL,
    [Month]                     INT              NULL,
    [TeacherPersonId]           INT              NULL, -- no FK, we dont need a hard reference
    [TeacherPersonName]         NVARCHAR(1000)   NULL,

    [CreateDate]                DATETIME2        NOT NULL,
    [CreatedBySysUserId]        INT              NOT NULL,
    [ModifyDate]                DATETIME2        NOT NULL,
    [ModifiedBySysUserId]       INT              NOT NULL,
    [Version]                   ROWVERSION       NOT NULL,

    CONSTRAINT [PK_LectureSchedulesReport] PRIMARY KEY ([SchoolYear], [LectureSchedulesReportId]),
    CONSTRAINT [CHK_LectureSchedulesReport_Period] CHECK ([Period] IN (1, 2, 3, 4)),

    -- external references
    CONSTRAINT [FK_LectureSchedulesReport_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_LectureSchedulesReport_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_LectureSchedulesReport_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_LectureSchedulesReport] UNIQUE ([SchoolYear], [InstId], [LectureSchedulesReportId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'LectureSchedulesReportItem'
GO

CREATE TABLE [school_books].[LectureSchedulesReportItem] (
    [SchoolYear]                      SMALLINT         NOT NULL,
    [LectureSchedulesReportId]        INT              NOT NULL,
    [LectureSchedulesReportItemId]    INT              NOT NULL,

    [TeacherPersonId]                 INT              NOT NULL, -- no FK, we dont need a hard reference
    [TeacherPersonName]               NVARCHAR(1000)   NOT NULL,
    [Date]                            DATE             NOT NULL,
    [ClassBookId]                     INT              NOT NULL, -- no FK, we dont need a hard reference
    [ClassBookName]                   NVARCHAR(1000)   NOT NULL,
    [CurriculumId]                    INT              NOT NULL, -- no FK, we dont need a hard reference
    [CurriculumName]                  NVARCHAR(1000)   NOT NULL,
    [LectureScheduleId]               INT              NOT NULL, -- no FK, we dont need a hard reference
    [OrderNumber]                     NVARCHAR(1000)   NOT NULL,
    [OrderDate]                       DATE             NOT NULL,
    [HoursTaken]                      INT              NOT NULL,

    CONSTRAINT [PK_LectureSchedulesReportItem] PRIMARY KEY ([SchoolYear], [LectureSchedulesReportId], [LectureSchedulesReportItemId]),
    CONSTRAINT [FK_LectureSchedulesReportItem_LectureSchedulesReport] FOREIGN KEY ([SchoolYear], [LectureSchedulesReportId]) REFERENCES [school_books].[LectureSchedulesReport] ([SchoolYear], [LectureSchedulesReportId])
);
GO
