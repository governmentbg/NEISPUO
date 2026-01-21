PRINT 'Create LectureSchedule table'
GO

exec [school_books].[spCreateIdSequence] N'LectureSchedule'
GO

CREATE TABLE [school_books].[LectureSchedule] (
    [SchoolYear]               SMALLINT         NOT NULL,
    [LectureScheduleId]        INT              NOT NULL,

    [InstId]                   INT              NOT NULL,
    [TeacherPersonId]          INT              NOT NULL,
    [OrderNumber]              NVARCHAR(100)    NOT NULL,
    [OrderDate]                DATE             NOT NULL,
    [StartDate]                DATE             NOT NULL,
    [EndDate]                  DATE             NOT NULL,

    [CreateDate]               DATETIME2        NOT NULL,
    [CreatedBySysUserId]       INT              NOT NULL,
    [ModifyDate]               DATETIME2        NOT NULL,
    [ModifiedBySysUserId]      INT              NOT NULL,
    [Version]                  ROWVERSION       NOT NULL,

    CONSTRAINT [PK_LectureSchedule] PRIMARY KEY ([SchoolYear], [LectureScheduleId]),

    CONSTRAINT [CHK_LectureSchedule_StartDate_EndDate] CHECK ([StartDate] <= [EndDate]),

    -- external references
    CONSTRAINT [FK_LectureSchedule_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_LectureSchedule_TeacherPersonId] FOREIGN KEY ([TeacherPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_LectureSchedule_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_LectureSchedule_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_LectureSchedule] UNIQUE ([SchoolYear], [InstId], [LectureScheduleId])
);
GO

exec school_books.spDescTable  N'LectureSchedule', N'Лекторски график.'

exec school_books.spDescColumn N'LectureSchedule', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'LectureSchedule', N'LectureScheduleId'         , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'LectureSchedule', N'InstId'                    , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'LectureSchedule', N'TeacherPersonId'           , N'Идентификатор на учител.'
exec school_books.spDescColumn N'LectureSchedule', N'OrderNumber'               , N'Номер на заповед.'
exec school_books.spDescColumn N'LectureSchedule', N'OrderDate'                 , N'Дата на заповед.'
exec school_books.spDescColumn N'LectureSchedule', N'StartDate'                 , N'Начална дата на лекторския график.'
exec school_books.spDescColumn N'LectureSchedule', N'EndDate'                   , N'Крайна дата на лекторския график.'

exec school_books.spDescColumn N'LectureSchedule', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'LectureSchedule', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'LectureSchedule', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'LectureSchedule', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'LectureSchedule', N'Version'                   , N'Версия.'
