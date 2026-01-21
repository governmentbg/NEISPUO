GO

ALTER TABLE [school_books].[QualificationAcquisitionProtocolStudent]
ADD
    [AverageDecimalGrade] DECIMAL(3,2) NULL;
GO

UPDATE s
SET AverageDecimalGrade = (COALESCE(s.TheoryDecimalGrade, PracticeDecimalGrade) + COALESCE(s.PracticeDecimalGrade, TheoryDecimalGrade)) / 2
FROM [school_books].[QualificationAcquisitionProtocolStudent] s
WHERE s.TheoryDecimalGrade IS NOT NULL OR s.PracticeDecimalGrade IS NOT NULL
GO

ALTER TABLE [school_books].[QualificationAcquisitionProtocolStudent] DROP COLUMN [TheoryDecimalGrade]
ALTER TABLE [school_books].[QualificationAcquisitionProtocolStudent] DROP COLUMN [PracticeDecimalGrade]
GO
