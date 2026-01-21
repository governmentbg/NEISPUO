GO

ALTER TABLE [school_books].[HisMedicalNoticeReadReceipt]
ADD CONSTRAINT [FK_HisMedicalNoticeReadReceipt_ExtSystemId]
    FOREIGN KEY ([ExtSystemId])
    REFERENCES [core].[ExtSystem] ([ExtSystemID]);
GO
