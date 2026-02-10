PRINT 'Create HighSchoolCertificateProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'HighSchoolCertificateProtocol'
GO

CREATE TABLE [school_books].[HighSchoolCertificateProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [HighSchoolCertificateProtocolId]   INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [Stage]                             INT              NOT NULL,
    [ProtocolNum]                       NVARCHAR(100)    NULL,
    [ProtocolDate]                      DATE             NULL,
    [CommissionMeetingDate]             DATE             NOT NULL,
    [CommissionNominationOrderNumber]   NVARCHAR(100)    NOT NULL,
    [CommissionNominationOrderDate]     DATE             NOT NULL,
    [ExamSession]                       NVARCHAR(100)    NULL,
    [DirectorPersonId]                  INT              NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_HighSchoolCertificateProtocol] PRIMARY KEY ([SchoolYear], [HighSchoolCertificateProtocolId]),
    CONSTRAINT [CHK_HighSchoolCertificateProtocol_Stage] CHECK ([Stage] IN (1, 2)),

    -- external references
    CONSTRAINT [FK_HighSchoolCertificateProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_HighSchoolCertificateProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_HighSchoolCertificateProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_HighSchoolCertificateProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_HighSchoolCertificateProtocol_SchoolYear_InstId_HighSchoolCertificateProtocolId] UNIQUE ([SchoolYear], [InstId], [HighSchoolCertificateProtocolId]),
);
GO

exec school_books.spDescTable  N'HighSchoolCertificateProtocol', N'Протокол за удостоверяване на завършен гимназиален етап.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'SchoolYear'                           , N'Учебна година.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'HighSchoolCertificateProtocolId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'InstId'                               , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'Stage'                                , N'Гимназиален етап. 1 - Първи, 2 - Втори.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'ProtocolNum'                          , N'Номер на протокола.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'ProtocolDate'                         , N'Дата на протокола.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'CommissionMeetingDate'                , N'Дата на заседанието на комисията.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'CommissionNominationOrderNumber'      , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'CommissionNominationOrderDate'        , N'Дата на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'ExamSession'                          , N'Сесия.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'DirectorPersonId'                     , N'Идентификатор на директора.'

exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'CreateDate'                           , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'CreatedBySysUserId'                   , N'Създадено от.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'ModifyDate'                           , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'ModifiedBySysUserId'                  , N'Последна модификация от.'
exec school_books.spDescColumn N'HighSchoolCertificateProtocol', N'Version'                              , N'Версия.'
