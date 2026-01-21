PRINT 'Create ParentMeeting table'
GO

EXEC [school_books].[spCreateIdSequence] N'ParentMeeting'
GO

CREATE TABLE [school_books].[ParentMeeting] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [ParentMeetingId]       INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [Date]                  DATE             NOT NULL,
    [StartTime]             TIME             NOT NULL,
    [Location]              NVARCHAR(100)    NULL,
    [Title]                 NVARCHAR(100)    NOT NULL,
    [Description]           NVARCHAR(1000)   NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ParentMeeting] PRIMARY KEY ([SchoolYear], [ParentMeetingId]),
    CONSTRAINT [UK_ParentMeeting] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [ParentMeetingId]),

    CONSTRAINT [FK_ParentMeeting_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_ParentMeeting_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ParentMeeting_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'ParentMeeting', N'Родителска среща.'

exec school_books.spDescColumn N'ParentMeeting', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ParentMeeting', N'ParentMeetingId'           , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ParentMeeting', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'ParentMeeting', N'Date'                      , N'Дата на родителската среща.'
exec school_books.spDescColumn N'ParentMeeting', N'StartTime'                 , N'Начален час.'
exec school_books.spDescColumn N'ParentMeeting', N'Location'                  , N'Място.'
exec school_books.spDescColumn N'ParentMeeting', N'Title'                     , N'Тема.'
exec school_books.spDescColumn N'ParentMeeting', N'Description'               , N'Описание.'

exec school_books.spDescColumn N'ParentMeeting', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ParentMeeting', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'ParentMeeting', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ParentMeeting', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'ParentMeeting', N'Version'                   , N'Версия.'

