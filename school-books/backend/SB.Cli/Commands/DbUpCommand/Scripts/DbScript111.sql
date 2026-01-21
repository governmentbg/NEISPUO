GO

UPDATE hmn
SET CreateDate = hmnb.CreateDate
FROM [school_books].[HisMedicalNotice] hmn
JOIN [school_books].[HisMedicalNoticeBatch] hmnb
    ON hmn.HisMedicalNoticeBatchId = hmnb.HisMedicalNoticeBatchId
WHERE hmn.CreateDate IS NULL
GO

ALTER TABLE [school_books].[HisMedicalNotice]
ALTER COLUMN [CreateDate] DATETIME2 NOT NULL
GO

ALTER TABLE [school_books].[HisMedicalNotice]
DROP CONSTRAINT [UK_HisMedicalNotice]
GO

ALTER TABLE [school_books].[HisMedicalNotice]
ADD CONSTRAINT [UK_HisMedicalNotice] UNIQUE CLUSTERED ([CreateDate], [HisMedicalNoticeId])
GO
