PRINT 'Create QualificationAcquisitionProtocolCommissioner table'
GO

CREATE TABLE [school_books].[QualificationAcquisitionProtocolCommissioner] (
    [SchoolYear]                                    SMALLINT         NOT NULL,
    [QualificationAcquisitionProtocolId]            INT              NOT NULL,
    [PersonId]                                      INT              NOT NULL,

    [IsChairman]                                    BIT              NOT NULL,
    [OrderNum]                                      SMALLINT         NOT NULL,

    CONSTRAINT [PK_QualificationAcquisitionProtocolCommissioner] PRIMARY KEY ([SchoolYear], [QualificationAcquisitionProtocolId], [PersonId]),
    CONSTRAINT [FK_QualificationAcquisitionProtocolCommissioner_QualificationAcquisitionProtocol] FOREIGN KEY ([SchoolYear], [QualificationAcquisitionProtocolId])
        REFERENCES [school_books].[QualificationAcquisitionProtocol] ([SchoolYear], [QualificationAcquisitionProtocolId]),
    -- external references
    CONSTRAINT [FK_QualificationAcquisitionProtocolCommissioner_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [core].[Person] ([PersonID])
);
GO

exec school_books.spDescTable  N'QualificationAcquisitionProtocolCommissioner', N'Протокол за придобиване на професионална квалификация - член на комисията.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocolCommissioner', N'SchoolYear'                            , N'Учебна година.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolCommissioner', N'QualificationAcquisitionProtocolId'    , N'Идентификатор на протокол за придобиване на професионална квалификация.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolCommissioner', N'PersonId'                              , N'Идентификатор на член на комисията.'

exec school_books.spDescColumn N'QualificationAcquisitionProtocolCommissioner', N'IsChairman'                            , N'Председател – Да/Не.'
exec school_books.spDescColumn N'QualificationAcquisitionProtocolCommissioner', N'OrderNum'                              , N'Пореден номер.'
