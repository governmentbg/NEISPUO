PRINT 'Create TopicDplr table'
GO

EXEC [school_books].[spCreateIdSequence] N'TopicDplr'
GO

CREATE TABLE [school_books].[TopicDplr] (
    [SchoolYear]                SMALLINT        NOT NULL,
    [TopicDplrId]               INT             NOT NULL,

    [ClassBookId]               INT             NOT NULL,
    [Date]                      DATE            NOT NULL,
    [Day]                       INT             NOT NULL,
    [HourNumber]                INT             NOT NULL,
    [StartTime]                 TIME            NOT NULL,
    [EndTime]                   TIME            NOT NULL,
    [CurriculumId]              INT             NOT NULL,
    [Title]                     NVARCHAR(1000)  NOT NULL,
    [Location]                  NVARCHAR(550)   NULL,
    [IsVerified]                BIT             NOT NULL,
    [VerifyDate]                DATETIME2       NULL,
    [VerifiedBySysUserId]       INT             NULL,

    [CreateDate]                DATETIME2       NOT NULL,
    [CreatedBySysUserId]        INT             NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_TopicDplr] PRIMARY KEY NONCLUSTERED ([SchoolYear], [TopicDplrId]),
    CONSTRAINT [UK_TopicDplr] UNIQUE CLUSTERED ([SchoolYear], [ClassBookId], [TopicDplrId]),

    CONSTRAINT [FK_TopicDplr_ClassBook] FOREIGN KEY ([SchoolYear], [ClassBookId])
        REFERENCES [school_books].[ClassBook] ([SchoolYear], [ClassBookId]),

    -- external references
    CONSTRAINT [FK_TopicDplr_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
);
GO

exec school_books.spDescTable  N'TopicDplr', N'Тема на часа от дневник ДПЛР.'

exec school_books.spDescColumn N'TopicDplr', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'TopicDplr', N'TopicDplrId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'TopicDplr', N'ClassBookId'               , N'Идентификатор на дневник.'
exec school_books.spDescColumn N'TopicDplr', N'Date'                      , N'Дата на темата на часа.'
exec school_books.spDescColumn N'TopicDplr', N'Day'                       , N'Ден за който е часа. Число от 1 (Понеделник) до 7 (Неделя).'
exec school_books.spDescColumn N'TopicDplr', N'HourNumber'                , N'Номер на часа. Число >= 0.'
exec school_books.spDescColumn N'TopicDplr', N'StartTime'                 , N'Начален час.'
exec school_books.spDescColumn N'TopicDplr', N'EndTime'                   , N'Краен час.'
exec school_books.spDescColumn N'TopicDplr', N'CurriculumId'              , N'Идентификатор на предмет от учебния план.'
exec school_books.spDescColumn N'TopicDplr', N'Title'                     , N'Тема.'
exec school_books.spDescColumn N'TopicDplr', N'Location'                  , N'Място на провеждане.'
exec school_books.spDescColumn N'TopicDplr', N'IsVerified'                , N'Проверен.'
exec school_books.spDescColumn N'TopicDplr', N'VerifyDate'                , N'Дата на проверка на записа.'
exec school_books.spDescColumn N'TopicDplr', N'VerifiedBySysUserId'       , N'Проверен от.'

exec school_books.spDescColumn N'TopicDplr', N'CreateDate'                , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'TopicDplr', N'CreatedBySysUserId'        , N'Създадено от.'
exec school_books.spDescColumn N'TopicDplr', N'Version'                   , N'Версия.'
