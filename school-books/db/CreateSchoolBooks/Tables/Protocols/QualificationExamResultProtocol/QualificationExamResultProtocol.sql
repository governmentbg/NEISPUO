PRINT 'Create QualificationExamResultProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'QualificationExamResultProtocol'
GO

CREATE TABLE [school_books].[QualificationExamResultProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [QualificationExamResultProtocolId] INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [QualificationExamTypeId]           INT              NOT NULL,
    [GroupNum]                          NVARCHAR(100)    NULL,
    [Profession]                        NVARCHAR(100)    NOT NULL,
    [Speciality]                        NVARCHAR(100)    NOT NULL,
    [QualificationDegreeId]             INT              NOT NULL,
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

    CONSTRAINT [PK_QualificationExamResultProtocol] PRIMARY KEY ([SchoolYear], [QualificationExamResultProtocolId]),

    CONSTRAINT [FK_QualificationExamResultProtocol_QualificationDegree] FOREIGN KEY ([QualificationDegreeId]) REFERENCES [school_books].[QualificationDegree] ([Id]),
    CONSTRAINT [FK_QualificationExamResultProtocol_QualificationExamType] FOREIGN KEY ([QualificationExamTypeId]) REFERENCES [school_books].[QualificationExamType] ([Id]),

    -- external references
    CONSTRAINT [FK_QualificationExamResultProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_QualificationExamResultProtocol_EduForm] FOREIGN KEY ([EduFormId]) REFERENCES [inst_nom].[EduForm] ([ClassEduFormID]),
    CONSTRAINT [FK_QualificationExamResultProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_QualificationExamResultProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_QualificationExamResultProtocol_SchoolYear_InstId_QualificationExamResultProtocolId] UNIQUE ([SchoolYear], [InstId], [QualificationExamResultProtocolId])
);
GO

exec school_books.spDescTable  N'QualificationExamResultProtocol', N'Протокол за резултата от изпит за професионална квалификация.'

exec school_books.spDescColumn N'QualificationExamResultProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'QualificationExamResultProtocolId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'QualificationExamResultProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'QualificationExamTypeId'                , N'Вид изпит. Номенклатура QualificationExamType.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'GroupNum'                               , N'Номер на група.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'Profession'                             , N'Професия.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'Speciality'                             , N'Специалност.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'QualificationDegreeId'                  , N'Степен на професионална квалификация. Номенклатура QualificationDegree.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'EduFormId'                              , N'Форма на обучение. Номенклатура inst_nom.EduForm.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'SessionType'                            , N'Сесия.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'Date'                                   , N'Дата.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'CommissionNominationOrderNumber'        , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'CommissionNominationOrderDate'          , N'Дата на заповед за назначаване на комисията.'

exec school_books.spDescColumn N'QualificationExamResultProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'QualificationExamResultProtocol', N'Version'                                , N'Версия.'
