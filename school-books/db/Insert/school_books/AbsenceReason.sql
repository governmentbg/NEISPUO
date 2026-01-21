GO
PRINT 'Insert AbsenceReason'
GO

SET IDENTITY_INSERT [school_books].[AbsenceReason] ON;

INSERT INTO [school_books].[AbsenceReason] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Здравословни', 'Здравословни', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Семейни', 'Семейни', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Други', 'Други', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[AbsenceReason] OFF;

GO
