CREATE TABLE [school_books].[TopicStudent] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,

    CONSTRAINT [PK_TopicStudent] PRIMARY KEY ([SchoolYear], [TopicId], [PersonId]),

    CONSTRAINT [FK_TopicStudent_Topic] FOREIGN KEY ([SchoolYear], [TopicId])
        REFERENCES [school_books].[Topic] ([SchoolYear], [TopicId]),

    INDEX [IX_TopicStudent_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

EXEC [school_books].[spCreateIdSequence] N'TopicDplr'
GO

CREATE TABLE [school_books].[TopicDplr] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,

    [ClassBookId]               INT             NOT NULL,
    [Date]                      DATE            NOT NULL,
    [Day]                       INT             NOT NULL,
    [HourNumber]                INT             NOT NULL,
    [StartTime]                 TIME            NOT NULL,
    [EndTime]                   TIME            NOT NULL,
    [CurriculumId]              INT             NOT NULL,
    [Title]                     NVARCHAR(1000)  NOT NULL,
    [Location]                  NVARCHAR(550)   NULL,
    [IsVerified]                BIT             NOT NULL,
    [VerifyDate]                DATETIME2       NULL,
    [VerifiedBySysUserId]       INT             NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [CreatedBySysUserId]        INT             NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_TopicDplr] PRIMARY KEY NONCLUSTERED ([SchoolYear], [TopicDplrId]),
    CONSTRAINT [UK_TopicDplr] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [TopicDplrId]),

    CONSTRAINT [FK_TopicDplr_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_TopicDplr_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

CREATE TABLE [school_books].[TopicDplrStudent] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,

    CONSTRAINT [PK_TopicDplrStudent] PRIMARY KEY ([SchoolYear], [TopicDplrId], [PersonId]),

    CONSTRAINT [FK_TopicDplrStudent_TopicDplr] FOREIGN KEY ([SchoolYear], [TopicDplrId])
        REFERENCES [school_books].[TopicDplr] ([SchoolYear], [TopicDplrId]),

    INDEX [IX_TopicDplrStudent_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicDplrStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

CREATE TABLE [school_books].[TopicDplrTeacher] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,
    [PersonId]                  INT             NOT NULL,

    CONSTRAINT [PK_TopicDplrTeacher] PRIMARY KEY ([SchoolYear], [TopicDplrId], [PersonId]),

    CONSTRAINT [FK_TopicDplrTeacher_TopicDplr] FOREIGN KEY ([SchoolYear], [TopicDplrId])
        REFERENCES [school_books].[TopicDplr] ([SchoolYear], [TopicDplrId]),

    INDEX [IX_TopicDplrTeacher_PersonId] ([SchoolYear], [PersonId]),

    -- external references
    CONSTRAINT [FK_TopicDplrTeacher_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonId]),
);
GO

CREATE OR ALTER VIEW [school_books].[vwEplrHoursTaken] AS
(
    SELECT ts.PersonId AS StudentPersonId, t.SchoolYear, cb.InstId, t.ClassBookId, sl.CurriculumId,
           tt.PersonId AS TeacherPersonId, sl.Date, sl.HourNumber
    FROM [school_books].[Topic] t
    INNER JOIN [school_books].[ClassBook] cb ON t.SchoolYear = cb.SchoolYear AND t.ClassBookId = cb.ClassBookId
    INNER JOIN [school_books].[ScheduleLesson] sl ON t.SchoolYear = sl.SchoolYear AND t.ScheduleLessonId = sl.ScheduleLessonId
    INNER JOIN [school_books].[TopicStudent] ts ON t.SchoolYear = ts.SchoolYear AND t.TopicId = ts.TopicId
    INNER JOIN [school_books].[TopicTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicId = tt.TopicId

    UNION ALL

    SELECT ts.PersonId AS StudentPersonId, t.SchoolYear, cb.InstId, t.ClassBookId, t.CurriculumId,
           tt.PersonId AS TeacherPersonId, t.Date, t.HourNumber
    FROM [school_books].[TopicDplr] t
    INNER JOIN [school_books].[ClassBook] cb ON t.SchoolYear = cb.SchoolYear AND t.ClassBookId = cb.ClassBookId
    INNER JOIN [school_books].[TopicDplrStudent] ts ON t.SchoolYear = ts.SchoolYear AND t.TopicDplrId = ts.TopicDplrId
    INNER JOIN [school_books].[TopicDplrTeacher] tt ON t.SchoolYear = tt.SchoolYear AND t.TopicDplrId = tt.TopicDplrId
);
GO
