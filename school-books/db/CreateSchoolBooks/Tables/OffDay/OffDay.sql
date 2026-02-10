PRINT 'Create OffDay table'
GO

EXEC [school_books].[spCreateIdSequence] N'OffDay'
GO

CREATE TABLE [school_books].[OffDay] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [OffDayId]              INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [From]                  DATE             NOT NULL,
    [To]                    DATE             NOT NULL,
    [Description]           NVARCHAR(1000)   NOT NULL,
    [IsPgOffProgramDay]     BIT              NOT NULL,
    [IsForAllClasses]       BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_OffDay] PRIMARY KEY ([SchoolYear], [OffDayId]),

    -- external references
    CONSTRAINT [FK_OffDay_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_OffDay_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_OffDay_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_OffDay] UNIQUE ([SchoolYear], [InstId], [OffDayId]),
);
GO

exec school_books.spDescTable  N'OffDay', N'Неучебен период.'

exec school_books.spDescColumn N'OffDay', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'OffDay', N'OffDayId'                  , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'OffDay', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'OffDay', N'From'                      , N'Начална дата на периода.'
exec school_books.spDescColumn N'OffDay', N'To'                        , N'Крайна дата на периода.'
exec school_books.spDescColumn N'OffDay', N'Description'               , N'Описание.'
exec school_books.spDescColumn N'OffDay', N'IsPgOffProgramDay'         , N'Неучебните дни важат за всички класове - да/не'
exec school_books.spDescColumn N'OffDay', N'IsForAllClasses'           , N'Позволено е въвеждане на присъствия в ПГ за дните от понеделник до петък - да/не.'

exec school_books.spDescColumn N'OffDay', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'OffDay', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'OffDay', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'OffDay', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'OffDay', N'Version'                   , N'Версия.'
