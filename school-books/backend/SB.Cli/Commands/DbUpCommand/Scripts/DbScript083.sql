GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_QueueMessage_Status_Pending]
    ON [school_books].[QueueMessage]([Type], [DueDateUnixTimeMs], [QueueMessageId])
    INCLUDE ([FailedAttempts], [StatusDateUtc])
    WHERE [Status] = 1 -- Pending
    ON [PRIMARY]

GO
