exec [school_books].[spCreateIdSequence] N'LectureSchedule'
GO

CREATE TABLE [school_books].[LectureSchedule] (
    [SchoolYear]               SMALLINT         NOT NULL,
    [LectureScheduleId]        INT              NOT NULL,

    [InstId]                   INT              NOT NULL,
    [TeacherPersonId]          INT              NOT NULL,
    [OrderNumber]              NVARCHAR(100)    NOT NULL,
    [OrderDate]                DATE             NOT NULL,
    [StartDate]                DATE             NOT NULL,
    [EndDate]                  DATE             NOT NULL,

    [CreateDate]               DATETIME2        NOT NULL,
    [CreatedBySysUserId]       INT              NOT NULL,
    [ModifyDate]               DATETIME2        NOT NULL,
    [ModifiedBySysUserId]      INT              NOT NULL,
    [Version]                  ROWVERSION       NOT NULL,

    CONSTRAINT [PK_LectureSchedule] PRIMARY KEY ([SchoolYear], [LectureScheduleId]),

    CONSTRAINT [CHK_LectureSchedule_StartDate_EndDate] CHECK ([StartDate] <= [EndDate]),

    -- external references
    CONSTRAINT [FK_LectureSchedule_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_LectureSchedule_TeacherPersonId] FOREIGN KEY ([TeacherPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_LectureSchedule_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_LectureSchedule_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_LectureSchedule] UNIQUE ([SchoolYear], [InstId], [LectureScheduleId])
);
GO

CREATE TABLE [school_books].[LectureScheduleHour] (
    [SchoolYear]                   SMALLINT        NOT NULL,
    [LectureScheduleId]            INT             NOT NULL,
    [ScheduleLessonId]             INT             NOT NULL,

    CONSTRAINT [PK_LectureScheduleHour] PRIMARY KEY ([SchoolYear], [LectureScheduleId], [ScheduleLessonId]),
    CONSTRAINT [FK_LectureScheduleHour_LectureSchedule] FOREIGN KEY ([SchoolYear], [LectureScheduleId])
        REFERENCES [school_books].[LectureSchedule] ([SchoolYear], [LectureScheduleId]),
    CONSTRAINT [FK_LectureScheduleHour_ScheduleLesson] FOREIGN KEY ([SchoolYear], [ScheduleLessonId])
        REFERENCES [school_books].[ScheduleLesson] ([SchoolYear], [ScheduleLessonId]),
);
GO
