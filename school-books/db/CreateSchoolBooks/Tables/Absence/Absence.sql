PRINT 'Create Absence table'
GO

EXEC [school_books].[spCreateIdSequence] N'Absence'
GO

CREATE TABLE [school_books].[Absence] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [AbsenceId]             INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Type]                  INT              NOT NULL,
    [Term]                  INT              NOT NULL,
    [ExcusedReasonId]       INT              NULL,
    [ExcusedReasonComment]  NVARCHAR(1000)   NULL,
    [ScheduleLessonId]      INT              NOT NULL,
    [TeacherAbsenceId]      INT              NULL,
    [HisMedicalNoticeId]    INT              NULL,
    [IsReadFromParent]      BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Absence] PRIMARY KEY NONCLUSTERED ([SchoolYear], [AbsenceId]),
    CONSTRAINT [UK_Absence] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [AbsenceId]),
    CONSTRAINT [UK_Absence_PersonId_SchoolYear_ClassBookId_ScheduleLessonId]
        UNIQUE ([PersonId], [SchoolYear], [ClassBookId], [ScheduleLessonId]),

    CONSTRAINT [FK_Absence_ExcusedReason] FOREIGN KEY ([ExcusedReasonId])
        REFERENCES [school_books].[AbsenceReason] ([Id]),
    CONSTRAINT [FK_Absence_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Absence_ScheduleLesson] FOREIGN KEY ([SchoolYear], [ScheduleLessonId])
        REFERENCES [school_books].[ScheduleLesson] ([SchoolYear], [ScheduleLessonId]),
    CONSTRAINT [FK_Absence_TeacherAbsenceHour] FOREIGN KEY ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId])
        REFERENCES [school_books].[TeacherAbsenceHour] ([SchoolYear], [TeacherAbsenceId], [ScheduleLessonId]),
    CONSTRAINT [FK_Absence_PersonMedicalNotice] FOREIGN KEY ([SchoolYear], [PersonId], [HisMedicalNoticeId])
        REFERENCES [school_books].[PersonMedicalNotice] ([SchoolYear], [PersonId], [HisMedicalNoticeId]),

    CONSTRAINT [CHK_Absence_Type] CHECK ([Type] IN (1, 2, 3, 4, 5)),
    CONSTRAINT [CHK_Absence_Term] CHECK ([Term] IN (1, 2)),

    -- ExcusedReason and ExcusedReasonComment are allowed only for excused absences
    CONSTRAINT [CHK_Absences_ExcusedReason_ExcusedReasonComment] CHECK (
        [Type] = 3 OR
        ([ExcusedReasonId] IS NULL AND [ExcusedReasonComment] IS NULL)
    ),

    INDEX [IX_Absence_ScheduleLesson] ([SchoolYear], [ScheduleLessonId], [TeacherAbsenceId]),

    -- external references
    CONSTRAINT [FK_Absence_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Absence_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Absence_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Absence', N'Отсъствие.'

exec school_books.spDescColumn N'Absence', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Absence', N'AbsenceId'                 , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Absence', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Absence', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Absence', N'Date'                      , N'Дата, за която е въведено отсъствието.'
exec school_books.spDescColumn N'Absence', N'Type'                      , N'Вид на отсъствието. Стойности 4 и 5 са за секция Присъствия в ДГ. 1 - Закъснение, 2 - Отсъствие по неуважителни причини, 3 - Отсъствие по уважителни причини, 4 - Отсъствие, 5 - Присъствие.'
exec school_books.spDescColumn N'Absence', N'Term'                      , N'Срок. 1 - Първи срок, 2 - Втори срок.'
exec school_books.spDescColumn N'Absence', N'ExcusedReasonId'           , N'Причини за отсъствието. Номенклатура AbsenceReason.'
exec school_books.spDescColumn N'Absence', N'ExcusedReasonComment'      , N'Причини за отсъствието - забележка.'
exec school_books.spDescColumn N'Absence', N'ScheduleLessonId'          , N'Идентификатор на часа от учебното разписание.'
exec school_books.spDescColumn N'Absence', N'TeacherAbsenceId'          , N'Идентификатор на заместване.'
exec school_books.spDescColumn N'Absence', N'HisMedicalNoticeId'        , N'Идентификатор на медицинска бележка.'
exec school_books.spDescColumn N'Absence', N'IsReadFromParent'          , N'Прочетено от родител – да/не.'

exec school_books.spDescColumn N'Absence', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Absence', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Absence', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Absence', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Absence', N'Version'                   , N'Версия.'
