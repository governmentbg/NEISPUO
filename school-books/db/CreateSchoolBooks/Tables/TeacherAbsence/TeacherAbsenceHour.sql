PRINT 'Create TeacherAbsenceHour table'
GO

CREATE TABLE [school_books].[TeacherAbsenceHour] (
    [SchoolYear]                   SMALLINT        NOT NULL,
    [TeacherAbsenceId]             INT             NOT NULL,
    [ScheduleLessonId]             INT             NOT NULL,

    [ReplTeacherPersonId]          INT             NULL,
    [ReplTeacherIsNonSpecialist]   BIT             NULL,
    [ExtReplTeacherName]           NVARCHAR(550)   NULL,

    CONSTRAINT [PK_TeacherAbsenceHour] PRIMARY KEY ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId]),
    CONSTRAINT [FK_TeacherAbsenceHour_TeacherAbsence] FOREIGN KEY ([SchoolYear], [TeacherAbsenceId])
        REFERENCES [school_books].[TeacherAbsence] ([SchoolYear], [TeacherAbsenceId]),
    CONSTRAINT [FK_TeacherAbsenceHour_ScheduleLesson] FOREIGN KEY ([SchoolYear], [ScheduleLessonId])
        REFERENCES [school_books].[ScheduleLesson] ([SchoolYear], [ScheduleLessonId]),
    CONSTRAINT [CHK_TeacherAbsenceHour_ReplTeacherPersonId_ReplTeacherIsNonSpecialist] CHECK (
        -- Empty hour
        ([ReplTeacherPersonId] IS NULL AND [ReplTeacherIsNonSpecialist] IS NULL) OR
        -- Replacement teacher - Specialist or NonSpecialist
        ([ReplTeacherPersonId] IS NOT NULL AND [ReplTeacherIsNonSpecialist] IS NOT NULL)
    ),

    INDEX [IX_TeacherAbsenceHour_ScheduleLesson] UNIQUE ([SchoolYear], [ScheduleLessonId]) INCLUDE ([TeacherAbsenceId], [ReplTeacherPersonId], [ReplTeacherIsNonSpecialist]),
    INDEX [IX_TeacherAbsenceHour_ReplTeacherPersonId] ([SchoolYear], [ReplTeacherPersonId]),

    -- external references
    CONSTRAINT [FK_TeacherAbsenceHour_ReplTeacherPersonId] FOREIGN KEY ([ReplTeacherPersonId]) REFERENCES [core].[Person] ([PersonID]),
);
GO

exec school_books.spDescTable  N'TeacherAbsenceHour', N'Учителско отсъствие - час, за който се създава промяна в разписанието/заместване.'

exec school_books.spDescColumn N'TeacherAbsenceHour', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'TeacherAbsenceHour', N'TeacherAbsenceId'               , N'Идентификатор на учителско отсъствие.'
exec school_books.spDescColumn N'TeacherAbsenceHour', N'ScheduleLessonId'               , N'Идентификатор на часа от учебното разписание.'

exec school_books.spDescColumn N'TeacherAbsenceHour', N'ReplTeacherPersonId'            , N'Идентификатор на заместника.'
exec school_books.spDescColumn N'TeacherAbsenceHour', N'ReplTeacherIsNonSpecialist'     , N'Заместник неспециалист (Час по ГО) – Да/Не.'
exec school_books.spDescColumn N'TeacherAbsenceHour', N'ExtReplTeacherName'             , N'Име на външен лектор.'
