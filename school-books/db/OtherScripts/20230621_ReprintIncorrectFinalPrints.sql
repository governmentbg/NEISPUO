BEGIN TRANSACTION

DECLARE CbpCursor CURSOR FOR
SELECT cbp.SchoolYear, cbp.ClassBookId, cbp.ClassBookPrintId
FROM school_books.ClassBookPrint cbp
JOIN school_books.ClassBook cb
    ON cb.SchoolYear = cbp.SchoolYear
    AND cb.ClassBookId = cbp.ClassBookId
WHERE cb.BookType = 4 
    AND cbp.IsSigned = 1
    AND IsExternal = 0
    AND cbp.CreateDate > '2023-06-08 10:51:19'
    AND cbp.StatusDate < '2023-06-21 16:24:20'
    AND cbp.Status = 2

DECLARE @SchoolYear INT;
DECLARE @ClassBookId INT;
DECLARE @ClassBookPrintId INT;

OPEN CbpCursor;

FETCH NEXT FROM CbpCursor
INTO @SchoolYear, @ClassBookId, @ClassBookPrintId;

WHILE @@FETCH_STATUS = 0
BEGIN
    UPDATE school_books.ClassBook
    SET IsFinalized = 0,
        ModifyDate = GETDATE(),
        ModifiedBySysUserId = 881627 -- neispuo_ciela@edu.mon.bg
    WHERE SchoolYear = @SchoolYear
        AND ClassBookId = @ClassBookId

    UPDATE school_books.ClassBookStatusChange
    SET IsLast = 0
    WHERE SchoolYear = @SchoolYear
        AND ClassBookId = @ClassBookId
        AND IsLast = 1

    UPDATE school_books.ClassBookPrint
    SET IsFinal = 0
    WHERE SchoolYear = @SchoolYear
        AND ClassBookId = @ClassBookId
        AND IsFinal = 1

    INSERT INTO school_books.ClassBookStatusChange(
        SchoolYear,
        ClassBookId,
        ClassBookStatusChangeId,
        Type,
        ChangeDate,
        ChangedBySysUserId,
        Signees,
        IsLast
    )
    VALUES (
        @SchoolYear,
        @ClassBookId,
        NEXT VALUE FOR school_books.ClassBookStatusChangeIdSequence,
        4, -- Unfinalized
        GETDATE(),
        881627, -- neispuo_ciela@edu.mon.bg
        NULL,
        1
    )

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
