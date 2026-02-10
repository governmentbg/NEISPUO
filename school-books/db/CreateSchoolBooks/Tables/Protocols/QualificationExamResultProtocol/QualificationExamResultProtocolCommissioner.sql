PRINT 'Create QualificationExamResultProtocolCommissioner table'
GO

CREATE TABLE [school_books].[QualificationExamResultProtocolCommissioner] (
    [SchoolYear]                                    SMALLINT         NOT NULL,
    [QualificationExamResultProtocolId]             INT              NOT NULL,
    [PersonId]                                      INT              NOT NULL,

    [IsChairman]                                    BIT              NOT NULL,
    [OrderNum]                                      SMALLINT         NOT NULL,

    CONSTRAINT [PK_QualificationExamResultProtocolCommissioner] PRIMARY KEY ([SchoolYear], [QualificationExamResultProtocolId], [PersonId]),
    CONSTRAINT [FK_QualificationExamResultProtocolCommissioner_QualificationExamResultProtocol] FOREIGN KEY ([SchoolYear], [QualificationExamResultProtocolId])
        REFERENCES [school_books].[QualificationExamResultProtocol] ([SchoolYear], [QualificationExamResultProtocolId]),
    -- external references
    CONSTRAINT [FK_QualificationExamResultProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'QualificationExamResultProtocolCommissioner', N'Протокол за резултата от изпит за професионална квалификация - член на комисията.'

exec school_books.spDescColumn N'QualificationExamResultProtocolCommissioner', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'QualificationExamResultProtocolCommissioner', N'QualificationExamResultProtocolId'     , N'Идентификатор на протокол за резултата от изпит за професионална квалификация.'
exec school_books.spDescColumn N'QualificationExamResultProtocolCommissioner', N'PersonId'                              , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'QualificationExamResultProtocolCommissioner', N'IsChairman'                            , N'Председател – Да/Не.'
exec school_books.spDescColumn N'QualificationExamResultProtocolCommissioner', N'OrderNum'                              , N'Пореден номер.'
