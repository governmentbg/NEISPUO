PRINT 'Create HighSchoolCertificateProtocolStudent table'
GO

CREATE TABLE [school_books].[HighSchoolCertificateProtocolStudent] (
    [SchoolYear]                         SMALLINT         NOT NULL,
    [HighSchoolCertificateProtocolId]    INT              NOT NULL,
    [ClassId]                            INT              NOT NULL,
    [PersonId]                           INT              NOT NULL,

    CONSTRAINT [PK_HighSchoolCertificateProtocolStudent] PRIMARY KEY ([SchoolYear], [HighSchoolCertificateProtocolId], [ClassId], [PersonId]),
    CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_HighSchoolCertificateProtocol] FOREIGN KEY ([SchoolYear], [HighSchoolCertificateProtocolId]) REFERENCES [school_books].[HighSchoolCertificateProtocol] ([SchoolYear], [HighSchoolCertificateProtocolId]),

    -- external references
    CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_HighSchoolCertificateProtocolStudent_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_HighSchoolCertificateProtocolStudent_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'HighSchoolCertificateProtocolStudent', N'Протокол за удостоверяване на завършен гимназиален етап - ученик.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocolStudent', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolStudent', N'HighSchoolCertificateProtocolId'    , N'Идентификатор на протокол за удостоверяване на завършен гимназиален етап.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolStudent', N'ClassId'                            , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocolStudent', N'PersonId'                           , N'Идентификатор на ученик.'
