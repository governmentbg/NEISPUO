ALTER TABLE [school_books].[Publication] ALTER COLUMN [Title] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[QualificationAcquisitionProtocol] ALTER COLUMN [Profession] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[QualificationAcquisitionProtocol] ALTER COLUMN [Speciality] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [GroupNum] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [Profession] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [Speciality] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[SkillsCheckExamResultProtocolEvaluator] ALTER COLUMN [Name] NVARCHAR(100) NOT NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [SendingCommission] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [SendingCommissionAddress] NVARCHAR(1000) NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [SendingCommissionPhoneNumber] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [InspectorNames] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [InspectorAddress] NVARCHAR(1000) NULL;
ALTER TABLE [school_books].[SpbsBookRecord] ALTER COLUMN [InspectorPhoneNumber] NVARCHAR(100) NULL;
GO
