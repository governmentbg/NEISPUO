ALTER TABLE [school_books].[PgResult] ALTER COLUMN [StartSchoolYearResult] NVARCHAR(MAX) NULL
ALTER TABLE [school_books].[PgResult] ALTER COLUMN [EndSchoolYearResult] NVARCHAR(MAX) NULL
ALTER TABLE [school_books].[Note] ALTER COLUMN [Description] NVARCHAR(MAX) NOT NULL
ALTER TABLE [school_books].[AdditionalActivity] ALTER COLUMN [Activity] NVARCHAR(MAX) NOT NULL

GO
