PRINT 'Create Schedule table'
GO

EXEC [school_books].[spCreateIdSequence] N'Schedule'
GO

CREATE TABLE [school_books].[Schedule] (
    [SchoolYear]              SMALLINT         NOT NULL,
    [ScheduleId]              INT              NOT NULL,

    [ClassBookId]             INT              NOT NULL,
    [IsIndividualSchedule]    BIT              NOT NULL,
    [PersonId]                INT              NULL,
    [Term]                    INT              NULL,
    [StartDate]               DATE             NOT NULL,
    [EndDate]                 DATE             NOT NULL,
    [ShiftId]                 INT              NOT NULL,
    [IsRziApproved]           BIT              NOT NULL,
    [IncludesWeekend]         BIT              NOT NULL,
    [IsSplitting]             BIT              NOT NULL,

    [CreateDate]              DATETIME2        NOT NULL,
    [CreatedBySysUserId]      INT              NOT NULL,
    [ModifyDate]              DATETIME2        NOT NULL,
    [ModifiedBySysUserId]     INT              NOT NULL,
    [Version]                 ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Schedule] PRIMARY KEY ([SchoolYear], [ScheduleId]),

    CONSTRAINT [FK_Schedule_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId]) REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),
    CONSTRAINT [FK_Schedule_Shift] FOREIGN KEY ([SchoolYear], [ShiftId]) REFERENCES [school_books].[Shift] ([SchoolYear], [ShiftId]),

    CONSTRAINT [CHK_Schedule_Term] CHECK ([Term] IN (1, 2)),
    CONSTRAINT [CHK_Schedule_StartDate_EndDate] CHECK ([StartDate] <= [EndDate]),
    CONSTRAINT [CHK_Schedule_IsIndividualSchedule_PersonId] CHECK (
      ([IsIndividualSchedule] = 0 AND [PersonId] IS NULL) OR
      ([IsIndividualSchedule] = 1 AND [PersonId] IS NOT NULL)
    ),

    -- external references
    CONSTRAINT [FK_Schedule_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_Schedule_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Schedule_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_Schedule] UNIQUE ([SchoolYear], [ClassBookId], [ScheduleId]),
);
GO

exec school_books.spDescTable  N'Schedule', N'Учебно разписание.'

exec school_books.spDescColumn N'Schedule', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Schedule', N'ScheduleId'                , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Schedule', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'Schedule', N'IsIndividualSchedule'      , N'Разписанието е за ученик в индивидуална форма на обучение – Да/Не.'
exec school_books.spDescColumn N'Schedule', N'PersonId'                  , N'Идентификатор на ученик.'
exec school_books.spDescColumn N'Schedule', N'Term'                      , N'Срок. 1 - Първи срок, 2 - Втори срок.'
exec school_books.spDescColumn N'Schedule', N'StartDate'                 , N'Начална дата.'
exec school_books.spDescColumn N'Schedule', N'EndDate'                   , N'Крайна дата.'
exec school_books.spDescColumn N'Schedule', N'ShiftId'                   , N'Идентификатор на учебна смяна.'
exec school_books.spDescColumn N'Schedule', N'IsRziApproved'             , N'Одобрено от РЗИ – Да/Не. В момента не се попълва. Оставена за евентуално бъдещо разширение.'
exec school_books.spDescColumn N'Schedule', N'IncludesWeekend'           , N'Разписанието включва събота и неделя – Да/Не.'
exec school_books.spDescColumn N'Schedule', N'IsSplitting'               , N'В процес на разделяне – Да/Не.'

exec school_books.spDescColumn N'Schedule', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Schedule', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Schedule', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Schedule', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Schedule', N'Version'                   , N'Версия.'
