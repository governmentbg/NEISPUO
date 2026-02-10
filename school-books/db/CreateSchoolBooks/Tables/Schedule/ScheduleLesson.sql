PRINT 'Create ScheduleLesson table'
GO

EXEC [school_books].[spCreateIdSequence] N'ScheduleLesson'
GO

CREATE TABLE [school_books].[ScheduleLesson] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ScheduleLessonId]      INT              NOT NULL,

    [ScheduleId]            INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Day]                   INT              NOT NULL,
    [HourNumber]            INT              NOT NULL,
    [CurriculumId]          INT              NOT NULL,

    [IsVerified]            BIT              NOT NULL,
    [VerifyDate]            DATETIME2        NULL,
    [VerifiedBySysUserId]   INT              NULL,

    CONSTRAINT [PK_ScheduleLesson] PRIMARY KEY ([SchoolYear], [ScheduleLessonId]),

    CONSTRAINT [UQ_ScheduleLesson_SchoolYear_ScheduleId_Date_HourNumber_CurriculumId]
        UNIQUE ([SchoolYear], [ScheduleId], [Date], [HourNumber], [CurriculumId]),

    CONSTRAINT [FK_ScheduleLesson_Schedule] FOREIGN KEY ([SchoolYear], [ScheduleId])
        REFERENCES [school_books].[Schedule] ([SchoolYear], [ScheduleId]),
    CONSTRAINT [FK_ScheduleLesson_ScheduleDate] FOREIGN KEY ([SchoolYear], [ScheduleId], [Date])
        REFERENCES [school_books].[ScheduleDate] ([SchoolYear], [ScheduleId], [Date]),

    INDEX [IX_ScheduleLesson] UNIQUE ([SchoolYear], [ScheduleId], [ScheduleLessonId]),
    -- ScheduleHour deletetion helper indexes
    INDEX [IX_ScheduleLesson_ScheduleHour] ([SchoolYear], [ScheduleId], [Day], [HourNumber], [CurriculumId]),

    -- external references
    CONSTRAINT [FK_ScheduleLesson_Curriculum] FOREIGN KEY ([CurriculumId]) REFERENCES [inst_year].[Curriculum] ([CurriculumID]),
    CONSTRAINT [FK_ScheduleLesson_VerifiedBySysUserId] FOREIGN KEY ([VerifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_ScheduleLesson_CurriculumId] ([CurriculumId] ASC) INCLUDE ([ScheduleId]),
);
GO

exec school_books.spDescTable  N'ScheduleLesson', N'Учебно разписание - занятие.'

exec school_books.spDescColumn N'ScheduleLesson', N'SchoolYear'           , N'Учебна година.'
exec school_books.spDescColumn N'ScheduleLesson', N'ScheduleLessonId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ScheduleLesson', N'ScheduleId'           , N'Идентификатор на учебно разписание.'
exec school_books.spDescColumn N'ScheduleLesson', N'Date'                 , N'Дата на занятието.'
exec school_books.spDescColumn N'ScheduleLesson', N'Day'                  , N'Ден от седмицата. Число от 1 (Понеделник) до 7 (Неделя).'
exec school_books.spDescColumn N'ScheduleLesson', N'HourNumber'           , N'Номер на часа.'
exec school_books.spDescColumn N'ScheduleLesson', N'CurriculumId'         , N'Идентификатор на предмет от учебния план.'

exec school_books.spDescColumn N'ScheduleLesson', N'IsVerified'           , N'Проверен.'
exec school_books.spDescColumn N'ScheduleLesson', N'VerifyDate'           , N'Дата на проверка на записа.'
exec school_books.spDescColumn N'ScheduleLesson', N'VerifiedBySysUserId'  , N'Проверен от.'
