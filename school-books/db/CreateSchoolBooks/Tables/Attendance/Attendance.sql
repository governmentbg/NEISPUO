PRINT 'Create Attendance table'
GO

EXEC [school_books].[spCreateIdSequence] N'Attendance'
GO

CREATE TABLE [school_books].[Attendance] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [AttendanceId]          INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [PersonId]              INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [Type]                  INT              NOT NULL,
    [ExcusedReasonId]       INT              NULL,
    [ExcusedReasonComment]  NVARCHAR(1000)   NULL,
    [HisMedicalNoticeId]    INT              NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Attendance] PRIMARY KEY NONCLUSTERED ([SchoolYear], [AttendanceId]),
    CONSTRAINT [UK_Attendance] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [AttendanceId]),
    CONSTRAINT [UK_Attendance_ClassBookId_PersonId_Date] UNIQUE ([SchoolYear], [ClassBookId], [PersonId], [Date]),

    CONSTRAINT [FK_Attendance_ExcusedReason] FOREIGN KEY ([ExcusedReasonId])
        REFERENCES [school_books].[AbsenceReason] ([Id]),
    CONSTRAINT [FK_Attendance_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Attendance_PersonMedicalNotice] FOREIGN KEY ([SchoolYear], [PersonId], [HisMedicalNoticeId])
        REFERENCES [school_books].[PersonMedicalNotice] ([SchoolYear], [PersonId], [HisMedicalNoticeId]),

    CONSTRAINT [CHK_Attendance_Type] CHECK ([Type] IN (1, 2, 3)),

    -- ExcusedReason and ExcusedReasonComment are allowed only for excused absences
    CONSTRAINT [CHK_Attendance_ExcusedReason_ExcusedReasonComment] CHECK (
        [Type] = 3 OR
        ([ExcusedReasonId] IS NULL AND [ExcusedReasonComment] IS NULL)
    ),

    -- external references
    CONSTRAINT [FK_Attendance_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Attendance_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Attendance_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'Attendance', N'Присъствие.'

exec school_books.spDescColumn N'Attendance', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Attendance', N'AttendanceId'              , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Attendance', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Attendance', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Attendance', N'Date'                      , N'Дата, за която е въведено отсъствието.'
exec school_books.spDescColumn N'Attendance', N'Type'                      , N'Вид на отсъствието. 1 - Присъствие, 2 - Отсъствие по неуважителни причини, 3 - Отсъствие по уважителни причини'
exec school_books.spDescColumn N'Attendance', N'ExcusedReasonId'           , N'Причини за отсъствието. Номенклатура AbsenceReason.'
exec school_books.spDescColumn N'Attendance', N'ExcusedReasonComment'      , N'Причини за отсъствието - забележка.'
exec school_books.spDescColumn N'Attendance', N'HisMedicalNoticeId'        , N'Идентификатор на медицинска бележка.'

exec school_books.spDescColumn N'Attendance', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Attendance', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Attendance', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Attendance', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Attendance', N'Version'                   , N'Версия.'
