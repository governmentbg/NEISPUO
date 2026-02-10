PRINT 'Create StateExamsAdmProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'StateExamsAdmProtocol'
GO

CREATE TABLE [school_books].[StateExamsAdmProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [StateExamsAdmProtocolId]           INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
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

    CONSTRAINT [PK_StateExamsAdmProtocol] PRIMARY KEY ([SchoolYear], [StateExamsAdmProtocolId]),

    -- external references
    CONSTRAINT [FK_StateExamsAdmProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_StateExamsAdmProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_StateExamsAdmProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_StateExamsAdmProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_StateExamsAdmProtocol_SchoolYear_InstId_StateExamsAdmProtocolId] UNIQUE ([SchoolYear], [InstId], [StateExamsAdmProtocolId]),
);
GO

exec school_books.spDescTable  N'StateExamsAdmProtocol', N'Протокол за допускане до държавни изпити.'

exec school_books.spDescColumn N'StateExamsAdmProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'StateExamsAdmProtocolId'                , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'StateExamsAdmProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'ProtocolNum'                            , N'Номер на протокола.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'CommissionNominationOrderNumber'        , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'CommissionNominationOrderDate'          , N'Дата на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'ExamSession'                            , N'Сесия.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'DirectorPersonId'                       , N'Идентификатор на директора.'

exec school_books.spDescColumn N'StateExamsAdmProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'StateExamsAdmProtocol', N'Version'                                , N'Версия.'
