PRINT 'Create HighSchoolCertificateProtocolCommissioner table'
GO

CREATE TABLE [school_books].[HighSchoolCertificateProtocolCommissioner] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [HighSchoolCertificateProtocolId]   INT              NOT NULL,
    [PersonId]                          INT              NOT NULL,

    [IsChairman]                        BIT              NOT NULL,
    [OrderNum]                          SMALLINT         NOT NULL,

    CONSTRAINT [PK_HighSchoolCertificateProtocolCommissioner] PRIMARY KEY ([SchoolYear], [HighSchoolCertificateProtocolId], [PersonId]),
    CONSTRAINT [FK_HighSchoolCertificateProtocolCommissioner_HighSchoolCertificateProtocol] FOREIGN KEY ([SchoolYear], [HighSchoolCertificateProtocolId])
        REFERENCES [school_books].[HighSchoolCertificateProtocol] ([SchoolYear], [HighSchoolCertificateProtocolId]),
    -- external references
    CONSTRAINT [FK_HighSchoolCertificateProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'HighSchoolCertificateProtocolCommissioner', N'Протокол за удостоверяване на завършен гимназиален етап - член на комисията.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocolCommissioner', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolCommissioner', N'HighSchoolCertificateProtocolId'    , N'Идентификатор на протокол за удостоверяване на завършен гимназиален етап.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolCommissioner', N'PersonId'                           , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocolCommissioner', N'IsChairman'                         , N'Председател – Да/Не.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolCommissioner', N'OrderNum'                           , N'Пореден номер.'
