PRINT 'Create SpbsBookRecord table'
GO

EXEC [school_books].[spCreateIdSequence] N'SpbsBookRecord'
GO

-- Social Pedagogy Boarding School Book Records
CREATE TABLE [school_books].[SpbsBookRecord] (
    [SchoolYear]                    SMALLINT         NOT NULL,
    [SpbsBookRecordId]              INT              NOT NULL,

    [InstId]                        INT              NOT NULL,
    [RecordNumber]                  INT              NOT NULL,
    [ClassId]                       INT              NOT NULL,
    [PersonId]                      INT              NOT NULL,

    [SendingCommission]             NVARCHAR(100)    NULL,
    [SendingCommissionAddress]      NVARCHAR(1000)   NULL,
    [SendingCommissionPhoneNumber]  NVARCHAR(100)    NULL,

    [InspectorNames]                NVARCHAR(100)    NULL,
    [InspectorAddress]              NVARCHAR(1000)   NULL,
    [InspectorPhoneNumber]          NVARCHAR(100)    NULL,

    [CreateDate]                    DATETIME2        NOT NULL,
    [CreatedBySysUserId]            INT              NOT NULL,
    [ModifyDate]                    DATETIME2        NOT NULL,
    [ModifiedBySysUserId]           INT              NOT NULL,
    [Version]                       ROWVERSION       NOT NULL,

    CONSTRAINT [PK_SpbsBookRecord] PRIMARY KEY ([SchoolYear], [SpbsBookRecordId]),

    -- external references
    CONSTRAINT [FK_SpbsBookRecord_InstId_SchoolYear] FOREIGN KEY ([InstId], [SchoolYear]) REFERENCES [core].[InstitutionSchoolYear] ([InstitutionID], [SchoolYear]),
    CONSTRAINT [FK_SpbsBookRecord_ClassGroup] FOREIGN KEY ([ClassId]) REFERENCES [inst_year].[ClassGroup] ([ClassID]),
    CONSTRAINT [FK_SpbsBookRecord_Person] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID]),
    CONSTRAINT [FK_SpbsBookRecord_CreatedBySysUserId] FOREIGN KEY ([CreatedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),
    CONSTRAINT [FK_SpbsBookRecord_ModifiedBySysUserId] FOREIGN KEY ([ModifiedBySysUserId]) REFERENCES [core].[SysUser] ([SysUserID]),

    INDEX [IX_SpbsBookRecord] UNIQUE ([SchoolYear], [InstId], [SpbsBookRecordId]),

    -- Curriculum/ClassGroup deletetion helper indexes
    INDEX [IX_SpbsBookRecord_ClassId] ([ClassId] ASC),
);
GO

exec school_books.spDescTable  N'SpbsBookRecord', N'Запис от книга за движението на учениците от СПИ/ВУИ.'

exec school_books.spDescColumn N'SpbsBookRecord', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'SpbsBookRecord', N'SpbsBookRecordId'               , N'Уникален системно генериран идентификатор.'

exec school_books.spDescColumn N'SpbsBookRecord', N'InstId'                         , N'Идентификатор на институцията.'
exec school_books.spDescColumn N'SpbsBookRecord', N'RecordNumber'                   , N'Пореден номер.'
exec school_books.spDescColumn N'SpbsBookRecord', N'ClassId'                        , N'Идентификатор на група/паралелка, към която принадлежи ученикът.'
exec school_books.spDescColumn N'SpbsBookRecord', N'PersonId'                       , N'Идентификатор на ученик.'

exec school_books.spDescColumn N'SpbsBookRecord', N'SendingCommission'              , N'Изпращаща МКБППМН.'
exec school_books.spDescColumn N'SpbsBookRecord', N'SendingCommissionAddress'       , N'Адрес на МКБППМН.'
exec school_books.spDescColumn N'SpbsBookRecord', N'SendingCommissionPhoneNumber'   , N'Телефони на МКБППМН'

exec school_books.spDescColumn N'SpbsBookRecord', N'InspectorNames'                 , N'Име на инспектора от детска педагогическа стая.'
exec school_books.spDescColumn N'SpbsBookRecord', N'InspectorAddress'               , N'Адрес на инспектора от детска педагогическа стая.'
exec school_books.spDescColumn N'SpbsBookRecord', N'InspectorPhoneNumber'           , N'Телефони на инспектора от детска педагогическа стая'

exec school_books.spDescColumn N'SpbsBookRecord', N'CreateDate'                     , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'SpbsBookRecord', N'CreatedBySysUserId'             , N'Създадено от.'
exec school_books.spDescColumn N'SpbsBookRecord', N'ModifyDate'                     , N'Дата на последно редактиране на записа.'
exec school_books.spDescColumn N'SpbsBookRecord', N'ModifiedBySysUserId'            , N'Последна модификация от.'
exec school_books.spDescColumn N'SpbsBookRecord', N'Version'                        , N'Версия.'
