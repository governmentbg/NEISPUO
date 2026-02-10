PRINT 'Create SkillsCheckExamResultProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'SkillsCheckExamResultProtocol'
GO

CREATE TABLE [school_books].[SkillsCheckExamResultProtocol] (
    [SchoolYear]                       SMALLINT         NOT NULL,
    [SkillsCheckExamResultProtocolId]  INT              NOT NULL,

    [InstId]                     INT              NOT NULL,
    [ProtocolNumber]             NVARCHAR(100)    NULL,
    [SubjectId]                  INT              NOT NULL,
    [Date]                       DATE             NULL,
    [StudentsCapacity]           INT              NOT NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SkillsCheckExamResultProtocol] PRIMARY KEY ([SchoolYear], [SkillsCheckExamResultProtocolId]),

    -- external references
    CONSTRAINT [FK_SkillsCheckExamResultProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SkillsCheckExamResultProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_SkillsCheckExamResultProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SkillsCheckExamResultProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SkillsCheckExamResultProtocol_SchoolYear_InstId_SkillsCheckExamResultProtocolId] UNIQUE ([SchoolYear], [InstId], [SkillsCheckExamResultProtocolId])
);
GO

exec school_books.spDescTable  N'SkillsCheckExamResultProtocol', N'Протокол за резултатите от изпита за проверка на способностите.'

exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'SkillsCheckExamResultProtocolId'        , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'SubjectId'                              , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'Date'                                   , N'Дата.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'StudentsCapacity'                       , N'Брой ученици.'

exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocol', N'Version'                                , N'Версия.'
