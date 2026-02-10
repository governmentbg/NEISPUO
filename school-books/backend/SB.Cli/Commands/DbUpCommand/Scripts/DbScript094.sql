GO

CREATE TABLE [school_books].[HisMedicalNoticeBatch] (
    [HisMedicalNoticeBatchId]   INT             NOT NULL IDENTITY(1,1),
    [CreateDate]                DATETIME2       NOT NULL,
    [RequestId]                 NVARCHAR(50)    NOT NULL,
    [Error]                     NVARCHAR(MAX)   NULL,
    [Version]                   ROWVERSION      NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeBatch] PRIMARY KEY ([HisMedicalNoticeBatchId]),
);
GO

CREATE TABLE [school_books].[HisMedicalNotice] (
    -- НЕИСПУО
    [HisMedicalNoticeId]        INT             NOT NULL IDENTITY,
    [HisMedicalNoticeBatchId]   INT             NOT NULL,
    [PersonalIDTypeId]          INT             NOT NULL,
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

    CONSTRAINT [PK_HisMedicalNotice] PRIMARY KEY ([HisMedicalNoticeId]),
    CONSTRAINT [FK_HisMedicalNotice_HisMedicalNoticeBatchId]
        FOREIGN KEY ([HisMedicalNoticeBatchId])
        REFERENCES [school_books].[HisMedicalNoticeBatch] ([HisMedicalNoticeBatchId]),
    CONSTRAINT [FK_HisMedicalNotice_PersonalIdType]
        FOREIGN KEY ([PersonalIDTypeId])
        REFERENCES [noms].[PersonalIDType] ([PersonalIDTypeID]),

    CONSTRAINT [UK_HisMedicalNotice_NrnMedicalNotice] UNIQUE CLUSTERED ([NrnMedicalNotice]),
);
GO

CREATE TABLE [school_books].[HisMedicalNoticeSchoolYear] (
    [HisMedicalNoticeId]    INT             NOT NULL,
    [SchoolYear]            SMALLINT        NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeSchoolYear] PRIMARY KEY ([HisMedicalNoticeId], [SchoolYear])
);
GO

CREATE TABLE [school_books].[HisMedicalNoticeReadReceipt] (
    [ExtSystemId]           INT             NOT NULL,
    [HisMedicalNoticeId]    INT             NOT NULL,
    [IsAcknowledged]        BIT             NOT NULL,
    [Version]               ROWVERSION      NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeReadReceipt] PRIMARY KEY ([ExtSystemId], [HisMedicalNoticeId])
);
GO
