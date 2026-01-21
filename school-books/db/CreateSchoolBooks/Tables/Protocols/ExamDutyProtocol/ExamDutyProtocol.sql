PRINT 'Create ExamDutyProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'ExamDutyProtocol'
GO

CREATE TABLE [school_books].[ExamDutyProtocol] (
    [SchoolYear]                 SMALLINT         NOT NULL,
    [ExamDutyProtocolId]         INT              NOT NULL,

    [InstId]                     INT              NOT NULL,
    [ProtocolNumber]             NVARCHAR(100)    NULL,
    [ProtocolDate]               DATE             NULL,
    [SubjectId]                  INT              NOT NULL,
    [SubjectTypeId]              INT              NOT NULL,
    [ProtocolExamTypeId]         INT              NOT NULL,
    [ProtocolExamSubTypeId]      INT              NOT NULL,
    [GroupNum]                   NVARCHAR(100)    NULL,
    [EduFormId]                  INT              NULL,
    [SessionType]                NVARCHAR(100)    NULL,
    [Date]                       DATE             NOT NULL,
    [OrderNumber]                NVARCHAR(100)    NOT NULL,
    [OrderDate]                  DATE             NOT NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ExamDutyProtocol] PRIMARY KEY ([SchoolYear], [ExamDutyProtocolId]),
    CONSTRAINT [FK_ExamDutyProtocol_ProtocolExamType] FOREIGN KEY ([ProtocolExamTypeId]) REFERENCES [school_books].[ProtocolExamType] ([Id]),
    CONSTRAINT [FK_ExamDutyProtocol_ProtocolExamSubType] FOREIGN KEY ([ProtocolExamSubTypeId]) REFERENCES [school_books].[ProtocolExamSubType] ([Id]),

    -- external references
    CONSTRAINT [FK_ExamDutyProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ExamDutyProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_ExamDutyProtocol_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_ExamDutyProtocol_EduForm] FOREIGN KEY ([EduFormId]) REFERENCES [inst_nom].[EduForm] ([ClassEduFormID]),
    CONSTRAINT [FK_ExamDutyProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ExamDutyProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ExamDutyProtocol_SchoolYear_InstId_ExamDutyProtocolId] UNIQUE ([SchoolYear], [InstId], [ExamDutyProtocolId])
);
GO

exec school_books.spDescTable  N'ExamDutyProtocol', N'Протокол за дежурство при провеждане на изпит.'

exec school_books.spDescColumn N'ExamDutyProtocol', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ExamDutyProtocolId'             , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ExamDutyProtocol', N'InstId'                         , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ProtocolNumber'                 , N'Номер на протокола.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ProtocolDate'                   , N'Дата на протокола.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'SubjectId'                      , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'SubjectTypeId'                  , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ProtocolExamTypeId'             , N'Вид на изпита. Номенклатура ProtocolExamType.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ProtocolExamSubTypeId'          , N'Подвид на изпита. Номенклатура ProtocolExamSubType.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'GroupNum'                       , N'Номер на група.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'EduFormId'                      , N'Форма на обучение. Номенклатура inst_nom.EduForm.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'SessionType'                    , N'Сесия.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'Date'                           , N'Дата.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'OrderNumber'                    , N'Номер на заповед'
exec school_books.spDescColumn N'ExamDutyProtocol', N'OrderDate'                      , N'Дата на заповед'

exec school_books.spDescColumn N'ExamDutyProtocol', N'CreateDate'                     , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'CreatedBySysUserId'             , N'Създадено от.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ModifyDate'                     , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'ModifiedBySysUserId'            , N'Последна модификация от.'
exec school_books.spDescColumn N'ExamDutyProtocol', N'Version'                        , N'Версия.'
