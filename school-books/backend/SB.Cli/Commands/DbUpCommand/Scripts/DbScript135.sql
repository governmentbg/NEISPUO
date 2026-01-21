ALTER TABLE [school_books].[QueueMessage] DROP CONSTRAINT [CHK_QueueMessage_Type]
GO

ALTER TABLE [school_books].[QueueMessage] ADD CONSTRAINT [CHK_QueueMessage_Type] CHECK ([Type] IN (1, 2, 3))
GO
