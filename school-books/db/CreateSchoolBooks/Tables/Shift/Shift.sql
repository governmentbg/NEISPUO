PRINT 'Create Shift table'
GO

EXEC [school_books].[spCreateIdSequence] N'Shift'
GO

CREATE TABLE [school_books].[Shift] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ShiftId]               INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [Name]                  NVARCHAR(100)    NOT NULL,
    [IsMultiday]            BIT              NOT NULL,
    [IsAdhoc]               BIT              NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_Shift] PRIMARY KEY ([SchoolYear], [ShiftId]),

    -- external references
    CONSTRAINT [FK_Shift_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_Shift_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_Shift_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_Shift] UNIQUE ([SchoolYear], [InstId], [ShiftId]),
);
GO

exec school_books.spDescTable  N'Shift', N'Учебна смяна.'

exec school_books.spDescColumn N'Shift', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'Shift', N'ShiftId'                   , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'Shift', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'Shift', N'Name'                      , N'Наименование.'
exec school_books.spDescColumn N'Shift', N'IsMultiday'                , N'Смяна с различни режими за отделните дни – Да/Не.'
exec school_books.spDescColumn N'Shift', N'IsAdhoc'                   , N'Собствена смяна към разписание – Да/Не.'

exec school_books.spDescColumn N'Shift', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'Shift', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'Shift', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'Shift', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'Shift', N'Version'                   , N'Версия.'
