PRINT 'Create SkillsCheckExamResultProtocolEvaluator table'
GO

EXEC [school_books].[spCreateIdSequence] N'SkillsCheckExamResultProtocolEvaluator'
GO

CREATE TABLE [school_books].[SkillsCheckExamResultProtocolEvaluator] (
    [SchoolYear]                                SMALLINT         NOT NULL,
    [SkillsCheckExamResultProtocolId]           INT              NOT NULL,
    [SkillsCheckExamResultProtocolEvaluatorId]  INT              NOT NULL,

    [Name]                                      NVARCHAR(100)    NOT NULL,

    CONSTRAINT [PK_SkillsCheckExamResultProtocolEvaluator] PRIMARY KEY ([SchoolYear], [SkillsCheckExamResultProtocolId], [SkillsCheckExamResultProtocolEvaluatorId]),
    CONSTRAINT [FK_SkillsCheckExamResultProtocolEvaluator_SkillsCheckExamResultProtocol] FOREIGN KEY ([SchoolYear], [SkillsCheckExamResultProtocolId]) REFERENCES [school_books].[SkillsCheckExamResultProtocol] ([SchoolYear], [SkillsCheckExamResultProtocolId])
);
GO

exec school_books.spDescTable  N'SkillsCheckExamResultProtocolEvaluator', N'Протокол за резултатите от изпита за проверка на способностите - оценител.'

exec school_books.spDescColumn N'SkillsCheckExamResultProtocolEvaluator', N'SchoolYear'                                     , N'Учебна година.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocolEvaluator', N'SkillsCheckExamResultProtocolId'                , N'Идентификатор на протокол за резултатите от изпита за проверка на способностите.'
exec school_books.spDescColumn N'SkillsCheckExamResultProtocolEvaluator', N'SkillsCheckExamResultProtocolEvaluatorId'       , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SkillsCheckExamResultProtocolEvaluator', N'Name'                                           , N'Имена.'
