PRINT 'Create SpbsBookRecordMovement table'
GO

CREATE TABLE [school_books].[SpbsBookRecordMovement] (
    [SchoolYear]                SMALLINT         NOT NULL,
    [SpbsBookRecordId]          INT              NOT NULL,
    [OrderNum]                  SMALLINT         NOT NULL,

    [CourtDecisionNumber]       NVARCHAR (100)   NULL,
    [CourtDecisionDate]         DATE             NULL,
    [IncomingInstId]            INT              NULL,
    [IncommingLetterNumber]     NVARCHAR (100)   NULL,
    [IncommingLetterDate]       DATE             NULL,
    [IncommingDate]             DATE             NULL,
    [IncommingDocNumber]        NVARCHAR (100)   NULL,
    [TransferInstId]            INT              NULL,
    [TransferReason]            NVARCHAR (1000)  NULL,
    [TransferProtocolNumber]    NVARCHAR (100)   NULL,
    [TransferProtocolDate]      DATE             NULL,
    [TransferLetterNumber]      NVARCHAR (100)   NULL,
    [TransferLetterDate]        DATE             NULL,
    [TransferCertificateNumber] NVARCHAR (100)   NULL,
    [TransferCertificateDate]   DATE             NULL,
    [TransferMessageNumber]     NVARCHAR (100)   NULL,
    [TransferMessageDate]       DATE             NULL,

    CONSTRAINT [PK_SpbsBookRecordMovement] PRIMARY KEY ([SchoolYear], [SpbsBookRecordId], [OrderNum]),

    CONSTRAINT [FK_SpbsBookRecordMovement_SpbsBookRecord] FOREIGN KEY ([SchoolYear], [SpbsBookRecordId]) REFERENCES [school_books].[SpbsBookRecord] ([SchoolYear], [SpbsBookRecordId]),
);
GO

exec school_books.spDescTable  N'SpbsBookRecordMovement', N'Запис от книга за движението на учениците от СПИ/ВУИ - движение.'

exec school_books.spDescColumn N'SpbsBookRecordMovement', N'SchoolYear'                     , N'Учебна година.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'SpbsBookRecordId'               , N'Идентификатор на запис от книга за движението на учениците от СПИ/ВУИ.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'OrderNum'                       , N'Пореден номер.'

exec school_books.spDescColumn N'SpbsBookRecordMovement', N'CourtDecisionNumber'            , N'Номер на съдебното решение.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'CourtDecisionDate'              , N'Дата на съдебното решение.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'IncomingInstId'                 , N'Идентификатор на институция на постъпване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'IncommingLetterNumber'          , N'Номер на настанителното писмо от МОН.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'IncommingLetterDate'            , N'Дата на настанителното писмо от МОН.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'IncommingDate'                  , N'Дата на постъпване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'IncommingDocNumber'             , N'Документ за постъпване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferInstId'                 , N'Идентификатор на институцията на преместване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferReason'                 , N'Основание за преместване/прекратяване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferProtocolNumber'         , N'Номер на протокола за преместване/прекратяване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferProtocolDate'           , N'Дата на протокола за преместване/прекратяване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferLetterNumber'           , N'Номер на писмото за преместване/прекратяване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferLetterDate'             , N'Дата на писмото за преместване/прекратяване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferCertificateNumber'      , N'Номер на удостоверението за преместване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferCertificateDate'        , N'Дата на удостоверението за преместване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferMessageNumber'          , N'Номер на съобщението за преместване.'
exec school_books.spDescColumn N'SpbsBookRecordMovement', N'TransferMessageDate'            , N'Дата на съобщението за преместване.'
