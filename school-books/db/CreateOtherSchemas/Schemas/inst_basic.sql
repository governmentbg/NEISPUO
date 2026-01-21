SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[Building](
	[BuildingID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NULL,
	[InstitutionDepartmentID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[BuildingTypeID] [int] NULL,
	[ExpectedStudentCount] [int] NULL,
	[StructureCount] [int] NULL,
	[SchoolShiftTypeID] [int] NULL,
	[CadastralCode] [nvarchar](50) NULL,
	[InitialOwnershipDoc] [int] NULL,
	[LatestOwnershipDoc] [int] NULL,
	[LatestRepairYear] [int] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Note] [nvarchar](4000) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_Building] PRIMARY KEY CLUSTERED 
(
	[BuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[BuildingModernizationDegree](
	[BuildingModernizationDegreeID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingID] [int] NOT NULL,
	[ModernizationDegreeID] [int] NOT NULL,
	[Count] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_BuildingModernizationDegree] PRIMARY KEY CLUSTERED 
(
	[BuildingModernizationDegreeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[BuildingRoom](
	[BuildingRoomID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingID] [int] NOT NULL,
	[InstitutionID] [int] NULL,
	[BuildingAreaTypeID] [smallint] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[BuildingRoomTypeID] [int] NOT NULL,
	[MeetRequiredLevel] [int] NULL,
	[Note] [nvarchar](4000) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_BuildingRoom] PRIMARY KEY CLUSTERED 
(
	[BuildingRoomID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[BuildingRoomEquipment](
	[BuildingRoomEquipmentID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingRoomID] [int] NOT NULL,
	[EquipmentTypeID] [int] NOT NULL,
	[Note] [nvarchar](4000) NULL,
	[IsSpecial] [bit] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_BuildingRoomEquipment] PRIMARY KEY CLUSTERED 
(
	[BuildingRoomEquipmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[BuildingSchoolTypeAreaTypeLimit](
	[BuildingSchoolTypeAreaTypeLimitID] [int] IDENTITY(1,1) NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[BuildingAreaTypeID] [int] NOT NULL,
 CONSTRAINT [PK_MeetRequirementsLevelId] PRIMARY KEY CLUSTERED 
(
	[BuildingSchoolTypeAreaTypeLimitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[changeYearClassesFinishedTemp](
	[InstitutionID] [int] NULL,
	[ClassID] [int] NULL,
	[NewSchoolYear] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[changeYearEduStateTemp](
	[EduStateID] [int] NULL,
	[InstitutionID] [int] NULL,
	[PersonID] [int] NULL,
	[NewSchoolYear] [int] NULL,
	[PositionIDBeforeUpdate] [int] NULL,
	[PositionIDAfterUpdate] [int] NULL,
	[Status] [int] NULL,
	[OperationTypeID] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[changeYearJobTemp](
	[Timestamp] [datetime2](7) NULL,
	[RowCountUpdated] [int] NULL,
	[RowCountDeleted] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[changeYearLogTemp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NULL,
	[TimeStamp] [datetime2](7) NULL,
	[OperationID] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[changeYearStudentsFinishedTemp](
	[InstitutionID] [int] NULL,
	[PersonID] [int] NULL,
	[NewSchoolYear] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[CurrentYear](
	[CurrentYearID] [smallint] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[TempCurrentYearID] [smallint] NULL,
	[ChangeYearFrom] [datetime] NULL,
	[ChangeYearTo] [datetime] NULL,
 CONSTRAINT [PK_CurrentYеarID] PRIMARY KEY CLUSTERED 
(
	[CurrentYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[DB_Errors](
	[ErrorID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](100) NULL,
	[ErrorNumber] [int] NULL,
	[ErrorState] [int] NULL,
	[ErrorSeverity] [int] NULL,
	[ErrorLine] [int] NULL,
	[ErrorProcedure] [varchar](max) NULL,
	[OperationType] [int] NULL,
	[ErrorMessage] [varchar](max) NULL,
	[ErrorDateTime] [datetime] NULL,
	[Data] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[DistanceLearningCondition](
	[DistanceLearningConditionID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[Condition] [nvarchar](1000) NULL,
	[CompliedDegree] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_DistanceLearningCondition] PRIMARY KEY CLUSTERED 
(
	[DistanceLearningConditionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionDepartment](
	[InstitutionDepartmentID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CountryID] [int] NOT NULL,
	[TownID] [int] NULL,
	[LocalAreaID] [int] NULL,
	[Address] [nvarchar](255) NULL,
	[PostCode] [int] NULL,
	[IsMain] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[tmp_DepID] [int] NULL,
	[tmp_SchoolYear] [smallint] NULL,
	[IsValid] [bit] NULL,
 CONSTRAINT [PK_InstitutionDepartmentID] PRIMARY KEY CLUSTERED 
(
	[InstitutionDepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionDetail](
	[InstitutionID] [int] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[Website] [nvarchar](50) NULL,
	[EstablishedYear] [nvarchar](255) NULL,
	[ConstitActFirst] [nvarchar](1024) NULL,
	[ConstitActLast] [nvarchar](1024) NULL,
	[IsODZ] [bit] NULL,
	[IsProfSchool] [bit] NULL,
	[IsNational] [bit] NULL,
	[IsProvideEduServ] [bit] NULL,
	[IsDelegateBudget] [bit] NULL,
	[IsNonIndDormitory] [bit] NULL,
	[IsInternContract] [bit] NULL,
	[BankIBAN] [nvarchar](255) NULL,
	[BankBIC] [nvarchar](255) NULL,
	[BankName] [nvarchar](255) NULL,
	[BankAccountHolder] [nvarchar](255) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[IsAppInnovSystem] [bit] NULL,
	[IsProfSchoolWithSpecialties] [bit] NULL,
 CONSTRAINT [PK_InstitutionDetail] PRIMARY KEY CLUSTERED 
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionDetailFL](
	[InstitutionDetailFlID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[InstitutionNameEn] [nvarchar](255) NULL,
	[AddressEn] [nvarchar](255) NULL,
	[FeesScholarshipEn] [nvarchar](255) NULL,
	[EdTypeEnID] [int] NULL,
	[SysUserID] [int] NULL,
	[ValidFrom] [date] NULL,
	[ValidTo] [date] NULL,
	[UpdatedOn] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[InstitutionDetailFlID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionInnovation](
	[InstitutionInnovationID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[InnovationTypeID] [int] NOT NULL,
	[Notes] [nvarchar](1024) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionInnovation] PRIMARY KEY CLUSTERED 
(
	[InstitutionInnovationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionPhone](
	[InstitutionPhoneID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[PhoneTypeID] [int] NOT NULL,
	[PhoneCode] [nvarchar](10) NULL,
	[PhoneNumber] [nvarchar](50) NOT NULL,
	[ContactKind] [nvarchar](255) NOT NULL,
	[IsInstitution] [bit] NULL,
	[IsMain] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_InstitutionPhone] PRIMARY KEY CLUSTERED 
(
	[InstitutionPhoneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionProject](
	[InstitutionProjectID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[ProjectTypeID] [int] NULL,
	[ProjectProgramTypeID] [int] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](3000) NULL,
	[Website] [nvarchar](255) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[PlanPreSchoolCount] [int] NULL,
	[PlanPrimarySchoolCount] [int] NULL,
	[PlanSecondarySchoolCount] [int] NULL,
	[PlanHighSchoolCount] [int] NULL,
	[PlanCountAll] [int] NULL,
	[ActualPreSchoolCount] [int] NULL,
	[ActualPrimarySchoolCount] [int] NULL,
	[ActualSecondarySchoolCount] [int] NULL,
	[ActualHighSchoolCount] [int] NULL,
	[ActualCountAll] [int] NULL,
	[InterPreSchoolCount] [int] NULL,
	[InterPrimarySchoolCount] [int] NULL,
	[InterSecondarySchoolCount] [int] NULL,
	[InterHighSchoolCount] [int] NULL,
	[InterCountAll] [int] NULL,
	[Goals] [nvarchar](max) NULL,
	[IsArchive] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[tmp_ProjID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionProject] PRIMARY KEY CLUSTERED 
(
	[InstitutionProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionProjectPartner](
	[InstitutionProjectPartnerID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectPartnerTypeID] [int] NULL,
	[InstitutionProjectID] [int] NULL,
	[Eik] [nvarchar](50) NULL,
	[Name] [nvarchar](255) NULL,
	[IsCoordinator] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionProjectPartner] PRIMARY KEY CLUSTERED 
(
	[InstitutionProjectPartnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionProjectPriorityArea](
	[InstitutionProjectPriorityAreaID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[ProjectPriorityAreaTypeID] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionProjectPriorityArea] PRIMARY KEY CLUSTERED 
(
	[InstitutionProjectPriorityAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionPublicCouncil](
	[InstitutionPublicCouncilID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[MiddleName] [nvarchar](255) NULL,
	[FamilyName] [nvarchar](255) NOT NULL,
	[PublicCouncilTypeID] [int] NOT NULL,
	[PublicCouncilRoleID] [int] NOT NULL,
	[Email] [nvarchar](150) NULL,
	[PhoneNumber] [nvarchar](150) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionPublicCouncil] PRIMARY KEY CLUSTERED 
(
	[InstitutionPublicCouncilID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionSchoolBoard](
	[InstitutionSchoolBoardID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[MiddleName] [nvarchar](255) NULL,
	[FamilyName] [nvarchar](255) NOT NULL,
	[SchoolBoardRoleID] [int] NOT NULL,
	[Email] [nvarchar](50) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[HelpStudentGovernment] [bit] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionSchoolBoard] PRIMARY KEY CLUSTERED 
(
	[InstitutionSchoolBoardID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[InstitutionVehicle](
	[InstitutionVehicleID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[AcquisitionWayTypeID] [int] NOT NULL,
	[DocumentTypeID] [int] NOT NULL,
	[DocumentNum] [nvarchar](50) NOT NULL,
	[DocumentDate] [date] NOT NULL,
	[RegistrationNum] [nvarchar](50) NOT NULL,
	[ProducedYear] [int] NOT NULL,
	[PlaceCountTypeID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionVehicle] PRIMARY KEY CLUSTERED 
(
	[InstitutionVehicleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[MedicalStaff](
	[ExMedStaffID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[MiddleName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[PersonalIDType] [int] NOT NULL,
	[PersonalID] [nvarchar](255) NOT NULL,
	[NKPDPositionID] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [date] NULL,
	[ValidTo] [date] NULL,
	[UpdatedOn] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ExMedStaffID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonCompSkill](
	[PersonCompSkillID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[CompSkillID] [int] NOT NULL,
	[CompSkillLevelID] [int] NOT NULL,
	[Notes] [nvarchar](1024) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonCompSkill] PRIMARY KEY CLUSTERED 
(
	[PersonCompSkillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonDetail](
	[PersonDetailID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[Title] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](100) NULL,
	[IsExtendStudent] [bit] NULL,
	[IsPensioneer] [bit] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[Email] [nvarchar](100) NULL,
 CONSTRAINT [PK_PersonDetail] PRIMARY KEY CLUSTERED 
(
	[PersonDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_PersonID] UNIQUE NONCLUSTERED 
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonFLSkill](
	[PersonFLSkillID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[FLID] [int] NOT NULL,
	[FLLevelID] [int] NOT NULL,
	[Notes] [nvarchar](1024) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonFLSkill] PRIMARY KEY CLUSTERED 
(
	[PersonFLSkillID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonOKS](
	[PersonOKSID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[EducationGradeTypeID] [int] NULL,
	[SpecialityOrdTypeID] [int] NOT NULL,
	[UniversityID] [int] NULL,
	[UniversityNotes] [nvarchar](1024) NULL,
	[CertifcateNo] [nvarchar](50) NULL,
	[YearOfGraduation] [int] NULL,
	[Speciality] [nvarchar](1024) NULL,
	[AcquiredPK] [nvarchar](1024) NULL,
	[IsPKTeacher] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonOKS] PRIMARY KEY CLUSTERED 
(
	[PersonOKSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonOKSSubjectGroup](
	[PersonOKSSubjectGroupID] [int] IDENTITY(1,1) NOT NULL,
	[PersonOKSID] [int] NOT NULL,
	[SubjectGroupID] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonOKSSubjectGroup] PRIMARY KEY CLUSTERED 
(
	[PersonOKSSubjectGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonPKS](
	[PersonPKSID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[PKSTypeID] [int] NOT NULL,
	[UniversityID] [int] NULL,
	[UniversityNotes] [nvarchar](1024) NULL,
	[CertifcateNo] [nvarchar](50) NULL,
	[YearOfGraduation] [int] NULL,
	[Speciality] [nvarchar](1024) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonPKS] PRIMARY KEY CLUSTERED 
(
	[PersonPKSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[PersonQCourse](
	[PersonQCourseID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[QCourseActTypeID] [int] NULL,
	[QCourseYear] [int] NULL,
	[QCourseTypeID] [int] NOT NULL,
	[QCourseTypeNotes] [nvarchar](255) NULL,
	[QCourseBudgetSourceTypeID] [int] NULL,
	[InternalCoursePrice] [float] NULL,
	[UniversityID] [int] NULL,
	[UniversityNotes] [nvarchar](1024) NULL,
	[QCourseTopic] [nvarchar](255) NULL,
	[QCourseDurationTypeID] [int] NULL,
	[QCourseDurationCredits] [int] NULL,
	[QCourseDurationHours] [decimal](6, 2) NULL,
	[DocumentType] [nvarchar](255) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonQCourse] PRIMARY KEY CLUSTERED 
(
	[PersonQCourseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_basic].[StaffPosition](
	[StaffPositionID] [int] IDENTITY(1,1) NOT NULL,
	[StaffOrd] [int] NULL,
	[PersonID] [int] NOT NULL,
	[InstitutionID] [int] NULL,
	[WorkStartYear] [int] NULL,
	[WorkExpTotalYears] [int] NULL,
	[WorkExpSpecYears] [int] NULL,
	[WorkExpTeachYears] [int] NULL,
	[IsTravel] [bit] NULL,
	[StaffPositionNo] [int] NULL,
	[ContractTypeID] [int] NULL,
	[ContractReasonID] [int] NULL,
	[ContractNo] [nvarchar](50) NULL,
	[ContractYear] [int] NULL,
	[ContractNotes] [nvarchar](1024) NULL,
	[ContractWithID] [int] NULL,
	[IsAccountablePerson] [bit] NULL,
	[StaffTypeID] [int] NULL,
	[CategoryStaffTypeID] [int] NULL,
	[NKPDPositionID] [int] NULL,
	[PositionKindID] [int] NULL,
	[PositionNotes] [nvarchar](1024) NULL,
	[PositionCount] [real] NULL,
	[TeachStageID] [int] NULL,
	[IsHospital] [bit] NULL,
	[IsMentor] [bit] NULL,
	[IsTrainee] [bit] NULL,
	[IsNotMeetReq] [bit] NULL,
	[CurrentlyValid] [bit] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[MainPersonOKSID] [int] NULL,
	[HasTELK] [bit] NULL,
	[IsValid] [bit] NULL,
	[PositionSubjectGroupID] [int] NULL,
	[HelpStudentGovernment] [bit] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[TerminationDate] [date] NULL,
	[TerminationReasonID] [int] NULL,
	[MainClassID] [int] NULL,
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[StaffPositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Building_InstitutionDepartmentID] ON [inst_basic].[Building]
(
	[InstitutionDepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Building_InstitutionID] ON [inst_basic].[Building]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BuildingModernizationDegree_BuildingID] ON [inst_basic].[BuildingModernizationDegree]
(
	[BuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BuildingRoom_BuildingID] ON [inst_basic].[BuildingRoom]
(
	[BuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BuildingRoom_InstitutionID] ON [inst_basic].[BuildingRoom]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BuildingRoomEquipment_BuildingRoomID] ON [inst_basic].[BuildingRoomEquipment]
(
	[BuildingRoomID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_CurrentYear_TempCurrentYearID] ON [inst_basic].[CurrentYear]
(
	[TempCurrentYearID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DistanceLearningCondition_InstitutionID] ON [inst_basic].[DistanceLearningCondition]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionDepartment_InstitutionId] ON [inst_basic].[InstitutionDepartment]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionInnovation_InstitutionID] ON [inst_basic].[InstitutionInnovation]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionPhone_InstitutionDepartmentID] ON [inst_basic].[InstitutionPhone]
(
	[DepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionPhone_InstitutionID] ON [inst_basic].[InstitutionPhone]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionProject_InstitutionID] ON [inst_basic].[InstitutionProject]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionProjectPartner_ProjectID] ON [inst_basic].[InstitutionProjectPartner]
(
	[InstitutionProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionProjectPriorityArea_ProjectID] ON [inst_basic].[InstitutionProjectPriorityArea]
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionPublicCouncil_InstitutionID] ON [inst_basic].[InstitutionPublicCouncil]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionVehicle_InstitutionID] ON [inst_basic].[InstitutionVehicle]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonCompSkill_PersonID] ON [inst_basic].[PersonCompSkill]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonDetail_PersonID] ON [inst_basic].[PersonDetail]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonFLSkill_PersonID] ON [inst_basic].[PersonFLSkill]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonOKS_PersonID] ON [inst_basic].[PersonOKS]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonOKSSubjectGroup_PersonOKSID] ON [inst_basic].[PersonOKSSubjectGroup]
(
	[PersonOKSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonPKS_PersonID] ON [inst_basic].[PersonPKS]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonQCourse_PersonID] ON [inst_basic].[PersonQCourse]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPosition_InstitutionId] ON [inst_basic].[StaffPosition]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPosition_MainClassId] ON [inst_basic].[StaffPosition]
(
	[MainClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPosition_PersonID] ON [inst_basic].[StaffPosition]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPosition_PersonOKSID] ON [inst_basic].[StaffPosition]
(
	[MainPersonOKSID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
