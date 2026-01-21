PRINT 'Create GradeChangeExamsAdmProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'GradeChangeExamsAdmProtocol'
GO

CREATE TABLE [school_books].[GradeChangeExamsAdmProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GradeChangeExamsAdmProtocolId]     INT              NOT NULL,

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

    CONSTRAINT [PK_GradeChangeExamsAdmProtocol] PRIMARY KEY ([SchoolYear], [GradeChangeExamsAdmProtocolId]),

    -- external references
    CONSTRAINT [FK_GradeChangeExamsAdmProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_GradeChangeExamsAdmProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_GradeChangeExamsAdmProtocol_SchoolYear_InstId_GradeChangeExamsAdmProtocolId] UNIQUE ([SchoolYear], [InstId], [GradeChangeExamsAdmProtocolId]),
);
GO

exec school_books.spDescTable  N'GradeChangeExamsAdmProtocol', N'Протокол за допускане до изпити за промяна на оценката.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'GradeChangeExamsAdmProtocolId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'InstId'                             , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'ProtocolNum'                        , N'Номер на протокола.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'ProtocolDate'                       , N'Дата на протокола.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'CommissionMeetingDate'              , N'Дата на заседанието на комисията.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'CommissionNominationOrderNumber'    , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'CommissionNominationOrderDate'      , N'Дата на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'ExamSession'                        , N'Сесия.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'DirectorPersonId'                   , N'Идентификатор на директора.'

exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'CreateDate'                         , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'CreatedBySysUserId'                 , N'Създадено от.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'ModifyDate'                         , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'ModifiedBySysUserId'                , N'Последна модификация от.'
exec school_books.spDescColumn N'GradeChangeExamsAdmProtocol', N'Version'                            , N'Версия.'
