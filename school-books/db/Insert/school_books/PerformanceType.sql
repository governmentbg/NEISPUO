GO
PRINT 'Insert PerformanceType'
GO

SET IDENTITY_INSERT [school_books].[PerformanceType] ON;

INSERT INTO [school_books].[PerformanceType] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'Организирана от ЦПЛР', 'Организирана от ЦПЛР', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'На общинско ниво', 'На общинско ниво', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'На регионално ниво', 'На регионално ниво', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'На национално ниво', 'На национално ниво', 1, '2021-01-01 00:00:00.000', NULL),
(5, 5, 'На международно ниво', 'На международно ниво', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[PerformanceType] OFF;

GO