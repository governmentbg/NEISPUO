PRINT 'Create QueueMessage table'
GO

EXEC [school_books].[spCreateIdSequence] N'QueueMessage'
GO

-- create monthly partition function from Jan 2022 to Dec 2029
DECLARE @pfMonthlyQueueMessageSQL nvarchar(max) =
    N'CREATE PARTITION FUNCTION pfMonthlyQueueMessage (BIGINT)
    AS RANGE RIGHT FOR VALUES (';
DECLARE @i datetime2 = '20220101';  -- from Jan 2022
WHILE @i < '20291201'               -- to Dec 2029
BEGIN
SET @pfMonthlyQueueMessageSQL += '''' + CAST(DATEDIFF_BIG(MILLISECOND, '1970-01-01', @i) AS NVARCHAR(20)) + '''' + N', ';
SET @i = DATEADD(MM, 1, @i);
END
SET @pfMonthlyQueueMessageSQL += '''' + CAST(DATEDIFF_BIG(MILLISECOND, '1970-01-01', @i) AS NVARCHAR(20))+ '''' + N');';
EXEC sp_executesql @pfMonthlyQueueMessageSQL;
GO

-- create monthly partition scheme
CREATE PARTITION SCHEME [psMonthlyQueueMessage]
AS PARTITION [pfMonthlyQueueMessage] ALL TO ([PRIMARY]);
GO

CREATE TABLE [school_books].[QueueMessage](
    [Type]                          INT               NOT NULL,
    [DueDateUnixTimeMs]             BIGINT            NOT NULL,
    [QueueMessageId]                INT               NOT NULL,
    [Status]                        INT               NOT NULL,
    [Key]                           NVARCHAR(100)     NULL,
    [Tag]                           NVARCHAR(MAX)     NULL,
    [Payload]                       NVARCHAR(MAX)     NULL,
    [FailedAttempts]                INT               NOT NULL,
    [FailedAttemptsErrors]          NVARCHAR(MAX)     NULL,
    [CreateDateUtc]                 DATETIME2         NOT NULL,
    [StatusDateUtc]                 DATETIME2         NOT NULL,
    [Version]                       ROWVERSION        NOT NULL,

    CONSTRAINT [PK_QueueMessage] PRIMARY KEY ([Type], [DueDateUnixTimeMs], [QueueMessageId]),
    CONSTRAINT [CHK_QueueMessage_Status] CHECK ([Status] IN (1, 2, 3, 4, 5)),
    CONSTRAINT [CHK_QueueMessage_Type] CHECK ([Type] IN (1, 2, 3)),
)
ON [psMonthlyQueueMessage]([DueDateUnixTimeMs]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_QueueMessage_Key]
    ON [school_books].[QueueMessage]([Key])
    WHERE [Key] IS NOT NULL
    ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UQ_QueueMessage_Status_Pending]
    ON [school_books].[QueueMessage]([Type], [DueDateUnixTimeMs], [QueueMessageId])
    INCLUDE ([FailedAttempts], [StatusDateUtc])
    WHERE [Status] = 1 -- Pending
    ON [PRIMARY]
GO
