PRINT 'Create ExamResultProtocolCommissioner table'
GO

CREATE TABLE [school_books].[ExamResultProtocolCommissioner] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [ExamResultProtocolId]              INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    [IsChairman]                        BIT              NOT NULL,
    [OrderNum]                          SMALLINT         NOT NULL,

    CONSTRAINT [PK_ExamResultProtocolCommissioner] PRIMARY KEY ([SchoolYear], [ExamResultProtocolId], [PersonId]),
    CONSTRAINT [FK_ExamResultProtocolCommissioner_ExamResultProtocol] FOREIGN KEY ([SchoolYear], [ExamResultProtocolId])
        REFERENCES [school_books].[ExamResultProtocol] ([SchoolYear], [ExamResultProtocolId]),
    -- external references
    CONSTRAINT [FK_ExamResultProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'ExamResultProtocolCommissioner', N'Протокол за резултат от изпит - член на комисията.'

exec school_books.spDescColumn N'ExamResultProtocolCommissioner', N'SchoolYear'                , N'Учебна година.'
exec school_books.spDescColumn N'ExamResultProtocolCommissioner', N'ExamResultProtocolId'      , N'Идентификатор на протокол за резултат от изпит.'
exec school_books.spDescColumn N'ExamResultProtocolCommissioner', N'PersonId'                  , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'ExamResultProtocolCommissioner', N'IsChairman'                , N'Председател – Да/Не.'
exec school_books.spDescColumn N'ExamResultProtocolCommissioner', N'OrderNum'                  , N'Пореден номер.'
