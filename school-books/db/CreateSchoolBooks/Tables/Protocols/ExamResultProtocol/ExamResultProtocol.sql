PRINT 'Create ExamResultProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'ExamResultProtocol'
GO

CREATE TABLE [school_books].[ExamResultProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [ExamResultProtocolId]              INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [SubjectId]                         INT              NOT NULL,
    [SubjectTypeId]                     INT              NOT NULL,
    [ProtocolExamTypeId]                INT              NOT NULL,
    [ProtocolExamSubTypeId]             INT              NOT NULL,
    [GroupNum]                          NVARCHAR(100)    NULL,
    [EduFormId]                         INT              NULL,
    [SessionType]                       NVARCHAR(100)    NULL,
    [ProtocolNumber]                    NVARCHAR(100)    NULL,
    [ProtocolDate]                      DATE             NULL,
    [Date]                              DATE             NOT NULL,
    [CommissionNominationOrderNumber]   NVARCHAR(100)    NOT NULL,
    [CommissionNominationOrderDate]     DATE             NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_ExamResultProtocol] PRIMARY KEY ([SchoolYear], [ExamResultProtocolId]),
    CONSTRAINT [FK_ExamResultProtocol_ProtocolExamType] FOREIGN KEY ([ProtocolExamTypeId]) REFERENCES [school_books].[ProtocolExamType] ([Id]),
    CONSTRAINT [FK_ExamResultProtocol_ProtocolExamSubType] FOREIGN KEY ([ProtocolExamSubTypeId]) REFERENCES [school_books].[ProtocolExamSubType] ([Id]),

    -- external references
    CONSTRAINT [FK_ExamResultProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_ExamResultProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_ExamResultProtocol_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_ExamResultProtocol_EduForm] FOREIGN KEY ([EduFormId]) REFERENCES [inst_nom].[EduForm] ([ClassEduFormID]),
    CONSTRAINT [FK_ExamResultProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_ExamResultProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_ExamResultProtocol_SchoolYear_InstId_ExamResultProtocolId] UNIQUE ([SchoolYear], [InstId], [ExamResultProtocolId])
);
GO

exec school_books.spDescTable  N'ExamResultProtocol', N'Протокол за резултат от изпит.'

exec school_books.spDescColumn N'ExamResultProtocol', N'SchoolYear'                         , N'Учебна година.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ExamResultProtocolId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'ExamResultProtocol', N'InstId'                             , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'ExamResultProtocol', N'SubjectId'                          , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'ExamResultProtocol', N'SubjectTypeId'                      , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ProtocolExamTypeId'                 , N'Вид на изпита. Номенклатура ProtocolExamType.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ProtocolExamSubTypeId'              , N'Подвид на изпита. Номенклатура ProtocolExamSubType.'
exec school_books.spDescColumn N'ExamResultProtocol', N'GroupNum'                           , N'Номер на група.'
exec school_books.spDescColumn N'ExamResultProtocol', N'EduFormId'                          , N'Форма на обучение. Номенклатура inst_nom.EduForm.'
exec school_books.spDescColumn N'ExamResultProtocol', N'SessionType'                        , N'Сесия.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ProtocolNumber'                     , N'Номер на протокола.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ProtocolDate'                       , N'Дата на протокола.'
exec school_books.spDescColumn N'ExamResultProtocol', N'Date'                               , N'Дата.'
exec school_books.spDescColumn N'ExamResultProtocol', N'CommissionNominationOrderNumber'    , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'ExamResultProtocol', N'CommissionNominationOrderDate'      , N'Дата на заповед за назначаване на комисията.'

exec school_books.spDescColumn N'ExamResultProtocol', N'CreateDate'                         , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'ExamResultProtocol', N'CreatedBySysUserId'                 , N'Създадено от.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ModifyDate'                         , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'ExamResultProtocol', N'ModifiedBySysUserId'                , N'Последна модификация от.'
exec school_books.spDescColumn N'ExamResultProtocol', N'Version'                            , N'Версия.'
