GO
PRINT 'Insert ProtocolExamType'
GO

SET IDENTITY_INSERT [school_books].[ProtocolExamType] ON;

INSERT INTO [school_books].[ProtocolExamType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'писмен изпит', 'писмен изпит', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'устен изпит', 'устен изпит', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'практически изпит', 'практически изпит', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'изпит за определяне на срочна оценка', 'изпит за определяне на срочна оценка', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'изпит за определяне на крайна оценка', 'изпит за определяне на крайна оценка', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'изпит за определяне на годишна оценка', 'изпит за определяне на годишна оценка', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[ProtocolExamType] OFF;

GO
