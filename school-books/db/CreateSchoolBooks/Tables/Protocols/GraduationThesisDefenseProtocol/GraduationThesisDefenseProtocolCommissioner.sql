PRINT 'Create GraduationThesisDefenseProtocolCommissioner table'
GO

CREATE TABLE [school_books].[GraduationThesisDefenseProtocolCommissioner] (
    [SchoolYear]                          SMALLINT         NOT NULL,
    [GraduationThesisDefenseProtocolId]   INT              NOT NULL,
    [PersonId]                            INT              NOT NULL,

    [IsChairman]                          BIT              NOT NULL,
    [OrderNum]                            SMALLINT         NOT NULL,

    CONSTRAINT [PK_GraduationThesisDefenseProtocolCommissioner] PRIMARY KEY ([SchoolYear], [GraduationThesisDefenseProtocolId], [PersonId]),
    CONSTRAINT [FK_GraduationThesisDefenseProtocolCommissioner_GraduationThesisDefenseProtocol] FOREIGN KEY ([SchoolYear], [GraduationThesisDefenseProtocolId])
        REFERENCES [school_books].[GraduationThesisDefenseProtocol] ([SchoolYear], [GraduationThesisDefenseProtocolId]),
    -- external references
    CONSTRAINT [FK_GraduationThesisDefenseProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'GraduationThesisDefenseProtocolCommissioner', N'Протокол на комисията за оценяване на изпит чрез защитa на дипломен проект - част по теория на професията - член на комисията.'

exec school_books.spDescColumn N'GraduationThesisDefenseProtocolCommissioner', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocolCommissioner', N'GraduationThesisDefenseProtocolId'      , N'Идентификатор на протокол на комисията за оценяване на изпит чрез защитa на дипломен проект - част по теория на професията.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocolCommissioner', N'PersonId'                               , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'GraduationThesisDefenseProtocolCommissioner', N'IsChairman'                             , N'Председател – Да/Не.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocolCommissioner', N'OrderNum'                               , N'Пореден номер.'
