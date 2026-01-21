PRINT 'Create TeacherAbsence table'
GO

EXEC [school_books].[spCreateIdSequence] N'TeacherAbsence'
GO

CREATE TABLE [school_books].[TeacherAbsence] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [TeacherAbsenceId]      INT              NOT NULL,

    [InstId]                INT              NOT NULL,
    [TeacherPersonId]       INT              NOT NULL,
    [StartDate]             DATE             NOT NULL,
    [EndDate]               DATE             NOT NULL,
    [Reason]                NVARCHAR(1000)   NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_TeacherAbsence] PRIMARY KEY ([SchoolYear], [TeacherAbsenceId]),

    CONSTRAINT [CHK_TeacherAbsence_StartDate_EndDate] CHECK ([StartDate] <= [EndDate]),

    -- external references
    CONSTRAINT [FK_TeacherAbsence_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_TeacherAbsence_TeacherPersonId] FOREIGN KEY ([TeacherPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_TeacherAbsence_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_TeacherAbsence_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_TeacherAbsence] UNIQUE ([SchoolYear], [InstId], [TeacherAbsenceId]),
);
GO

exec school_books.spDescTable  N'TeacherAbsence', N'Учителско отсъствие.'

exec school_books.spDescColumn N'TeacherAbsence', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TeacherAbsence', N'TeacherAbsenceId'          , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'TeacherAbsence', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'TeacherAbsence', N'TeacherPersonId'           , N'Идентификатор на учител.'
exec school_books.spDescColumn N'TeacherAbsence', N'StartDate'                 , N'Начална дата на отсъствието.'
exec school_books.spDescColumn N'TeacherAbsence', N'EndDate'                   , N'Крайна дата на отсъствието.'
exec school_books.spDescColumn N'TeacherAbsence', N'Reason'                    , N'Причини за отсъствието.'

exec school_books.spDescColumn N'TeacherAbsence', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'TeacherAbsence', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'TeacherAbsence', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'TeacherAbsence', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'TeacherAbsence', N'Version'                   , N'Версия.'
