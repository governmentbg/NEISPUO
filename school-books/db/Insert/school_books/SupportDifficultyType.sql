GO
PRINT 'Insert SupportDifficultyType'
GO

SET IDENTITY_INSERT [school_books].[SupportDifficultyType] ON;

INSERT INTO [school_books].[SupportDifficultyType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Затруднения в обучението', 'Затруднения в обучението', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'Рискови фактори в средата', 'Рискови фактори в средата', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'Проблемно поведение', 'Проблемно поведение', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'Хронично заболяване', 'Хронично заболяване', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'Напредва по-бързо от останалите ученици', 'Напредва по-бързо от останалите ученици', 1, '2021-01-01 00:00:00.000', NULL),
(6, 6, 'Друго', 'Друго', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[SupportDifficultyType] OFF;

GO
