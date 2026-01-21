ALTER TABLE [school_books].[AbsencesByStudentsReportItem] ADD [IsTransferred] BIT NOT NULL DEFAULT 0
GO

ALTER TABLE [school_books].[RegularGradePointAverageByStudentsReportItem] ADD [IsTransferred] BIT NOT NULL DEFAULT 0
GO

ALTER TABLE [school_books].[SessionStudentsReportItem] ADD [IsTransferred] BIT NOT NULL DEFAULT 0
GO

ALTER TABLE [school_books].[FinalGradePointAverageByStudentsReportItem] ADD [IsTransferred] BIT NOT NULL DEFAULT 0
GO
