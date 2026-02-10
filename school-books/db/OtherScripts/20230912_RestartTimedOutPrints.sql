BEGIN TRANSACTION

DECLARE CbpCursor CURSOR FOR
SELECT cbp.SchoolYear, cbp.ClassBookId, cbp.ClassBookPrintId
FROM school_books.ClassBookPrint cbp
WHERE cbp.Status = 1 AND
    cbp.CreateDate >= '2023-09-04 11:52:29.5805608' AND
    cbp.CreateDate <= '2023-09-05 15:14:19.1539678'

DECLARE @SchoolYear INT;
DECLARE @ClassBookId INT;
DECLARE @ClassBookPrintId INT;

OPEN CbpCursor;

FETCH NEXT FROM CbpCursor
INTO @SchoolYear, @ClassBookId, @ClassBookPrintId;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO school_books.QueueMessage(
        Type,
        DueDateUnixTimeMs,
        QueueMessageId,
        Status,
        [Key],
        Tag,
        Payload,
        FailedAttempts,
        FailedAttemptsErrors,
        CreateDateUtc,
        StatusDateUtc
    ) VALUES (
        1,
        DATEDIFF_BIG(MILLISECOND, '1970-01-01', GETUTCDATE()),
        NEXT VALUE FOR school_books.QueueMessageIdSequence,
        1,
        NULL,
        NULL,
        CONCAT('{"SchoolYear":', @SchoolYear, ',"ClassBookId":', @ClassBookId, ',"ClassBookPrintId":', @ClassBookPrintId,'}'),
        0,
        NULL,
        GETUTCDATE(),
        GETUTCDATE()
    );

    FETCH NEXT FROM CbpCursor
    INTO @SchoolYear, @ClassBookId, @ClassBookPrintId;
END

CLOSE CbpCursor
DEALLOCATE CbpCursor
