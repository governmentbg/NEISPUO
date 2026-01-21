SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BarcodeYear](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Edition] [smallint] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[HeaderPage] [nvarchar](128) NOT NULL,
	[InternalPage] [nvarchar](128) NULL,
	[BasicDocumentId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_BarcodeYear] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsMoC] [bit] NOT NULL,
	[HasBarcode] [bit] NOT NULL,
	[Contents] [nvarchar](max) NULL,
	[Pages] [int] NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[AttachedImagesCountMin] [int] NULL,
	[AttachedImagesCountMax] [int] NULL,
	[ReportFormPath] [nvarchar](1024) NULL,
	[IsValidation] [bit] NOT NULL,
	[CodeClassName] [nvarchar](1000) NULL,
	[IsAppendix] [bit] NOT NULL,
	[HasFactoryNumber] [bit] NOT NULL,
	[IsUniqueForStudent] [bit] NOT NULL,
	[HasSubjects] [bit] NOT NULL,
	[IsDuplicate] [bit] NOT NULL,
	[SeriesFormat] [nvarchar](100) NULL,
	[PageOrientation] [int] NULL,
	[IsIncludedInRegister] [bit] NOT NULL,
	[BasicClasses] [nvarchar](450) NULL,
	[Abbreviation] [nvarchar](255) NULL,
	[IsRuoDoc] [bit] NOT NULL,
	[MainBasicDocuments] [nvarchar](1000) NULL,
	[DetailedSchoolTypes] [nvarchar](1000) NULL,
 CONSTRAINT [PK_BasicDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentGenerator](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[RegNumberTotal] [int] NOT NULL,
	[RegNumberYear] [int] NOT NULL,
	[LastUpdateDateTime] [datetime2](7) NOT NULL,
	[RegionId] [int] NULL,
 CONSTRAINT [PK_BasicDocumentGenerator] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentLimit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[DetailedSchoolTypeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentMargin](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Left1Margin] [int] NOT NULL,
	[Top1Margin] [int] NOT NULL,
	[Left2Margin] [int] NOT NULL,
	[Top2Margin] [int] NOT NULL,
	[InstitutionId] [int] NULL,
	[BasicDocumentId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[BasicDocumentPrintFormId] [int] NOT NULL,
	[RuoRegId] [int] NULL,
 CONSTRAINT [PK_BasicDocumentMargin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentPart](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[PrintedLines] [int] NOT NULL,
	[TotalLines] [int] NOT NULL,
	[IsHorariumHidden] [bit] NOT NULL,
	[Code] [nvarchar](5) NULL,
	[BasicClass] [nvarchar](255) NULL,
	[BasicSubjectTypeId] [smallint] NULL,
	[SubjectTypesList] [nvarchar](max) NULL,
	[ExternalEvaluationTypesList] [nvarchar](max) NULL,
	[BasicClassId] [int] NULL,
	[Category] [int] NULL,
 CONSTRAINT [PK_BasicDocumentPart] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentPrintForm](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Edition] [smallint] NOT NULL,
	[ReportFormPath] [nvarchar](1024) NULL,
	[BasicDocumentId] [int] NOT NULL,
 CONSTRAINT [PK_BasicDocumentPrintForm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentRegDate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[MinRegDate] [datetime2](7) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_BasicDocumentRegDate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentSequence](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[RegNumberTotal] [int] NOT NULL,
	[RegNumberYear] [int] NOT NULL,
	[RegDate] [date] NOT NULL,
	[RegionId] [int] NULL,
 CONSTRAINT [PK_BasicDocumentSequence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[BasicDocumentSubject](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BasicDocumentPartId] [int] NOT NULL,
	[SubjectId] [int] NULL,
	[Position] [int] NOT NULL,
	[SubjectCanChange] [bit] NOT NULL,
	[SubjectTypeId] [int] NULL,
 CONSTRAINT [PK_BasicDocumentSubject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[Diploma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NULL,
	[InstitutionId] [int] NULL,
	[InstitutionName] [nvarchar](1024) NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[EduFormId] [int] NULL,
	[EduDuration] [decimal](5, 1) NULL,
	[GPA] [decimal](5, 2) NULL,
	[GPAText] [nvarchar](100) NULL,
	[ProtocolNumber] [nvarchar](50) NULL,
	[ProtocolDate] [date] NULL,
	[Series] [nvarchar](50) NULL,
	[FactoryNumber] [nvarchar](50) NULL,
	[RegistrationNumberTotal] [nvarchar](100) NOT NULL,
	[RegistrationNumberYear] [nvarchar](199) NULL,
	[RegistrationDate] [date] NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[FirstNameLatin] [nvarchar](255) NULL,
	[MiddleName] [nvarchar](255) NULL,
	[MiddleNameLatin] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[LastNameLatin] [nvarchar](255) NULL,
	[PersonalId] [nvarchar](255) NULL,
	[PersonalIDType] [int] NOT NULL,
	[NationalityID] [int] NULL,
	[BirthDate] [date] NULL,
	[BirthPlaceTown] [nvarchar](255) NULL,
	[BirthPlaceMunicipality] [nvarchar](255) NULL,
	[BirthPlaceRegion] [nvarchar](255) NULL,
	[Nationality] [nvarchar](255) NULL,
	[Gender] [int] NOT NULL,
	[Contents] [nvarchar](max) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsFinalized] [bit] NOT NULL,
	[FinalizedDate] [datetime2](7) NULL,
	[IsSigned] [bit] NOT NULL,
	[SignedDate] [datetime2](7) NULL,
	[Signature] [nvarchar](max) NULL,
	[IsPublic] [bit] NOT NULL,
	[IsDiplomaFormPrinted] [bit] NOT NULL,
	[SPPOOProfessionId] [int] NULL,
	[SPPOOSpecialityId] [int] NULL,
	[IsCancelled] [bit] NOT NULL,
	[CancellationDate] [datetime2](7) NULL,
	[CancellationReason] [nvarchar](1000) NULL,
	[CancelledBySysUserId] [int] NULL,
	[VETQualification] [nvarchar](255) NULL,
	[FLGELevelId] [int] NULL,
	[ITLevelId] [int] NULL,
	[ValidationSchoolYear] [smallint] NULL,
	[ValidationSession] [nvarchar](50) NULL,
	[EducationTypeId] [int] NULL,
	[RecognitionOrderNo] [nvarchar](35) NULL,
	[RecognitionOrderDate] [date] NULL,
	[OriginalDiplomaId] [int] NULL,
	[YearGraduated] [smallint] NULL,
	[BasicDocumentId] [int] NOT NULL,
	[ClassTypeId] [int] NULL,
	[tmp_IDNumber] [bigint] NULL,
	[tmp_TemplateID] [bigint] NULL,
	[tmp_CurrVer] [smallint] NULL,
	[StateExamQualificationGrade] [decimal](5, 2) NULL,
	[StateExamQualificationGradeText] [nvarchar](100) NULL,
	[BasicDocumentName] [nvarchar](255) NULL,
	[MinistryId] [int] NULL,
	[MinistryName] [nvarchar](255) NULL,
	[Principal] [nvarchar](1024) NULL,
	[Deputy] [nvarchar](1024) NULL,
	[EduFormName] [nvarchar](255) NULL,
	[ClassTypeName] [nvarchar](255) NULL,
	[SPPOOProfessionName] [nvarchar](255) NULL,
	[SPPOOSpecialityName] [nvarchar](255) NULL,
	[SignedBySysUserID] [int] NULL,
	[IsEditable] [bit] NOT NULL,
	[EditableSetDate] [datetime2](7) NULL,
	[EditableSetReason] [nvarchar](1000) NULL,
	[EditableSetBySysUserId] [int] NULL,
	[NKR] [int] NULL,
	[EKR] [int] NULL,
	[Session] [nvarchar](100) NULL,
	[Description] [nvarchar](max) NULL,
	[VetLevel] [int] NULL,
	[BasicClassId] [int] NULL,
	[ProfessionPart] [nvarchar](400) NULL,
	[CommissionOrderNumber] [nvarchar](50) NULL,
	[CommissionOrderData] [datetime2](7) NULL,
	[LeadTeacher] [nvarchar](100) NULL,
	[IsMigrated] [bit] NOT NULL,
	[RuoRegId] [int] NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[DiplomaAdditionalDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiplomaId] [int] NOT NULL,
	[BasicDocumentId] [int] NULL,
	[BasicDocumentName] [nvarchar](255) NULL,
	[InstitutionId] [int] NULL,
	[InstitutionName] [nvarchar](1024) NULL,
	[Series] [nvarchar](50) NULL,
	[FactoryNumber] [nvarchar](50) NULL,
	[RegistratioNumber] [nvarchar](50) NULL,
	[RegistrationDate] [date] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[RegistrationNumberYear] [nvarchar](199) NULL,
	[InstitutionAddress] [nvarchar](1024) NULL,
	[Town] [nvarchar](100) NULL,
	[Municipality] [nvarchar](100) NULL,
	[Region] [nvarchar](100) NULL,
	[LocalArea] [nvarchar](100) NULL,
	[MainDiplomaId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[DiplomaCreateRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[RequestingInstitutionId] [int] NOT NULL,
	[CurrentInstitutionId] [int] NULL,
	[CurrentInstitutionName] [nvarchar](1024) NULL,
	[BasicDocumentId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[RegistrationNumber] [nvarchar](100) NOT NULL,
	[RegistrationNumberYear] [nvarchar](199) NULL,
	[RegistrationDate] [date] NULL,
	[Note] [nvarchar](2048) NULL,
	[IsGranted] [bit] NOT NULL,
	[DiplomaId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_DiplomaCreateRequest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[DiplomaDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiplomaId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[BlobId] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[tmp_CurrYear] [smallint] NULL,
	[tmp_SchoolID] [bigint] NULL,
	[tmp_FileName] [nvarchar](100) NULL,
 CONSTRAINT [PK_DiplomaDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[DiplomaSubject](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiplomaId] [int] NOT NULL,
	[BasicDocumentPartId] [int] NOT NULL,
	[BasicDocumentSubjectId] [int] NULL,
	[SubjectId] [int] NULL,
	[Grade] [decimal](5, 2) NULL,
	[GradeText] [nvarchar](100) NULL,
	[Position] [int] NOT NULL,
	[Horarium] [int] NULL,
	[FlSubjectId] [int] NULL,
	[FlHorarium] [int] NULL,
	[NVOPoints] [decimal](7, 3) NULL,
	[SubjectTypeId] [int] NULL,
	[SubjectName] [nvarchar](255) NULL,
	[FlLevel] [nvarchar](255) NULL,
	[FlSubjectName] [nvarchar](255) NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ECTS] [char](1) NULL,
	[GradeCategory] [int] NOT NULL,
	[SpecialNeedsGrade] [int] NULL,
	[OtherGrade] [int] NULL,
	[ParentId] [int] NULL,
	[BasicClassId] [int] NULL,
	[QualitativeGrade] [int] NULL,
 CONSTRAINT [PK_DocumentSubject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[EducationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_EducationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[EKRType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_EKRType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[GraduationCommissionMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NULL,
	[FullName] [nvarchar](1000) NOT NULL,
	[FullNameLatin] [nvarchar](1000) NULL,
	[Position] [int] NOT NULL,
	[tmp_TemplateID] [bigint] NULL,
	[DiplomaId] [int] NULL,
 CONSTRAINT [PK_GraduationCommissionMember] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[ITLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ITLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[NKRType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_NKRType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[OriginalDiploma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiplomaId] [int] NOT NULL,
	[InstitutionName] [nvarchar](255) NULL,
	[InstitutionTown] [nvarchar](50) NULL,
	[InstitutionMunicipality] [nvarchar](50) NULL,
	[InstitutionDistrict] [nvarchar](50) NULL,
	[InstituionRegion] [nvarchar](50) NULL,
	[Series] [nvarchar](50) NULL,
	[Number] [nvarchar](50) NULL,
	[RegNumber1] [nvarchar](50) NULL,
	[RegNumber2] [nvarchar](50) NULL,
	[RegDate] [date] NULL,
	[EducationType] [nvarchar](100) NULL,
	[Qualification] [nvarchar](100) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_OriginalDiploma] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[PrintTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[InstitutionId] [int] NULL,
	[Name] [nvarchar](1000) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Contents] [varbinary](max) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Left1Margin] [int] NOT NULL,
	[Top1Margin] [int] NOT NULL,
	[Left2Margin] [int] NOT NULL,
	[Top2Margin] [int] NOT NULL,
	[BasicDocumentPrintFormId] [int] NOT NULL,
	[RuoRegId] [int] NULL,
 CONSTRAINT [PK_PrintTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[Template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[Principal] [nvarchar](255) NULL,
	[BasicDocumentId] [int] NOT NULL,
	[InstitutionId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[tmp_DocTemplateID] [bigint] NULL,
	[Contents] [nvarchar](max) NULL,
	[SubjectContents] [nvarchar](max) NULL,
	[Deputy] [nvarchar](255) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CommissionOrderNumber] [nvarchar](50) NULL,
	[CommissionOrderData] [datetime2](7) NULL,
	[BasicClassId] [int] NULL,
	[RuoRegId] [int] NULL,
 CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [document].[TemplateSubject](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[BasicDocumentPartId] [int] NOT NULL,
	[BasicDocumentSubjectId] [int] NULL,
	[SubjectId] [int] NULL,
	[SubjectTypeId] [int] NULL,
	[SubjectName] [nvarchar](255) NULL,
	[Grade] [decimal](5, 2) NULL,
	[GradeText] [nvarchar](100) NULL,
	[Position] [int] NOT NULL,
	[Horarium] [int] NULL,
	[FlSubjectId] [int] NULL,
	[FlHorarium] [int] NULL,
	[NVOPoints] [decimal](7, 3) NULL,
	[FlLevel] [nvarchar](255) NULL,
	[FlSubjectName] [nvarchar](255) NULL,
	[SubjectCanChange] [bit] NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ParentId] [int] NULL,
	[GradeCategory] [int] NOT NULL,
	[BasicClassId] [int] NULL,
 CONSTRAINT [PK_TemplateSubject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_BarcodeYear_Edition_BasicDocumentId_SchoolYear] ON [document].[BarcodeYear]
(
	[Edition] ASC,
	[BasicDocumentId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_BasicDocumentGenerator] ON [document].[BasicDocumentGenerator]
(
	[InstitutionId] ASC,
	[SchoolYear] ASC,
	[BasicDocumentId] ASC,
	[RegNumberTotal] ASC,
	[RegNumberYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BasicDocumentSequence_BasicDocumentId] ON [document].[BasicDocumentSequence]
(
	[BasicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BasicDocumentSequence_SchoolYear] ON [document].[BasicDocumentSequence]
(
	[SchoolYear] ASC
)
INCLUDE ( 	[InstitutionId],
	[BasicDocumentId],
	[RegNumberTotal],
	[RegNumberYear],
	[RegDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BasicDocumentSequence_SchoolYear_BasicDocumentId] ON [document].[BasicDocumentSequence]
(
	[SchoolYear] ASC,
	[BasicDocumentId] ASC
)
INCLUDE ( 	[InstitutionId],
	[RegNumberTotal],
	[RegNumberYear],
	[RegDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_BasicDocumentSequence] ON [document].[BasicDocumentSequence]
(
	[InstitutionId] ASC,
	[SchoolYear] ASC,
	[BasicDocumentId] ASC,
	[RegNumberTotal] ASC,
	[RegNumberYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_BasicDocumentId] ON [document].[Diploma]
(
	[BasicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_InstitutionId] ON [document].[Diploma]
(
	[InstitutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_InstitutionId_SchoolYear] ON [document].[Diploma]
(
	[InstitutionId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_IsFinalized] ON [document].[Diploma]
(
	[IsFinalized] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_OriginalDiplomaId] ON [document].[Diploma]
(
	[OriginalDiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_PersonalId] ON [document].[Diploma]
(
	[PersonalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_PersonId] ON [document].[Diploma]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_RuoRegId_SchoolYear] ON [document].[Diploma]
(
	[RuoRegId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_SchoolYear] ON [document].[Diploma]
(
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Diploma_TemplateId] ON [document].[Diploma]
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaAdditionalDocument_BasicDocumentId] ON [document].[DiplomaAdditionalDocument]
(
	[BasicDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaAdditionalDocument_DiplomaId] ON [document].[DiplomaAdditionalDocument]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaCreateRequest] ON [document].[DiplomaCreateRequest]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaCreateRequest_DiplomaId] ON [document].[DiplomaCreateRequest]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaDocument_DiplomaId] ON [document].[DiplomaDocument]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaSubject_BasicDocumentPartId] ON [document].[DiplomaSubject]
(
	[BasicDocumentPartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaSubject_BasicDocumentSubjectId] ON [document].[DiplomaSubject]
(
	[BasicDocumentSubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaSubject_DiplomaId] ON [document].[DiplomaSubject]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaSubject_SubjectId] ON [document].[DiplomaSubject]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DiplomaSubjectParent] ON [document].[DiplomaSubject]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_GraduationCommissionMember_DiplomaId] ON [document].[GraduationCommissionMember]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_GraduationCommissionMember_TemplateId] ON [document].[GraduationCommissionMember]
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OriginalDiploma_DiplomaId] ON [document].[OriginalDiploma]
(
	[DiplomaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TemplateSubject_ParentId] ON [document].[TemplateSubject]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_TemplateSubject_TemplateId] ON [document].[TemplateSubject]
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
