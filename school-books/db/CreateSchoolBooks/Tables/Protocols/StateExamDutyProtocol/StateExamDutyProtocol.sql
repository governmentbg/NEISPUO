PRINT 'Create StateExamDutyProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'StateExamDutyProtocol'
GO

CREATE TABLE [school_books].[StateExamDutyProtocol] (
    [SchoolYear]                 SMALLINT         NOT NULL,
    [StateExamDutyProtocolId]    INT              NOT NULL,

    [InstId]                     INT              NOT NULL,
    [ProtocolNumber]             NVARCHAR(100)    NULL,
    [ProtocolDate]               DATE             NULL,
    [SubjectId]                  INT              NOT NULL,
    [SubjectTypeId]              INT              NOT NULL,
    [EduFormId]                  INT              NULL,
    [SessionType]                NVARCHAR(100)    NULL,
    [Date]                       DATE             NOT NULL,
    [OrderNumber]                NVARCHAR(100)    NOT NULL,
    [OrderDate]                  DATE             NOT NULL,
    [ModulesCount]               INT              NOT NULL,
    [RoomNumber]                 NVARCHAR(100)    NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_StateExamDutyProtocol] PRIMARY KEY ([SchoolYear], [StateExamDutyProtocolId]),

    -- external references
    CONSTRAINT [FK_StateExamDutyProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_StateExamDutyProtocol_Subject] FOREIGN KEY ([SubjectId]) REFERENCES [inst_nom].[Subject] ([SubjectID]),
    CONSTRAINT [FK_StateExamDutyProtocol_SubjectType] FOREIGN KEY ([SubjectTypeId]) REFERENCES [inst_nom].[SubjectType] ([SubjectTypeID]),
    CONSTRAINT [FK_StateExamDutyProtocol_EduForm] FOREIGN KEY ([EduFormId]) REFERENCES [inst_nom].[EduForm] ([ClassEduFormID]),
    CONSTRAINT [FK_StateExamDutyProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_StateExamDutyProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_StateExamDutyProtocol_SchoolYear_InstId_StateExamDutyProtocolId] UNIQUE ([SchoolYear], [InstId], [StateExamDutyProtocolId])
);
GO

exec school_books.spDescTable  N'StateExamDutyProtocol', N'Протокол за дежурство при провеждане на писмен ДЗИ.'

exec school_books.spDescColumn N'StateExamDutyProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'StateExamDutyProtocolId'                , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'StateExamDutyProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'SubjectId'                              , N'Уч. предмет. Номенклатура inst_nom.Subject.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'SubjectTypeId'                          , N'Начин на изучаване. Номенклатура inst_nom.SubjectType.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'EduFormId'                              , N'Форма на обучение. Номенклатура inst_nom.EduForm.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'SessionType'                            , N'Сесия.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'Date'                                   , N'Дата на протокола.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'OrderNumber'                            , N'Номер на заповед.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'OrderDate'                              , N'Дата на заповед.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'ModulesCount'                           , N'Брой модули.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'RoomNumber'                             , N'Зала.'

exec school_books.spDescColumn N'StateExamDutyProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'StateExamDutyProtocol', N'Version'                                , N'Версия.'
