ALTER TABLE [school_books].[ExamResultProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[ExamResultProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[GraduationThesisDefenseProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[GraduationThesisDefenseProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[HighSchoolCertificateProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[HighSchoolCertificateProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[QualificationAcquisitionProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[QualificationAcquisitionProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[QualificationExamResultProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[QualificationExamResultProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

ALTER TABLE [school_books].[StateExamsAdmProtocolCommissioner] ADD [OrderNum] SMALLINT NOT NULL CONSTRAINT DEFAULT_OrderNum DEFAULT 0;
ALTER TABLE [school_books].[StateExamsAdmProtocolCommissioner] DROP CONSTRAINT DEFAULT_OrderNum

GO
