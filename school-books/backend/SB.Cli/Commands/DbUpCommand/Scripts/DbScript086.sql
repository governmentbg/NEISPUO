ALTER TABLE [school_books].[ExamDutyProtocol] ALTER COLUMN [GroupNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[ExamDutyProtocol] ALTER COLUMN [EduFormId] INT NULL;
ALTER TABLE [school_books].[ExamDutyProtocol] ALTER COLUMN [SessionType] NVARCHAR(100) NULL;

ALTER TABLE [school_books].[ExamResultProtocol] ALTER COLUMN [GroupNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[ExamResultProtocol] ALTER COLUMN [EduFormId] INT NULL;
ALTER TABLE [school_books].[ExamResultProtocol] ALTER COLUMN [SessionType] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[ExamResultProtocol] ALTER COLUMN [ProtocolNumber] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[ExamResultProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;

ALTER TABLE [school_books].[GradeChangeExamsAdmProtocol] ALTER COLUMN [ProtocolNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;
ALTER TABLE [school_books].[GradeChangeExamsAdmProtocol] ALTER COLUMN [ExamSession] NVARCHAR(100) NULL;

ALTER TABLE [school_books].[GraduationThesisDefenseProtocol] ALTER COLUMN [EduFormId] INT NULL;
ALTER TABLE [school_books].[GraduationThesisDefenseProtocol] ALTER COLUMN [SessionType] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[GraduationThesisDefenseProtocol] ALTER COLUMN [ProtocolNumber] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[GraduationThesisDefenseProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;

ALTER TABLE [school_books].[HighSchoolCertificateProtocol] ALTER COLUMN [ProtocolNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[HighSchoolCertificateProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;
ALTER TABLE [school_books].[HighSchoolCertificateProtocol] ALTER COLUMN [ExamSession] NVARCHAR(100) NULL;

ALTER TABLE [school_books].[NvoExamDutyProtocol] ALTER COLUMN [RoomNumber] NVARCHAR(100) NULL;

ALTER TABLE [school_books].[QualificationAcquisitionProtocol] ALTER COLUMN [ProtocolNumber] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[QualificationAcquisitionProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;

ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [GroupNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [EduFormId] INT NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [SessionType] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [ProtocolNumber] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[QualificationExamResultProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;

ALTER TABLE [school_books].[SkillsCheckExamResultProtocol] ALTER COLUMN [Date] DATE NULL;

ALTER TABLE [school_books].[StateExamDutyProtocol] ALTER COLUMN [EduFormId] INT NULL;
ALTER TABLE [school_books].[StateExamDutyProtocol] ALTER COLUMN [SessionType] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[StateExamDutyProtocol] ALTER COLUMN [RoomNumber] NVARCHAR(100) NULL;

ALTER TABLE [school_books].[StateExamsAdmProtocol] ALTER COLUMN [ProtocolNum] NVARCHAR(100) NULL;
ALTER TABLE [school_books].[StateExamsAdmProtocol] ALTER COLUMN [ProtocolDate] DATE NULL;
ALTER TABLE [school_books].[StateExamsAdmProtocol] ALTER COLUMN [ExamSession] NVARCHAR(100) NULL;

GO
