PRINT 'Create SkillsCheckExamDutyProtocolSupervisor table'
GO

CREATE TABLE [school_books].[SkillsCheckExamDutyProtocolSupervisor] (
    [SchoolYear]                    SMALLINT         NOT NULL,
    [SkillsCheckExamDutyProtocolId] INT              NOT NULL,
    [PersonId]                      INT              NOT NULL,

    CONSTRAINT [PK_SkillsCheckExamDutyProtocolSupervisor] PRIMARY KEY ([SchoolYear], [SkillsCheckExamDutyProtocolId], [PersonId]),
    CONSTRAINT [FK_SkillsCheckExamDutyProtocolSupervisor_SkillsCheckExamDutyProtocol] FOREIGN KEY ([SchoolYear], [SkillsCheckExamDutyProtocolId]) REFERENCES [school_books].[SkillsCheckExamDutyProtocol] ([SchoolYear], [SkillsCheckExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_SkillsCheckExamDutyProtocolSupervisor_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'SkillsCheckExamDutyProtocolSupervisor', N'Протокол за дежурство при провеждане на изпит за проверка на способностите - квестор.'

exec school_books.spDescColumn N'SkillsCheckExamDutyProtocolSupervisor', N'SchoolYear'                          , N'Учебна година.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocolSupervisor', N'SkillsCheckExamDutyProtocolId'       , N'Идентификатор на протокол за дежурство при провеждане на изпит за проверка на способностите.'
exec school_books.spDescColumn N'SkillsCheckExamDutyProtocolSupervisor', N'PersonId'                            , N'Идентификатор на квестор.'
