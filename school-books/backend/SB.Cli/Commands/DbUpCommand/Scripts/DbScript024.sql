ALTER TABLE [school_books].[IndividualWork] ALTER COLUMN [IndividualWorkActivity] NVARCHAR(4000) NOT NULL
GO

ALTER TABLE [school_books].[IndividualWork] DROP COLUMN [AdditionalTrainingResult]
GO
