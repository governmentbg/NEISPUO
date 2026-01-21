SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[AcquisitionWayType](
	[AcquisitionWayTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_AcquisitionWayType] PRIMARY KEY CLUSTERED 
(
	[AcquisitionWayTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BasicClass](
	[BasicClassID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[RomeName] [nvarchar](255) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[SortOrd] [int] NULL,
 CONSTRAINT [PK_BasicClass] PRIMARY KEY CLUSTERED 
(
	[BasicClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BasicClassLimit](
	[BasicClassLimitID] [int] IDENTITY(1,1) NOT NULL,
	[BasicClassID] [int] NOT NULL,
	[DetailedSchoolTypeID] [int] NULL,
	[InstType] [int] NOT NULL,
	[ClassKind] [int] NOT NULL,
 CONSTRAINT [PK_BasicClassLimit] PRIMARY KEY CLUSTERED 
(
	[BasicClassLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BasicEducationArea](
	[BasicEducationAreaID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_BasicEducationArea] PRIMARY KEY CLUSTERED 
(
	[BasicEducationAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BasicSubjectType](
	[BasicSubjectTypeID] [smallint] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Abrev] [nvarchar](15) NULL,
	[IsValid] [bit] NULL,
 CONSTRAINT [PK_BasicSubjectType] PRIMARY KEY CLUSTERED 
(
	[BasicSubjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BCCDOClassLimit](
	[BCClassCDOLimitID] [int] IDENTITY(1,1) NOT NULL,
	[BCClassID] [int] NOT NULL,
	[BCStudentID] [int] NOT NULL,
 CONSTRAINT [BCClassCDOLimitID] PRIMARY KEY CLUSTERED 
(
	[BCClassCDOLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BudgetingClassLimit](
	[BudgetingClassLimitID] [int] IDENTITY(1,1) NOT NULL,
	[BudgetingSchoolID] [int] NOT NULL,
	[BudgetingClassID] [int] NOT NULL,
 CONSTRAINT [PK_BudgetingClassLimit] PRIMARY KEY CLUSTERED 
(
	[BudgetingClassLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BuildingAreaType](
	[BuildingAreaTypeID] [smallint] NOT NULL,
	[BuildingAreaTypeName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[BuildingTypeID] [int] NULL,
	[BuildingAreaKind] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_BuildingAreaType] PRIMARY KEY CLUSTERED 
(
	[BuildingAreaTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BuildingRoomType](
	[BuildingRoomTypeID] [int] NOT NULL,
	[BuildingRoomTypeName] [nvarchar](1024) NOT NULL,
	[BuildingAreaTypeID] [smallint] NOT NULL,
	[IsSupportiveEnv] [bit] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_BuildingRoomType] PRIMARY KEY CLUSTERED 
(
	[BuildingRoomTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[BuildingType](
	[BuildingTypeID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingTypeName] [nvarchar](255) NOT NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_BuildingType] PRIMARY KEY CLUSTERED 
(
	[BuildingTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CampaignData](
	[CampaignDataID] [int] IDENTITY(1,1) NOT NULL,
	[InstType] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Period] [smallint] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[IsOpen] [bit] NULL,
	[IsLast] [bit] NULL,
 CONSTRAINT [PK_CampaignData] PRIMARY KEY CLUSTERED 
(
	[CampaignDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CategoryStaffDetailedSchoolTypeLimit](
	[CategoryStaffDetailedSchoolTypeLimitID] [int] IDENTITY(1,1) NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[CategoryStaffTypeID] [int] NOT NULL,
 CONSTRAINT [PK_CategoryStaffDetailedSchoolTypeLimit] PRIMARY KEY CLUSTERED 
(
	[CategoryStaffDetailedSchoolTypeLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CategoryStaffType](
	[CategoryStaffTypeID] [int] NOT NULL,
	[StaffTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_CategoryStaffType] PRIMARY KEY CLUSTERED 
(
	[CategoryStaffTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ChangeYearStatus](
	[ChangeYearStatusID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsFinal] [bit] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
 CONSTRAINT [PK_ChangeYearStatus] PRIMARY KEY CLUSTERED 
(
	[ChangeYearStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassEduDuration](
	[ClassEduDurationID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_ClassEduDuration] PRIMARY KEY CLUSTERED 
(
	[ClassEduDurationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassKind](
	[ClassKindID] [int] NOT NULL,
	[Name] [nchar](20) NULL,
 CONSTRAINT [PK_ClassKind] PRIMARY KEY CLUSTERED 
(
	[ClassKindID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassShift](
	[ClassShiftID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_ClassShift] PRIMARY KEY CLUSTERED 
(
	[ClassShiftID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassShiftLimit](
	[ClassShiftLimitID] [int] IDENTITY(1,1) NOT NULL,
	[InstTypeID] [int] NOT NULL,
	[ClassShiftID] [int] NULL,
 CONSTRAINT [PK_ClassShiftLimit] PRIMARY KEY CLUSTERED 
(
	[ClassShiftLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassType](
	[ClassTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[ClassKind] [int] NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
 CONSTRAINT [PK_ClassType] PRIMARY KEY CLUSTERED 
(
	[ClassTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ClassTypeLimit](
	[ClassTypeLimitID] [int] IDENTITY(1,1) NOT NULL,
	[ClassTypeID] [int] NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[InstType] [int] NOT NULL,
	[ClassKind] [int] NOT NULL,
	[BasicClassID] [int] NOT NULL,
 CONSTRAINT [PK_ClassTypeLimitID] PRIMARY KEY CLUSTERED 
(
	[ClassTypeLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CompSkill](
	[CompSkillID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_CompSkill] PRIMARY KEY CLUSTERED 
(
	[CompSkillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CompSkillLevel](
	[CompSkillLevelID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_CompSkillLevel] PRIMARY KEY CLUSTERED 
(
	[CompSkillLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ContractReason](
	[ContractReasonID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ContractReason] PRIMARY KEY CLUSTERED 
(
	[ContractReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ContractType](
	[ContractTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ContractType] PRIMARY KEY CLUSTERED 
(
	[ContractTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ContractWith](
	[ContractWithID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ContractWith] PRIMARY KEY CLUSTERED 
(
	[ContractWithID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ContractWithLimit](
	[ContractWithLimitID] [int] IDENTITY(1,1) NOT NULL,
	[InstType] [int] NOT NULL,
	[ContractWithID] [int] NOT NULL,
 CONSTRAINT [PK_ContractWithLimitID] PRIMARY KEY CLUSTERED 
(
	[ContractWithLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CurriculumCatalog](
	[CurriculumCatalogID] [int] NOT NULL,
	[CatalogNo] [int] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SortOrder] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_CurriculumCatalogID] PRIMARY KEY CLUSTERED 
(
	[CurriculumCatalogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CurriculumCatalogLimit](
	[CurriculumLimitID] [int] IDENTITY(1,1) NOT NULL,
	[CurriculumCatalogID] [int] NOT NULL,
	[ClassTypeID] [int] NOT NULL,
	[BasicClassID] [int] NOT NULL,
	[FLTypeID] [int] NOT NULL,
	[ClassEduFormID] [int] NULL,
	[DetailedSchoolTypeID] [int] NULL,
 CONSTRAINT [PK_CurriculumLimitID] PRIMARY KEY CLUSTERED 
(
	[CurriculumLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CurriculumLibrary](
	[CurriculumLibraryID] [int] IDENTITY(1,1) NOT NULL,
	[CurriculumCatalogID] [int] NOT NULL,
	[BasicClassID] [int] NOT NULL,
	[SubjectID] [int] NOT NULL,
	[SubjectTypeID] [int] NOT NULL,
	[WeeksFirstTerm] [smallint] NULL,
	[HoursWeeklyFirstTerm] [real] NULL,
	[WeeksSecondTerm] [smallint] NULL,
	[HoursWeeklySecondTerm] [real] NULL,
	[NormaS] [int] NULL,
	[SortOrder] [int] NULL,
	[tmp_SubjectID_orig] [int] NULL,
 CONSTRAINT [PK_CurriculumLibraryID] PRIMARY KEY CLUSTERED 
(
	[CurriculumLibraryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CurriculumPart](
	[CurriculumPartID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_CurriculumPart] PRIMARY KEY CLUSTERED 
(
	[CurriculumPartID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CustomVar](
	[CustomVarID] [int] NOT NULL,
	[CustomVarName] [nvarchar](255) NOT NULL,
	[IsAppendable] [bit] NOT NULL,
	[Add1Nom] [nvarchar](255) NULL,
	[Add2Nom] [nvarchar](255) NULL,
 CONSTRAINT [PK_CustomVar] PRIMARY KEY CLUSTERED 
(
	[CustomVarID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CustomVarValue](
	[CustomVarValueID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[CustomVarID] [int] NOT NULL,
	[CustomVarValue] [int] NULL,
	[CustomVarValueAdd1] [int] NULL,
	[CustomVarValueAdd2] [int] NULL,
	[IsValid] [bit] NULL,
	[CustomVarValueAdd3] [int] NULL,
 CONSTRAINT [PK_CustomVarValue] PRIMARY KEY CLUSTERED 
(
	[CustomVarValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[CustomVarValueTemp](
	[CustomVarValueID] [int] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[CustomVarID] [int] NOT NULL,
	[CustomVarValue] [int] NULL,
	[CustomVarValueAdd1] [int] NULL,
	[CustomVarValueAdd2] [int] NULL,
	[IsValid] [bit] NULL,
	[CustomVarValueAdd3] [int] NULL,
 CONSTRAINT [PK_CustomVarValueTemp] PRIMARY KEY CLUSTERED 
(
	[CustomVarValueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[DBVersion](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VersionName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[UpgradeStart] [date] NULL,
	[UpgradeEnd] [date] NULL,
	[TimeStamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_DBVersion] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Name] UNIQUE NONCLUSTERED 
(
	[VersionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_Version] UNIQUE NONCLUSTERED 
(
	[VersionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EdTypeFL](
	[EdTypeEnID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](25) NULL,
	[DescriptionEn] [nvarchar](25) NULL,
	[IsValid] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[EdTypeEnID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EducationArea](
	[EducationAreaID] [int] NOT NULL,
	[BasicEducationAreaID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_EducationArea] PRIMARY KEY CLUSTERED 
(
	[EducationAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EducationGradeType](
	[EducationGradeTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[SortOrd] [int] NULL,
 CONSTRAINT [PK_EducationGradeType] PRIMARY KEY CLUSTERED 
(
	[EducationGradeTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EduForm](
	[ClassEduFormID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[ValidForClass] [bit] NULL,
	[ValidforStudent] [bit] NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[IsNotPresentForm] [bit] NULL,
	[ValidforDiploma] [bit] NULL,
	[CanChoose] [bit] NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
	[NameShort] [nvarchar](255) NULL,
 CONSTRAINT [PK_ClassEduForm] PRIMARY KEY CLUSTERED 
(
	[ClassEduFormID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EduFormLimit](
	[EduFormLimitID] [int] IDENTITY(1,1) NOT NULL,
	[EduFormID] [int] NOT NULL,
	[InstType] [int] NOT NULL,
	[ClassKind] [int] NULL,
 CONSTRAINT [PK_EduFormLimit] PRIMARY KEY CLUSTERED 
(
	[EduFormLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EntranceLevel](
	[EntranceLevelID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_EntranceLevel] PRIMARY KEY CLUSTERED 
(
	[EntranceLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EntranceLevelLimit](
	[EntranceLevelLimitID] [int] IDENTITY(1,1) NOT NULL,
	[EntranceLevelID] [int] NOT NULL,
	[BasicClassID] [int] NOT NULL,
	[ClassTypeID] [int] NOT NULL,
 CONSTRAINT [PK_EntranceLevelLimit] PRIMARY KEY CLUSTERED 
(
	[EntranceLevelLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[EquipmentType](
	[EquipmentTypeId] [int] NOT NULL,
	[EquipmentTypeName] [nvarchar](1024) NOT NULL,
	[IsSpecial] [bit] NULL,
	[BuildingRoomTypeID] [int] NOT NULL,
	[IsSupportiveEnv] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_EquipmentType] PRIMARY KEY CLUSTERED 
(
	[EquipmentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[FL](
	[FLID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK__FL] PRIMARY KEY CLUSTERED 
(
	[FLID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[FLLevel](
	[FLLevelID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__FLLevel] PRIMARY KEY CLUSTERED 
(
	[FLLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[FLStudyType](
	[FLStudyTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_FLStudyType] PRIMARY KEY CLUSTERED 
(
	[FLStudyTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[InnovationType](
	[InnovationTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_InnovationType] PRIMARY KEY CLUSTERED 
(
	[InnovationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[InstAdminData](
	[InstAdminDataID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [int] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[InstitutionDepartmentID] [int] NULL,
	[IsInnovative] [bit] NOT NULL,
	[IsCentral] [bit] NOT NULL,
	[IsProtected] [bit] NOT NULL,
	[IsStateFunded] [bit] NOT NULL,
	[HasMunDecisionFor4] [bit] NOT NULL,
 CONSTRAINT [PK_InstAdminData] PRIMARY KEY CLUSTERED 
(
	[InstAdminDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[InstType](
	[InstTypeID] [int] NOT NULL,
	[Name] [nchar](10) NULL,
 CONSTRAINT [PK_InstType] PRIMARY KEY CLUSTERED 
(
	[InstTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[IscedCode](
	[SPPOOProfessionCode] [int] NOT NULL,
	[IscedCodeId] [int] NOT NULL,
	[IsValid] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[SPPOOProfessionCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[MainSubjectLimit](
	[MainSubjectLimitID] [int] IDENTITY(1,1) NOT NULL,
	[MainSubjectID] [int] NOT NULL,
	[MainSubjectName] [nvarchar](255) NOT NULL,
	[IsValid] [bit] NULL,
 CONSTRAINT [PK_MainSubjectLimitID] PRIMARY KEY CLUSTERED 
(
	[MainSubjectLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[MeetRequirementsLevel](
	[MeetRequirementsLevelId] [int] NOT NULL,
	[MeetRequirementsLevelName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_MeetRequirementsLevelId] PRIMARY KEY CLUSTERED 
(
	[MeetRequirementsLevelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ModernizationDegree](
	[ModernizationDegreeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsSupportiveEnv] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ModernizationDegree] PRIMARY KEY CLUSTERED 
(
	[ModernizationDegreeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[Municipality4year](
	[MunID] [int] NOT NULL,
	[Year] [smallint] NOT NULL,
 CONSTRAINT [PK_tMunicipality4year] PRIMARY KEY CLUSTERED 
(
	[MunID] ASC,
	[Year] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDClass](
	[NKPDClassID] [int] NOT NULL,
	[Code] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Descrition] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDClass] PRIMARY KEY CLUSTERED 
(
	[NKPDClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDCodeMON](
	[OccMONID] [int] NOT NULL,
	[OccMONName] [nvarchar](255) NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_OccMONID] PRIMARY KEY CLUSTERED 
(
	[OccMONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDGroup](
	[NKPDGroupID] [int] NOT NULL,
	[NKPDSubClassID] [int] NOT NULL,
	[Code] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDGroup] PRIMARY KEY CLUSTERED 
(
	[NKPDGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDPosition](
	[NKPDPositionID] [int] NOT NULL,
	[NKPDSubGroupID] [int] NULL,
	[Code] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[StaffTypeID] [int] NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDPosition] PRIMARY KEY CLUSTERED 
(
	[NKPDPositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDPositionCodeMON](
	[NKPDPositionCodeMONID] [int] IDENTITY(1,1) NOT NULL,
	[OccMONID] [int] NOT NULL,
	[NKPDPositionID] [int] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDPositionCodeMONID] PRIMARY KEY CLUSTERED 
(
	[NKPDPositionCodeMONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDPositionMainClassLimit](
	[NKPDPositionMainClassID] [int] IDENTITY(1,1) NOT NULL,
	[NKPDPositionID] [int] NOT NULL,
 CONSTRAINT [PK_NKPDPositionMainClassID] PRIMARY KEY CLUSTERED 
(
	[NKPDPositionMainClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDSubClass](
	[NKPDSubClassID] [int] NOT NULL,
	[NKPDClassID] [int] NOT NULL,
	[Code] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Descrition] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDSubClass] PRIMARY KEY CLUSTERED 
(
	[NKPDSubClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[NKPDSubGroup](
	[NKPDSubGroupID] [int] NOT NULL,
	[NKPDGroupID] [int] NOT NULL,
	[Code] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Descrition] [nvarchar](2048) NULL,
	[StaffTypeID] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_NKPDSubGroup] PRIMARY KEY CLUSTERED 
(
	[NKPDSubGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PhoneType](
	[PhoneTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_PhoneType] PRIMARY KEY CLUSTERED 
(
	[PhoneTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PKSType](
	[PKSTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__PKSType] PRIMARY KEY CLUSTERED 
(
	[PKSTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PlaceCountType](
	[PlaceCountTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_PlaceCountType] PRIMARY KEY CLUSTERED 
(
	[PlaceCountTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PositionKind](
	[PositionKindID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_PositionKind] PRIMARY KEY CLUSTERED 
(
	[PositionKindID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProfGroupMON](
	[ProfGroupMONID] [int] NOT NULL,
	[ProfGroupMONName] [nvarchar](250) NULL,
 CONSTRAINT [PK_Code ProfMON] PRIMARY KEY CLUSTERED 
(
	[ProfGroupMONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProfileModuleSubjectLimit](
	[ProfileModuleSubjectID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [int] NOT NULL,
	[ProfileSubjectID] [int] NOT NULL,
	[ModuleSubjectID] [int] NOT NULL,
	[FLTypeID] [smallint] NULL,
 CONSTRAINT [PK_ProfileIDs] PRIMARY KEY CLUSTERED 
(
	[ProfileModuleSubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProfSubjectLimit](
	[ProfSubjectLimitID] [int] IDENTITY(1,1) NOT NULL,
	[ProfSubjectTypeID] [int] NOT NULL,
	[ClassTypeID] [int] NULL,
	[ProfSubjectID] [int] NOT NULL,
 CONSTRAINT [PK_ProfSubjectLimit] PRIMARY KEY CLUSTERED 
(
	[ProfSubjectLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProfSubjectType](
	[ProfSubjectTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ProfSubjectType] PRIMARY KEY CLUSTERED 
(
	[ProfSubjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProjectPartnerType](
	[ProjectPartnerTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ProjectPartnerType] PRIMARY KEY CLUSTERED 
(
	[ProjectPartnerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProjectPriorityAreaType](
	[ProjectPriorityAreaTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ProjectPriorityAreaType] PRIMARY KEY CLUSTERED 
(
	[ProjectPriorityAreaTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProjectProgramType](
	[ProjectProgramTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ProjectProgramType] PRIMARY KEY CLUSTERED 
(
	[ProjectProgramTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[ProjectType](
	[ProjectTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_ProjectType] PRIMARY KEY CLUSTERED 
(
	[ProjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PublicCouncilRole](
	[PublicCouncilRoleID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_PublicCouncilRole] PRIMARY KEY CLUSTERED 
(
	[PublicCouncilRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[PublicCouncilType](
	[PublicCouncilTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_PublicCouncilType] PRIMARY KEY CLUSTERED 
(
	[PublicCouncilTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[QCourseActType](
	[QCourseActTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__QCourseActType] PRIMARY KEY CLUSTERED 
(
	[QCourseActTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[QCourseBudgetSourceType](
	[QCourseBudgetSourceTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[QCourseActTypeID] [int] NOT NULL,
	[IsAddPrice] [bit] NOT NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__QCourseBudgetSourceType] PRIMARY KEY CLUSTERED 
(
	[QCourseBudgetSourceTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[QCourseDurationType](
	[QCourseDurationTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[QCourseActTypeID] [int] NOT NULL,
	[SortOrd] [int] NOT NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__QCourseDurationType] PRIMARY KEY CLUSTERED 
(
	[QCourseDurationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[QCourseType](
	[QCourseTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[QCourseActTypeID] [int] NOT NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK__QCourseType] PRIMARY KEY CLUSTERED 
(
	[QCourseTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SchoolBasicProfile](
	[SchoolBasicProfileID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SchoolBasicProfile] PRIMARY KEY CLUSTERED 
(
	[SchoolBasicProfileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SchoolBoardRole](
	[SchoolBoardRoleID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SchoolBoardRole] PRIMARY KEY CLUSTERED 
(
	[SchoolBoardRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SchoolProfile](
	[SchoolProfileID] [int] NOT NULL,
	[SchoolBasicProfileID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SchoolProfile] PRIMARY KEY CLUSTERED 
(
	[SchoolProfileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SchoolShiftType](
	[SchoolShiftTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_SchoolShiftType] PRIMARY KEY CLUSTERED 
(
	[SchoolShiftTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SpecialityOrdType](
	[SpecialityOrdTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_SpecialityOrdType] PRIMARY KEY CLUSTERED 
(
	[SpecialityOrdTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SPPOOEducArea](
	[SPPOOEducAreaID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SPPOOEducAreaCode] [nvarchar](10) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_SPPOOEducArea] PRIMARY KEY CLUSTERED 
(
	[SPPOOEducAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SPPOOProfArea](
	[SPPOOProfAreaID] [int] NOT NULL,
	[EducAreaID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SPPOOProfAreaCode] [nvarchar](10) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_SPPOOProfArea] PRIMARY KEY CLUSTERED 
(
	[SPPOOProfAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SPPOOProfession](
	[SPPOOProfessionID] [int] NOT NULL,
	[ProfAreaID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SPPOOProfessionCode] [nvarchar](10) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
	[ProfGroupMONID] [int] NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
 CONSTRAINT [PK_SPPOOProfession] PRIMARY KEY CLUSTERED 
(
	[SPPOOProfessionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SPPOOSpeciality](
	[SPPOOSpecialityID] [int] NOT NULL,
	[ProfessionID] [int] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[VETLevel] [int] NULL,
	[SPPOOSpecialityCode] [nvarchar](10) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
 CONSTRAINT [PK_SPPOOSpeciality] PRIMARY KEY CLUSTERED 
(
	[SPPOOSpecialityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SPPOOSpecialityDetails](
	[SpecialityDetailsID] [int] IDENTITY(1,1) NOT NULL,
	[SpecialityID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[IsProtected] [bit] NULL,
	[IsPriority] [bit] NULL,
 CONSTRAINT [PK_SPPOOSpecialityDetails] PRIMARY KEY CLUSTERED 
(
	[SpecialityDetailsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[StaffType](
	[StaffTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_StaffType] PRIMARY KEY CLUSTERED 
(
	[StaffTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[Subject](
	[SubjectID] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](255) NULL,
	[SubjectNameShort] [nvarchar](255) NULL,
	[IsValid] [bit] NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
	[IsMandatory] [bit] NOT NULL,
 CONSTRAINT [PK_Subjects] PRIMARY KEY CLUSTERED 
(
	[SubjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SubjectGroup](
	[SubjectGroupID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SortOrd] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[NormaBasic] [int] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_SubjectGroup] PRIMARY KEY CLUSTERED 
(
	[SubjectGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SubjectType](
	[SubjectTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[BasicSubjectTypeID] [smallint] NOT NULL,
	[PartID] [int] NULL,
	[SortOrd] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SubjectType] PRIMARY KEY CLUSTERED 
(
	[SubjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SubmissionEventType](
	[SubmissionEventTypeID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SubmissionEventType] PRIMARY KEY CLUSTERED 
(
	[SubmissionEventTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SubmissionStatus](
	[SubmissionStatusID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsFinal] [bit] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[TreeColour] [nvarchar](50) NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
 CONSTRAINT [PK_SubmissionStatus] PRIMARY KEY CLUSTERED 
(
	[SubmissionStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[SubmissionUserType](
	[SubmissionUserTypeID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_SubmissionUserType] PRIMARY KEY CLUSTERED 
(
	[SubmissionUserTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[TeachStage](
	[TeachStageID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[CanChoose] [bit] NULL,
 CONSTRAINT [PK_TeachStage] PRIMARY KEY CLUSTERED 
(
	[TeachStageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[TeachStageLimit](
	[TeachStageLimitID] [int] IDENTITY(1,1) NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[TeachStageID] [int] NOT NULL,
 CONSTRAINT [PK_TeachStageLimit] PRIMARY KEY CLUSTERED 
(
	[TeachStageLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[TerminationReason](
	[TerminationReasonID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_TerminationReasonID] PRIMARY KEY CLUSTERED 
(
	[TerminationReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[TrustBoardRegistryStatus](
	[TrustBoardRegistryStatusID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_TrustBoardRegistryStatus] PRIMARY KEY CLUSTERED 
(
	[TrustBoardRegistryStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[University](
	[UniversityID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[TownID] [int] NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[SortOrd] [int] NULL,
 CONSTRAINT [PK_University] PRIMARY KEY CLUSTERED 
(
	[UniversityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_nom].[VehicleDocumentType](
	[VehicleDocumentTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
 CONSTRAINT [PK_VehicleDocumentType] PRIMARY KEY CLUSTERED 
(
	[VehicleDocumentTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CustomVarValue_ID_Value] ON [inst_nom].[CustomVarValue]
(
	[CustomVarID] ASC,
	[CustomVarValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionID_CustomVarID] ON [inst_nom].[CustomVarValue]
(
	[InstitutionID] ASC,
	[CustomVarID] ASC
)
INCLUDE ( 	[CustomVarValue],
	[CustomVarValueAdd1],
	[CustomVarValueAdd2],
	[IsValid],
	[CustomVarValueAdd3]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstAdminData_InstitutionID] ON [inst_nom].[InstAdminData]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
