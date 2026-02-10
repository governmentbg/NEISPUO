GO

CREATE TABLE [school_books].[HisMedicalNoticeReadReceiptAccess](
    [ExtSystemId]                       INT         NOT NULL,
    [HisMedicalNoticeId]                INT         NOT NULL,
    [SchoolYear]                        SMALLINT    NOT NULL,
    [InstId]                            INT         NOT NULL,

    CONSTRAINT [PK_HisMedicalNoticeReadReceiptAccess] PRIMARY KEY ([ExtSystemId], [HisMedicalNoticeId], [SchoolYear], [InstId]),
    CONSTRAINT [FK_HisMedicalNoticeReadReceiptAccess_HisMedicalNoticeReadReceipt]
        FOREIGN KEY ([ExtSystemId], [HisMedicalNoticeId])
        REFERENCES [school_books].[HisMedicalNoticeReadReceipt] ([ExtSystemId], [HisMedicalNoticeId]),
    CONSTRAINT [FK_HisMedicalNoticeReadReceiptAccess_ClassBookExtProvider]
        FOREIGN KEY ([SchoolYear], [InstId]) REFERENCES [school_books].[ClassBookExtProvider] ([SchoolYear], [InstId]),
)
GO

ALTER TABLE [school_books].[HisMedicalNoticeReadReceipt]
ADD
    [CreateDate] DATETIME2 NOT NULL CONSTRAINT DEFAULT_CreateDate DEFAULT GETDATE(),
    [ModifyDate] DATETIME2 NOT NULL CONSTRAINT DEFAULT_ModifyDate DEFAULT GETDATE(),
    CONSTRAINT [FK_HisMedicalNoticeReadReceipt_HisMedicalNoticeId]
        FOREIGN KEY ([HisMedicalNoticeId])
        REFERENCES [school_books].[HisMedicalNotice] ([HisMedicalNoticeId])
GO

ALTER TABLE [school_books].[HisMedicalNoticeReadReceipt]
DROP
    CONSTRAINT DEFAULT_CreateDate,
    CONSTRAINT DEFAULT_ModifyDate
GO

UPDATE hmnrr
SET
    hmnrr.CreateDate = hmnb.CreateDate,
    hmnrr.ModifyDate = hmnb.CreateDate
FROM [school_books].[HisMedicalNoticeReadReceipt] hmnrr
JOIN [school_books].[HisMedicalNotice] hmn ON hmnrr.HisMedicalNoticeId = hmn.HisMedicalNoticeId
JOIN [school_books].[HisMedicalNoticeBatch] hmnb ON hmn.HisMedicalNoticeBatchId = hmnb.HisMedicalNoticeBatchId
GO
