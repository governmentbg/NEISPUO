PRINT 'Create NvoExamDutyProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'NvoExamDutyProtocol'
GO

CREATE TABLE [school_books].[NvoExamDutyProtocol] (
    [SchoolYear]                 SMALLINT         NOT NULL,
    [NvoExamDutyProtocolId]      INT              NOT NULL,

    [InstId]                     INT              NOT NULL,
    [ProtocolNumber]             NVARCHAR(100)    NULL,
    [ProtocolDate]               DATE             NULL,
    [BasicClassId]               INT              NOT NULL,
    [SubjectId]                  INT              NOT NULL,
    [SubjectTypeId]              INT              NOT NULL,
    [Date]                       DATE             NOT NULL,
    [RoomNumber]                 NVARCHAR(100)    NULL,
    [DirectorPersonId]           INT              NOT NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_NvoExamDutyProtocol] PRIMARY KEY ([SchoolYear], [NvoExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_NvoExamDutyProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_NvoExamDutyProtocol_BasicClassId] FOREIGN KEY ([BasicClassId]) REFERENCES [inst_nom].[BasicClass] ([BasicClassID]),
    CONSTRAINT [FK_NvoExamDutyProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_NvoExamDutyProtocol_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_NvoExamDutyProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_NvoExamDutyProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_NvoExamDutyProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_NvoExamDutyProtocol_SchoolYear_InstId_NvoExamDutyProtocolId] UNIQUE ([SchoolYear], [InstId], [NvoExamDutyProtocolId])
);
GO

exec school_books.spDescTable  N'NvoExamDutyProtocol', N'Протокол за дежурство при провеждане на писмен изпит от НВО.'

exec school_books.spDescColumn N'NvoExamDutyProtocol', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'NvoExamDutyProtocolId'              , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'NvoExamDutyProtocol', N'InstId'                             , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'ProtocolNumber'                     , N'Номер на протокола.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'ProtocolDate'                       , N'Дата на протокола.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'BasicClassId'                       , N'Идентификатор на випуска. Номенклатура inst_nom.BasicClass.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'SubjectId'                          , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'SubjectTypeId'                      , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'Date'                               , N'Дата на протокола.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'RoomNumber'                         , N'Зала.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'DirectorPersonId'                   , N'Идентификатор на директора.'

exec school_books.spDescColumn N'NvoExamDutyProtocol', N'CreateDate'                         , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'CreatedBySysUserId'                 , N'Създадено от.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'ModifyDate'                         , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'ModifiedBySysUserId'                , N'Последна модификация от.'
exec school_books.spDescColumn N'NvoExamDutyProtocol', N'Version'                            , N'Версия.'
