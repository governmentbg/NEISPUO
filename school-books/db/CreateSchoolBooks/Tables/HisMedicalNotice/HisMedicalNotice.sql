PRINT 'Create HisMedicalNotice table'
GO

CREATE TABLE [school_books].[HisMedicalNotice] (
    -- НЕИСПУО
    [HisMedicalNoticeId]        INT             NOT NULL IDENTITY,
    [HisMedicalNoticeBatchId]   INT             NOT NULL,
    [PersonalIDTypeId]          INT             NOT NULL,
    [CreateDate]                DATETIME2       NOT NULL,
    [Version]                   ROWVERSION      NOT NULL,

    -- Данни за медицинската бележка изпратени от НЗИС
    [NrnMedicalNotice]          NVARCHAR(12)    NOT NULL,
    [NrnExamination]            NVARCHAR(12)    NOT NULL,
    -- Данни за пациента
    [IdentifierType]            SMALLINT        NOT NULL,
    [Identifier]                NVARCHAR(255)   NOT NULL,
    [GivenName]                 NVARCHAR(255)   NOT NULL,
    [FamilyName]                NVARCHAR(255)   NOT NULL,
    -- Данни за лекаря
    [Pmi]                       NVARCHAR(255)   NOT NULL,
    -- Информация за медицинското състояние и данни за самата медицинска бележка
    [FromDate]                  DATE            NOT NULL,
    [ToDate]                    DATE            NOT NULL,
    [AuthoredOn]                DATETIME2       NOT NULL,

    CONSTRAINT [PK_HisMedicalNotice] PRIMARY KEY NONCLUSTERED ([HisMedicalNoticeId]),
    CONSTRAINT [UK_HisMedicalNotice] UNIQUE CLUSTERED ([CreateDate], [HisMedicalNoticeId]),

    CONSTRAINT [FK_HisMedicalNotice_HisMedicalNoticeBatchId]
        FOREIGN KEY ([HisMedicalNoticeBatchId])
        REFERENCES [school_books].[HisMedicalNoticeBatch] ([HisMedicalNoticeBatchId]),

    CONSTRAINT [UK_HisMedicalNotice_NrnMedicalNotice] UNIQUE ([NrnMedicalNotice]),

    INDEX [IX_HisMedicalNotice_PersonalIDTypeId_Identifier_CreateDate] ([PersonalIDTypeId], [Identifier], [CreateDate])
        INCLUDE ([NrnMedicalNotice], [NrnExamination], [IdentifierType], [GivenName], [FamilyName], [Pmi], [FromDate], [ToDate], [AuthoredOn]),

    -- external references
    CONSTRAINT [FK_HisMedicalNotice_PersonalIdType]
        FOREIGN KEY ([PersonalIDTypeId])
        REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID]),
);
GO

exec school_books.spDescTable  N'HisMedicalNotice', N'Медицинска бележка от НЗИС.'

exec school_books.spDescColumn N'HisMedicalNotice', N'HisMedicalNoticeId'           , N'Уникален системно генериран идентификатор.'
exec school_books.spDescColumn N'HisMedicalNotice', N'HisMedicalNoticeBatchId'      , N'Идентификатор на пакет от бележки изпратени от НЗИС.'
exec school_books.spDescColumn N'HisMedicalNotice', N'PersonalIDTypeId'             , N'Тип на идентификатор. Номенклатура PersonalIDType.'
exec school_books.spDescColumn N'HisMedicalNotice', N'CreateDate'                   , N'Дата на създаване на записа.'
exec school_books.spDescColumn N'HisMedicalNotice', N'Version'                      , N'Версия.'

exec school_books.spDescColumn N'HisMedicalNotice', N'NrnMedicalNotice'             , N'Уникален идентификатор на електронния документ - НРН на медицинската бележка.'
exec school_books.spDescColumn N'HisMedicalNotice', N'NrnExamination'               , N'НРН на е-преглед или е-хоспитализация, спрямо който се издава медицинската бележка.'
exec school_books.spDescColumn N'HisMedicalNotice', N'IdentifierType'               , N'Типът на идентификатора на лицето. 1 - ЕГН. 2 - ЛНЧ. 3 - Социален номер - за чужди граждани. 4 - Номер на паспорт. 5 - Друг идентификатор.'
exec school_books.spDescColumn N'HisMedicalNotice', N'Identifier'                   , N'Идентификатор на лицето, пряко свързан с типа. Тоест самото ЕГН или ЛНЧ на ученика.'
exec school_books.spDescColumn N'HisMedicalNotice', N'GivenName'                    , N'Първо име на лицето.'
exec school_books.spDescColumn N'HisMedicalNotice', N'FamilyName'                   , N'Фамилия на лицето.'
exec school_books.spDescColumn N'HisMedicalNotice', N'Pmi'                          , N'УИН на лекаря.'
exec school_books.spDescColumn N'HisMedicalNotice', N'FromDate'                     , N'Начална дата на валидност на медицинската бележка (включително).'
exec school_books.spDescColumn N'HisMedicalNotice', N'ToDate'                       , N'Крайна дата на валидност на медицинската бележка (включително).'
exec school_books.spDescColumn N'HisMedicalNotice', N'AuthoredOn'                   , N'Дата на издаване на медицинската бележка.'
