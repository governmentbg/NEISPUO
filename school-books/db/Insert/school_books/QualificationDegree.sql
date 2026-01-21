GO
PRINT 'Insert QualificationDegree'
GO

SET IDENTITY_INSERT [school_books].[QualificationDegree] ON;

INSERT INTO [school_books].[QualificationDegree] (
    [Id]
    ,[SortOrd]
    ,[Name]
    ,[Description]
    ,[IsValid]
    ,[ValidFrom]
    ,[ValidTo])
VALUES
(1, 1, 'първа', 'първа', 1,'2021-01-01 00:00:00.000', NULL),
(2, 2, 'втора', 'втора', 1, '2021-01-01 00:00:00.000', NULL),
(3, 3, 'трета', 'трета', 1, '2021-01-01 00:00:00.000', NULL),
(4, 4, 'четвърта', 'четвърта', 1, '2021-01-01 00:00:00.000', NULL)

SET IDENTITY_INSERT [school_books].[QualificationDegree] OFF;

GO
