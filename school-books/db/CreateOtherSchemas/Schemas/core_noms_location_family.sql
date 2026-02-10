SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [family].[RelativeHistory](
	[RelativeID] [int] NOT NULL,
	[Notes] [nvarchar](4000) NULL,
	[PersonID] [int] NOT NULL,
	[WorkStatusID] [int] NULL,
	[Description] [nvarchar](2048) NULL,
	[Email] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[EducationTypeId] [int] NULL,
	[tmp_StudentID] [float] NULL,
	[tmp_RelativeType] [int] NULL,
	[FirstName] [nvarchar](255) NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[PersonalID] [nvarchar](255) NULL,
	[PersonalIDType] [int] NULL,
	[RelativeTypeID] [int] NOT NULL,
	[ChildId] [int] NULL,
	[CurrentAddress] [nvarchar](2048) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_RelativeHistory] ON [family].[RelativeHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [family].[Relative](
	[RelativeID] [int] IDENTITY(1,1) NOT NULL,
	[Notes] [nvarchar](4000) NULL,
	[PersonID] [int] NOT NULL,
	[WorkStatusID] [int] NULL,
	[Description] [nvarchar](2048) NULL,
	[Email] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[EducationTypeId] [int] NULL,
	[tmp_StudentID] [float] NULL,
	[tmp_RelativeType] [int] NULL,
	[FirstName] [nvarchar](255) NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[PersonalID] [nvarchar](255) NULL,
	[PersonalIDType] [int] NULL,
	[RelativeTypeID] [int] NOT NULL,
	[ChildId] [int] NULL,
	[CurrentAddress] [nvarchar](2048) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_Relative] PRIMARY KEY CLUSTERED 
(
	[RelativeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [family].[RelativeHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[InstitutionConfDataHistory](
	[InstitutionConfDataID] [int] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[SOVersion] [bigint] NOT NULL,
	[CBVersion] [bigint] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[SOExtProviderID] [int] NULL,
	[CBExtProviderID] [int] NULL,
	[ESExtProviderID] [int] NULL,
	[ASCExtProviderID] [int] NULL,
	[DOCExtProviderID] [int] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_InstitutionConfDataHistory] ON [core].[InstitutionConfDataHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[InstitutionConfData](
	[InstitutionConfDataID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[SOVersion] [bigint] NOT NULL,
	[CBVersion] [bigint] NOT NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[SOExtProviderID] [int] NULL,
	[CBExtProviderID] [int] NULL,
	[ESExtProviderID] [int] NULL,
	[ASCExtProviderID] [int] NULL,
	[DOCExtProviderID] [int] NULL,
 CONSTRAINT [PK_InstitutionConfData] PRIMARY KEY CLUSTERED 
(
	[InstitutionConfDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_InstitutionID] UNIQUE NONCLUSTERED 
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [core].[InstitutionConfDataHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[SysUserHistory](
	[SysUserID] [int] NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[IsAzureUser] [bit] NOT NULL,
	[PersonID] [int] NOT NULL,
	[isAzureSynced] [int] NOT NULL,
	[InitialPassword] [nvarchar](255) NULL,
	[DeletedOn] [datetime2](7) NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_SysUserHistory] ON [core].[SysUserHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[SysUser](
	[SysUserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[IsAzureUser] [bit] NOT NULL,
	[PersonID] [int] NOT NULL,
	[isAzureSynced] [int] NOT NULL,
	[InitialPassword] [nvarchar](255) NULL,
	[DeletedOn] [datetime2](7) NULL,
 CONSTRAINT [PK_SysUser] PRIMARY KEY CLUSTERED 
(
	[SysUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [core].[SysUserHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[PersonHistory](
	[PersonID] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[PermanentAddress] [nvarchar](2048) NULL,
	[PermanentTownID] [int] NULL,
	[CurrentAddress] [nvarchar](2048) NULL,
	[CurrentTownID] [int] NULL,
	[PublicEduNumber] [nvarchar](max) NULL,
	[PersonalIDType] [int] NULL,
	[NationalityID] [int] NULL,
	[PersonalID] [nvarchar](255) NULL,
	[BirthDate] [date] NULL,
	[BirthPlaceTownID] [int] NULL,
	[BirthPlaceCountry] [int] NULL,
	[Gender] [int] NULL,
	[SchoolBooksCodesID] [varchar](10) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[BirthPlace] [nvarchar](255) NULL,
	[AzureID] [varchar](100) NULL,
	[SysUserType] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_PersonHistory] ON [core].[PersonHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[Person](
	[PersonID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[PermanentAddress] [nvarchar](2048) NULL,
	[PermanentTownID] [int] NULL,
	[CurrentAddress] [nvarchar](2048) NULL,
	[CurrentTownID] [int] NULL,
	[PublicEduNumber] [nvarchar](max) NULL,
	[PersonalIDType] [int] NULL,
	[NationalityID] [int] NULL,
	[PersonalID] [nvarchar](255) NULL,
	[BirthDate] [date] NULL,
	[BirthPlaceTownID] [int] NULL,
	[BirthPlaceCountry] [int] NULL,
	[Gender] [int] NULL,
	[SchoolBooksCodesID] [varchar](10) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[BirthPlace] [nvarchar](255) NULL,
	[AzureID] [varchar](100) NULL,
	[SysUserType] [int] NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [core].[PersonHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[Certificate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[NotAfter] [datetime2](7) NOT NULL,
	[NotBefore] [datetime2](7) NOT NULL,
	[SerialNumber] [nvarchar](100) NOT NULL,
	[Thumbprint] [nvarchar](255) NOT NULL,
	[Issuer] [nvarchar](255) NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Contents] [varbinary](max) NOT NULL,
	[CertificateType] [int] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[Description] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Certificate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[EducationalState](
	[EducationalStateID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NULL,
	[InstitutionID] [int] NULL,
	[PositionID] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_EducationalState] PRIMARY KEY CLUSTERED 
(
	[EducationalStateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_EducationalState_PersonId_InstitutionId_PositionId] UNIQUE NONCLUSTERED 
(
	[PersonID] ASC,
	[InstitutionID] ASC,
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ExtSystem](
	[ExtSystemID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[IsValid] [bit] NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_ExtSystem] PRIMARY KEY CLUSTERED 
(
	[ExtSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ExtSystemAccess](
	[ExtSystemAccessID] [int] IDENTITY(1,1) NOT NULL,
	[ExtSystemID] [int] NOT NULL,
	[ExtSystemType] [int] NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_ExtSystemAccess] PRIMARY KEY CLUSTERED 
(
	[ExtSystemAccessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ExtSystemCertificate](
	[ExtSystemID] [int] NOT NULL,
	[Thumbprint] [nvarchar](255) NOT NULL,
	[NotAfter] [datetime2](7) NOT NULL,
	[NotBefore] [datetime2](7) NOT NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_ExtSystemThumbprint] PRIMARY KEY CLUSTERED 
(
	[Thumbprint] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ExtSystemServices](
	[ExtServiceID] [int] IDENTITY(1,1) NOT NULL,
	[ServiceName] [nvarchar](255) NOT NULL,
	[ServiceParameters] [nvarchar](255) NULL,
	[ProcedureName] [nvarchar](max) NOT NULL,
	[IsReturnArray] [bit] NULL,
	[ApiCallQuery] [nvarchar](max) NULL,
	[IsValid] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ExtSystemServices] PRIMARY KEY CLUSTERED 
(
	[ExtServiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ExtSystemServicesMap](
	[ExtServiceMapID] [int] IDENTITY(1,1) NOT NULL,
	[ExtServiceID] [int] NOT NULL,
	[ExtSystemID] [int] NOT NULL,
 CONSTRAINT [PK_ExtSystemServicesMap] PRIMARY KEY CLUSTERED 
(
	[ExtServiceMapID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[Institution](
	[InstitutionID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Abbreviation] [nvarchar](255) NOT NULL,
	[Bulstat] [nvarchar](13) NULL,
	[CountryID] [int] NULL,
	[LocalAreaID] [int] NULL,
	[FinancialSchoolTypeID] [int] NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[BudgetingSchoolTypeID] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[TownID] [int] NULL,
	[BaseSchoolTypeID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_InsitutionID] PRIMARY KEY CLUSTERED 
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
CREATE TABLE [core].[InstitutionExtSystem](
	[InstitutionExtSystemID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[ExtSystemID] [int] NOT NULL,
	[ExtSystemTypeID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_InstitutionExtSystem] PRIMARY KEY CLUSTERED 
(
	[InstitutionExtSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[InstitutionSchoolYear](
	[InstitutionId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[IsFinalized] [bit] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Abbreviation] [nvarchar](255) NOT NULL,
	[Bulstat] [nvarchar](13) NULL,
	[CountryID] [int] NULL,
	[LocalAreaID] [int] NULL,
	[FinancialSchoolTypeID] [int] NOT NULL,
	[DetailedSchoolTypeID] [int] NOT NULL,
	[BudgetingSchoolTypeID] [int] NOT NULL,
	[TownID] [int] NULL,
	[BaseSchoolTypeID] [int] NOT NULL,
	[IsCurrent] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_InstitutionSchoolYear] PRIMARY KEY CLUSTERED 
(
	[InstitutionId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[ParentChildSchoolBookAccess](
	[ParentChildSchoolBookAccessID] [int] IDENTITY(1,1) NOT NULL,
	[ChildID] [int] NOT NULL,
	[ParentID] [int] NOT NULL,
	[HasAccess] [int] NULL,
 CONSTRAINT [PK_ParentChildSchoolBookAccess] PRIMARY KEY CLUSTERED 
(
	[ParentChildSchoolBookAccessID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [ChildIDParentIDUniqueConstraint] UNIQUE NONCLUSTERED 
(
	[ParentID] ASC,
	[ChildID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[Position](
	[PositionID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysRoleID] [int] NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[RelativeChild](
	[RelativeID] [int] NOT NULL,
	[ChildID] [int] NOT NULL,
	[RelativeTypeID] [int] NOT NULL,
 CONSTRAINT [PK_RelativeChild] PRIMARY KEY CLUSTERED 
(
	[RelativeID] ASC,
	[ChildID] ASC,
	[RelativeTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[RelativeHistory](
	[RelativeID] [int] NOT NULL,
	[Notes] [nvarchar](4000) NULL,
	[PersonID] [int] NULL,
	[WorkStatusID] [int] NULL,
	[Description] [nvarchar](2048) NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[Email] [nvarchar](255) NULL,
	[PhoneNumber] [nvarchar](255) NULL,
	[EducationTypeId] [int] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_RelativeHistory] ON [core].[RelativeHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[SysRole](
	[SysRoleID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
 CONSTRAINT [PK_SysRole] PRIMARY KEY CLUSTERED 
(
	[SysRoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_SysRole] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[SystemUserMessage](
	[SystemUserMessageID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](255) NULL,
	[Content] [nvarchar](max) NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[Roles] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [core].[SysUserSysRole](
	[SysUserID] [int] NOT NULL,
	[SysRoleID] [int] NOT NULL,
	[InstitutionID] [int] NULL,
	[BudgetingInstitutionID] [int] NULL,
	[MunicipalityID] [int] NULL,
	[RegionID] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[BaseSchoolType](
	[BaseSchoolTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_BaseSchoolType] PRIMARY KEY CLUSTERED 
(
	[BaseSchoolTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[BudgetingInstitution](
	[BudgetingInstitutionID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_BudgetingInstitution] PRIMARY KEY CLUSTERED 
(
	[BudgetingInstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[Currency](
	[Code] [varchar](3) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[ExchangeRate] [decimal](9, 6) NULL,
	[IsMain] [bit] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_Currencies] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[DetailedSchoolType](
	[DetailedSchoolTypeID] [int] NOT NULL,
	[BaseSchoolTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[InstType] [int] NULL,
 CONSTRAINT [PK_DetailedSchoolType] PRIMARY KEY CLUSTERED 
(
	[DetailedSchoolTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[ExtSystemType](
	[ExtSystemTypeID] [int] NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[IsValid] [bit] NOT NULL,
	[InstitutionCheck] [bit] NULL,
 CONSTRAINT [PK_ExtSystemAccess] PRIMARY KEY CLUSTERED 
(
	[ExtSystemTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[FinancialSchoolType](
	[FinancialSchoolTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_FinancialSchoolType] PRIMARY KEY CLUSTERED 
(
	[FinancialSchoolTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[Gender](
	[GenderID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_Gender] PRIMARY KEY CLUSTERED 
(
	[GenderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [noms].[PersonalIDType](
	[PersonalIDTypeID] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_PersonalIDType] PRIMARY KEY CLUSTERED 
(
	[PersonalIDTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [location].[Country](
	[CountryID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Code] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [location].[LocalArea](
	[LocalAreaID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[TownCode] [int] NULL,
 CONSTRAINT [PK_LocalArea] PRIMARY KEY CLUSTERED 
(
	[LocalAreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [location].[Municipality](
	[MunicipalityID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Code] [nvarchar](1024) NOT NULL,
	[RegionID] [int] NULL,
	[Description] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Municipality] PRIMARY KEY CLUSTERED 
(
	[MunicipalityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [location].[Region](
	[RegionID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Code] [nvarchar](1024) NOT NULL,
	[Description] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [location].[Town](
	[TownID] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Code] [int] NOT NULL,
	[Type] [nvarchar](1024) NOT NULL,
	[Category] [int] NOT NULL,
	[Longtitude] [decimal](5, 2) NULL,
	[Latitude] [decimal](5, 2) NULL,
	[MunicipalityID] [int] NULL,
	[Description] [nvarchar](2048) NULL,
 CONSTRAINT [PK_Town] PRIMARY KEY CLUSTERED 
(
	[TownID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [family].[EducationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[Weight] [int] NULL,
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
CREATE TABLE [family].[RelativeType](
	[RelativeTypeID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
 CONSTRAINT [PK_RelativeType] PRIMARY KEY CLUSTERED 
(
	[RelativeTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [family].[WorkStatus](
	[WorkStatusID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Description] [nvarchar](2048) NOT NULL,
	[Weight] [int] NULL,
 CONSTRAINT [PK_WorkStatus] PRIMARY KEY CLUSTERED 
(
	[WorkStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EducationalState_InstitutionId] ON [core].[EducationalState]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EducationalState_PersonId] ON [core].[EducationalState]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EducationalState_PositionId] ON [core].[EducationalState]
(
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionConfData_CBExtProviderID] ON [core].[InstitutionConfData]
(
	[CBExtProviderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionConfData_InstitutionId] ON [core].[InstitutionConfData]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionConfData_InstitutionId_SchoolYear] ON [core].[InstitutionConfData]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_Person_AzureID] ON [core].[Person]
(
	[AzureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_Person_PersonalID] ON [core].[Person]
(
	[PersonalID] ASC
)
WHERE ([PersonalID] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_SysUser_Username] ON [core].[SysUser]
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_SysUser_Username_DeletedOn] ON [core].[SysUser]
(
	[Username] ASC,
	[DeletedOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [SysUserPersonID] ON [core].[SysUser]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [uqSysUserUsername] ON [core].[SysUser]
(
	[Username] ASC
)
WHERE ([DeletedOn] IS NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SysUserSysRole_SysUserID] ON [core].[SysUserSysRole]
(
	[SysUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Relative_PersonID] ON [family].[Relative]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
