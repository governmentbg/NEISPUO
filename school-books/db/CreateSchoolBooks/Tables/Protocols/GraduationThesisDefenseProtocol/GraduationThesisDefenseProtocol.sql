PRINT 'Create GraduationThesisDefenseProtocol table'
GO

EXEC [school_books].[spCreateIdSequence] N'GraduationThesisDefenseProtocol'
GO

CREATE TABLE [school_books].[GraduationThesisDefenseProtocol] (
    [SchoolYear]                        SMALLINT         NOT NULL,
    [GraduationThesisDefenseProtocolId] INT              NOT NULL,

    [InstId]                            INT              NOT NULL,
    [ProtocolNumber]                    NVARCHAR(100)    NULL,
    [ProtocolDate]                      DATE             NULL,
    [SessionType]                       NVARCHAR(100)    NULL,
    [EduFormId]                         INT              NULL,
    [CommissionMeetingDate]             DATE             NOT NULL,
    [DirectorOrderNumber]               NVARCHAR(100)    NOT NULL,
    [DirectorOrderDate]                 DATE             NOT NULL,
    [DirectorPersonId]                  INT              NOT NULL,
    [Section1StudentsCapacity]          INT              NOT NULL,
    [Section2StudentsCapacity]          INT              NOT NULL,
    [Section3StudentsCapacity]          INT              NOT NULL,
    [Section4StudentsCapacity]          INT              NOT NULL,

    [CreateDate]                        DATETIME2        NOT NULL,
    [CreatedBySysUserId]                INT              NOT NULL,
    [ModifyDate]                        DATETIME2        NOT NULL,
    [ModifiedBySysUserId]               INT              NOT NULL,
    [Version]                           ROWVERSION       NOT NULL,

    CONSTRAINT [PK_GraduationThesisDefenseProtocol] PRIMARY KEY ([SchoolYear], [GraduationThesisDefenseProtocolId]),

    -- external references
    CONSTRAINT [FK_GraduationThesisDefenseProtocol_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_GraduationThesisDefense_EduForm] FOREIGN KEY ([EduFormId]) REFERENCES [inst_nom].[EduForm] ([ClassEduFormID]),
    CONSTRAINT [FK_GraduationThesisDefenseProtocol_DirectorPersonId] FOREIGN KEY ([DirectorPersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_GraduationThesisDefenseProtocol_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_GraduationThesisDefenseProtocol_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_GraduationThesisDefenseProtocol_SchoolYear_InstId_GraduationThesisDefenseProtocolId] UNIQUE ([SchoolYear], [InstId], [GraduationThesisDefenseProtocolId]),
);
GO

exec school_books.spDescTable  N'GraduationThesisDefenseProtocol', N'Протокол на комисията за оценяване на изпит чрез защитa на дипломен проект - част по теория на професията.'

exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'SchoolYear'                             , N'Учебна година.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'GraduationThesisDefenseProtocolId'      , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'InstId'                                 , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'ProtocolNumber'                         , N'Номер на протокола.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'ProtocolDate'                           , N'Дата на протокола.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'SessionType'                            , N'Сесия.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'EduFormId'                              , N'Форма на обучение. Номенклатура inst_nom.EduForm.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'CommissionMeetingDate'                  , N'Дата на събиране на комисията.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'DirectorOrderNumber'                    , N'Номер на заповедта на директора.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'DirectorOrderDate'                      , N'Дата на заповедта на директора.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'DirectorPersonId'                       , N'Идентификатор на директора.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'Section1StudentsCapacity'               , N'Брой ученици от първа секция.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'Section2StudentsCapacity'               , N'Брой ученици от втора секция.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'Section3StudentsCapacity'               , N'Брой ученици от трата секция.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'Section4StudentsCapacity'               , N'Брой ученици от четвърта секция.'

exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'CreateDate'                             , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'CreatedBySysUserId'                     , N'Създадено от.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'ModifyDate'                             , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'ModifiedBySysUserId'                    , N'Последна модификация от.'
exec school_books.spDescColumn N'GraduationThesisDefenseProtocol', N'Version'                                , N'Версия.'
