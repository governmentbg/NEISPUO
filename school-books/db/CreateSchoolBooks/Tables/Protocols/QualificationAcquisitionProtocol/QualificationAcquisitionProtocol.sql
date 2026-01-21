PRINT 'Create QualificationAcquisitionProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'QualificationAcquisitionProtocol'
GO

CREATE TABLE [school_books].[QualificationAcquisitionProtocol] (
    [SchoolYear]                             SMALLINT         NOT NULL,
    [QualificationAcquisitionProtocolId]     INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [ProtocolType]                      INT              NOT NULL,
    [Profession]                        NVARCHAR(100)    NOT NULL,
    [Speciality]                        NVARCHAR(100)    NOT NULL,
    [QualificationDegreeId]             INT              NOT NULL,
    [ProtocolNumber]                    NVARCHAR(100)    NULL,
    [ProtocolDate]                      DATE             NULL,
    [Date]                              DATE             NOT NULL,
    [CommissionNominationOrderNumber]   NVARCHAR(100)    NOT NULL,
    [CommissionNominationOrderDate]     DATE             NOT NULL,
    [DirectorPersonId]                  INT              NOT NULL,

    [CreateDate]                 DATETIME2        NOT NULL,
    [CreatedBySysUserId]         INT              NOT NULL,
    [ModifyDate]                 DATETIME2        NOT NULL,
    [ModifiedBySysUserId]        INT              NOT NULL,
    [Version]                    ROWVERSION       NOT NULL,

    CONSTRAINT [PK_QualificationAcquisitionProtocol] PRIMARY KEY ([SchoolYear], [QualificationAcquisitionProtocolId]),

    CONSTRAINT [FK_QualificationAcquisitionProtocol_QualificationDegree] FOREIGN KEY ([QualificationDegreeId]) REFERENCES [school_books].[QualificationDegree] ([Id]),

    -- external references
    CONSTRAINT [FK_QualificationAcquisitionProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_QualificationAcquisitionProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_QualificationAcquisitionProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_QualificationAcquisitionProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_QualificationAcquisitionProtocol_SchoolYear_InstId_QualificationAcquisitionProtocolId] UNIQUE ([SchoolYear], [InstId], [QualificationAcquisitionProtocolId])
);
GO

exec school_books.spDescTable  N'QualificationAcquisitionProtocol', N'Протокол за придобиване на професионална квалификация.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'QualificationAcquisitionProtocolId'     , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'ProtocolType'                           , N'Вид на протокола. 1 - за придобиване на професионална квалификация, 2 - за оценките от изпит за придобиване на професионална квалификация по част от професия, 3 - за оценките от държавен изпит за придобиване на степен на професионална квалификация.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'Profession'                             , N'Професия.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'Speciality'                             , N'Специалност.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'QualificationDegreeId'                  , N'Степен на професионална квалификация. Номенклатура QualificationDegree.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'Date'                                   , N'Дата.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'CommissionNominationOrderNumber'        , N'Номер на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'CommissionNominationOrderDate'          , N'Дата на заповед за назначаване на комисията.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'DirectorPersonId'                       , N'Идентификатор на директора.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocol', N'Version'                                , N'Версия.'
