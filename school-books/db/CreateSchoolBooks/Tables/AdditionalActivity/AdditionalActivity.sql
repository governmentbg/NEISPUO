PRINT 'Create AdditionalActivity table'
GO

EXEC [school_books].[spCreateIdSequence] N'AdditionalActivity'
GO

CREATE TABLE [school_books].[AdditionalActivity] (
    [SchoolYear]            SMALLINT         NOT NULL,
    [AdditionalActivityId]  INT              NOT NULL,

    [ClassBookId]           INT              NOT NULL,
    [Year]                  INT              NOT NULL,
    [WeekNumber]            INT              NOT NULL,
    [Activity]              NVARCHAR(MAX)    NOT NULL,

    [CreateDate]            DATETIME2        NOT NULL,
    [CreatedBySysUserId]    INT              NOT NULL,
    [ModifyDate]            DATETIME2        NOT NULL,
    [ModifiedBySysUserId]   INT              NOT NULL,
    [Version]               ROWVERSION       NOT NULL,

    CONSTRAINT [PK_AdditionalActivity] PRIMARY KEY ([SchoolYear], [AdditionalActivityId]),
    CONSTRAINT [UK_AdditionalActivity] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [AdditionalActivityId]),

    CONSTRAINT [FK_AdditionalActivity_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_AdditionalActivity_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_AdditionalActivity_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'AdditionalActivity', N'Допълнителна дейност.'

exec school_books.spDescColumn N'AdditionalActivity', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'AdditionalActivity', N'AdditionalActivityId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'AdditionalActivity', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'AdditionalActivity', N'Year'                      , N'Година на седмицата по ISO 8601.'
exec school_books.spDescColumn N'AdditionalActivity', N'WeekNumber'                , N'Номер на седмицата по ISO 8601. Число от 1 до 53.'
exec school_books.spDescColumn N'AdditionalActivity', N'Activity'                  , N'Тема.'

exec school_books.spDescColumn N'AdditionalActivity', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'AdditionalActivity', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'AdditionalActivity', N'ModifyDate'                , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'AdditionalActivity', N'ModifiedBySysUserId'       , N'Последна модификация от.'
exec school_books.spDescColumn N'AdditionalActivity', N'Version'                   , N'Версия.'
