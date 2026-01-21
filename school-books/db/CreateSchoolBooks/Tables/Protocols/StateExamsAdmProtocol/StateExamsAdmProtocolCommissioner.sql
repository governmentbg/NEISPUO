PRINT 'Create StateExamsAdmProtocolCommissioner table'
GO

CREATE TABLE [school_books].[StateExamsAdmProtocolCommissioner] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [StateExamsAdmProtocolId]           INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    [IsChairman]                        BIT              NOT NULL,
    [OrderNum]                          SMALLINT         NOT NULL,

    CONSTRAINT [PK_StateExamsAdmProtocolCommissioner] PRIMARY KEY ([SchoolYear], [StateExamsAdmProtocolId], [PersonId]),
    CONSTRAINT [FK_StateExamsAdmProtocolCommissioner_StateExamsAdmProtocol] FOREIGN KEY ([SchoolYear], [StateExamsAdmProtocolId])
        REFERENCES [school_books].[StateExamsAdmProtocol] ([SchoolYear], [StateExamsAdmProtocolId]),
    -- external references
    CONSTRAINT [FK_StateExamsAdmProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'StateExamsAdmProtocolCommissioner', N'Протокол за допускане до държавни изпити - член на комисията.'

exec school_books.spDescColumn N'StateExamsAdmProtocolCommissioner', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'StateExamsAdmProtocolCommissioner', N'StateExamsAdmProtocolId'               , N'Идентификатор на протокол за допускане до държавни изпити.'
exec school_books.spDescColumn N'StateExamsAdmProtocolCommissioner', N'PersonId'                              , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'StateExamsAdmProtocolCommissioner', N'IsChairman'                            , N'Председател – Да/Не.'
exec school_books.spDescColumn N'StateExamsAdmProtocolCommissioner', N'OrderNum'                              , N'Пореден номер.'
