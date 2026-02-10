PRINT 'Create LectureScheduleHour table'
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

    INDEX [IX_LectureScheduleHour_ScheduleLesson] UNIQUE ([SchoolYear], [ScheduleLessonId]),
);
GO

exec school_books.spDescTable  N'LectureScheduleHour', N'Учителско отсъствие - час, за който се създава промяна в разписанието/заместване.'

exec school_books.spDescColumn N'LectureScheduleHour', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'LectureScheduleHour', N'LectureScheduleId'              , N'Идентификатор на лекторски график.'
exec school_books.spDescColumn N'LectureScheduleHour', N'ScheduleLessonId'               , N'Идентификатор на часа от учебното разписание.'
