PRINT 'Create SkillsCheckExamDutyProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'SkillsCheckExamDutyProtocol'
GO

CREATE TABLE [school_books].[SkillsCheckExamDutyProtocol] (
    [SchoolYear]                       SMALLINT         NOT NULL,
    [SkillsCheckExamDutyProtocolId]    INT              NOT NULL,

    [InstId]                     INT              NOT NULL,
    [ProtocolNumber]             NVARCHAR(100)    NULL,
    [ProtocolDate]               DATE             NULL,
    [SubjectId]                  INT              NOT NULL,
    [SubjectTypeId]              INT              NOT NULL,
    [Date]                       DATE             NULL,
    [DirectorPersonId]           INT              NOT NULL,
    [StudentsCapacity]           INT              NOT NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SkillsCheckExamDutyProtocol] PRIMARY KEY ([SchoolYear], [SkillsCheckExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SkillsCheckExamDutyProtocol_SchoolYear_InstId_SkillsCheckExamDutyProtocolId] UNIQUE ([SchoolYear], [InstId], [SkillsCheckExamDutyProtocolId])
);
GO

exec school_books.spDescTable  N'SkillsCheckExamDutyProtocol', N'Протокол за дежурство при провеждане на изпит за проверка на способностите.'

exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'SkillsCheckExamDutyProtocolId'          , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'SubjectId'                              , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'SubjectTypeId'                          , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'Date'                                   , N'Дата.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'DirectorPersonId'                       , N'Идентификатор на директора.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'StudentsCapacity'                       , N'Брой ученици.'

exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocol', N'Version'                                , N'Версия.'
