PRINT 'Create Topic table'
GO

EXEC [school_books].[spCreateIdSequence] N'Topic'
GO

CREATE TABLE [school_books].[Topic] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicId]                   INT             NOT NULL,

    [ClassBookId]               INT             NOT NULL,
    [Title]                     NVARCHAR(1000)  NULL, -- TODO: remove if the switch to TopicTitle is successful
    [Date]                      DATE            NOT NULL,
    [ScheduleLessonId]          INT             NOT NULL,
    [TeacherAbsenceId]          INT             NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [CreatedBySysUserId]        INT             NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_Topic] PRIMARY KEY NONCLUSTERED ([SchoolYear], [TopicId]),
    CONSTRAINT [UK_Topic] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [TopicId]),
    CONSTRAINT [UK_Topic_ScheduleLesson] UNIQUE ([SchoolYear], [ScheduleLessonId]),

    CONSTRAINT [FK_Topic_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Topic_ScheduleLesson] FOREIGN KEY ([SchoolYear], [ScheduleLessonId])
        REFERENCES [school_books].[ScheduleLesson] ([SchoolYear], [ScheduleLessonId]),
    CONSTRAINT [FK_Topic_TeacherAbsenceHour] FOREIGN KEY ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId])
        REFERENCES [school_books].[TeacherAbsenceHour] ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId]),

    INDEX [IX_Topic_ScheduleLesson] ([SchoolYear], [ScheduleLessonId], [TeacherAbsenceId]),

    -- external references
    CONSTRAINT [FK_Topic_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Topic', N'Тема на часа.'

exec school_books.spDescColumn N'Topic', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Topic', N'TopicId'                   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Topic', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Topic', N'Date'                      , N'Дата на темата на часа.'
exec school_books.spDescColumn N'Topic', N'ScheduleLessonId'          , N'Идентификатор на часа от учебното разписание.'
exec school_books.spDescColumn N'Topic', N'TeacherAbsenceId'          , N'Идентификатор на заместване.'

exec school_books.spDescColumn N'Topic', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Topic', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Topic', N'Version'                   , N'Версия.'
