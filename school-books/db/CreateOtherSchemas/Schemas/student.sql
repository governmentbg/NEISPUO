SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SchoolTypeLodAccessHistory](
	[Id] [int] NOT NULL,
	[DetailedSchoolTypeId] [int] NOT NULL,
	[isLodAccessAllowed] [bit] NOT NULL,
	[ValidFrom] [datetime2](0) NOT NULL,
	[ValidTo] [datetime2](0) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_SchoolTypeLodAccessHistory] ON [student].[SchoolTypeLodAccessHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SchoolTypeLodAccess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DetailedSchoolTypeId] [int] NOT NULL,
	[isLodAccessAllowed] [bit] NOT NULL,
	[ValidFrom] [datetime2](0) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](0) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_SchoolTypeLodAccessId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[SchoolTypeLodAccessHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportReportlHistory](
	[Id] [int] NOT NULL,
	[ReportNumber] [nvarchar](100) NOT NULL,
	[ReportDate] [date] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_ResourceSupportReportlHistory] ON [student].[ResourceSupportReportlHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportReport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReportNumber] [nvarchar](100) NOT NULL,
	[ReportDate] [date] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_ResourceSupportReport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[ResourceSupportReportlHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipStudentTemporalHistory](
	[Id] [int] NOT NULL,
	[StudentClassID] [int] NULL,
	[ScholarshipAmountID] [int] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [date] NULL,
	[Description] [nvarchar](1000) NULL,
	[ScholarshipTypeId] [int] NOT NULL,
	[AmountRate] [decimal](6, 2) NOT NULL,
	[Periodicity] [int] NULL,
	[OrderNumber] [nvarchar](100) NULL,
	[OrderDate] [date] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionId] [int] NULL,
	[FinancingOrganId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CommissionDate] [date] NULL,
	[CommissionNumber] [nvarchar](200) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [int] NULL,
	[StartSysTime] [datetime2](7) NOT NULL,
	[EndSysTime] [datetime2](7) NOT NULL,
	[Currency] [varchar](3) NOT NULL,
	[AltCurrency] [varchar](3) NOT NULL,
	[AltAmountRate] [numeric](20, 10) NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_ScholarshipStudentTemporalHistory] ON [student].[ScholarshipStudentTemporalHistory]
(
	[EndSysTime] ASC,
	[StartSysTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipStudent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentClassID] [int] NULL,
	[ScholarshipAmountID] [int] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [date] NULL,
	[Description] [nvarchar](1000) NULL,
	[ScholarshipTypeId] [int] NOT NULL,
	[AmountRate] [decimal](6, 2) NOT NULL,
	[Periodicity] [int] NULL,
	[OrderNumber] [nvarchar](100) NULL,
	[OrderDate] [date] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionId] [int] NULL,
	[FinancingOrganId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CommissionDate] [date] NULL,
	[CommissionNumber] [nvarchar](200) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [int] NULL,
	[StartSysTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[EndSysTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[Currency] [varchar](3) NOT NULL,
	[AltCurrency]  AS (case when [Currency]='BGN' then 'EUR' else 'BGN' end) PERSISTED NOT NULL,
	[AltAmountRate]  AS (case when [Currency]='BGN' then round([AmountRate]/(1.955830),(6)) else round([AmountRate]/(0.511292),(6)) end) PERSISTED,
 CONSTRAINT [PK_ScholarshipStudent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([StartSysTime], [EndSysTime])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[ScholarshipStudentTemporalHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsYearTemporalHistory](
	[Id] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[HasSuportiveEnvironment] [bit] NOT NULL,
	[SupportiveEnvironment] [nvarchar](1000) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_SpecialNeedsYearTemporalHistory] ON [student].[SpecialNeedsYearTemporalHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsYear](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[HasSuportiveEnvironment] [bit] NOT NULL,
	[SupportiveEnvironment] [nvarchar](1000) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_SpecialNeedsYear] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[SpecialNeedsYearTemporalHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsTemporalHistory](
	[Id] [int] NOT NULL,
	[SpecialNeedsYearId] [int] NOT NULL,
	[SpecialNeedsTypeId] [int] NOT NULL,
	[SpecialNeedsSubTypeId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_SpecialNeedsTemporalHistory] ON [student].[SpecialNeedsTemporalHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeeds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SpecialNeedsYearId] [int] NOT NULL,
	[SpecialNeedsTypeId] [int] NOT NULL,
	[SpecialNeedsSubTypeId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_SpecialNeeds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_SpecialNeedsYear] UNIQUE NONCLUSTERED 
(
	[SpecialNeedsYearId] ASC,
	[SpecialNeedsTypeId] ASC,
	[SpecialNeedsSubTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[SpecialNeedsTemporalHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AppSettingsHistory](
	[Key] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ValidFrom] [datetime2](0) NOT NULL,
	[ValidTo] [datetime2](0) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_AppSettingsHistory] ON [student].[AppSettingsHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AppSettings](
	[Key] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[ValidFrom] [datetime2](0) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](0) GENERATED ALWAYS AS ROW END NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[AppSettingsHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LODFinalizationHistory](
	[Id] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[IsFinalized] [bit] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[FinalizationDate] [datetime2](7) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsApproved] [bit] NOT NULL,
	[ApprovalDate] [datetime2](7) NULL,
	[StudentClassId] [int] NULL,
	[DocumentId] [int] NULL,
	[InstitutionId] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_LODFinalizationHistory] ON [student].[LODFinalizationHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LODFinalization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[IsFinalized] [bit] NOT NULL,
	[Remarks] [nvarchar](max) NULL,
	[FinalizationDate] [datetime2](7) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsApproved] [bit] NOT NULL,
	[ApprovalDate] [datetime2](7) NULL,
	[StudentClassId] [int] NULL,
	[DocumentId] [int] NULL,
	[InstitutionId] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_LODFinalization] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_PersonId_SchoolYear] UNIQUE NONCLUSTERED 
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[LODFinalizationHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[StudentClassTemporalHistory](
	[ID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ClassId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[StudentSpecialityId] [int] NULL,
	[StudentEduFormId] [int] NOT NULL,
	[ClassNumber] [int] NULL,
	[Status] [int] NOT NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[IsHourlyOrganization] [bit] NULL,
	[IsForSubmissionToNRA] [bit] NULL,
	[IsCurrent] [bit] NOT NULL,
	[RepeaterId] [int] NOT NULL,
	[CommuterTypeId] [int] NULL,
	[HasSuportiveEnvironment] [bit] NULL,
	[SupportiveEnvironment] [nvarchar](1000) NULL,
	[EnrollmentDate] [datetime2](7) NOT NULL,
	[AdmissionDocumentId] [int] NULL,
	[PositionId] [int] NOT NULL,
	[BasicClassId] [int] NOT NULL,
	[ClassTypeId] [int] NOT NULL,
	[newClassId] [int] NULL,
	[OldClassId] [int] NULL,
	[FromStudentClassId] [int] NULL,
	[DischargeReasonId] [int] NULL,
	[DischargeDocumentId] [int] NULL,
	[RelocationDocumentId] [int] NULL,
	[ORESTypeId] [int] NULL,
	[IsFTACOutsourced] [bit] NULL,
	[InstitutionId] [int] NOT NULL,
	[IsNotPresentForm] [bit] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ToDeleteByAdmin] [bit] NOT NULL,
	[EntryDate] [date] NULL,
	[DischargeDate] [date] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_StudentClassTemporalHistory] ON [student].[StudentClassTemporalHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[StudentClass](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ClassId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[StudentSpecialityId] [int] NULL,
	[StudentEduFormId] [int] NOT NULL,
	[ClassNumber] [int] NULL,
	[Status] [int] NOT NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[IsHourlyOrganization] [bit] NULL,
	[IsForSubmissionToNRA] [bit] NULL,
	[IsCurrent] [bit] NOT NULL,
	[RepeaterId] [int] NOT NULL,
	[CommuterTypeId] [int] NULL,
	[HasSuportiveEnvironment] [bit] NULL,
	[SupportiveEnvironment] [nvarchar](1000) NULL,
	[EnrollmentDate] [datetime2](7) NOT NULL,
	[AdmissionDocumentId] [int] NULL,
	[PositionId] [int] NOT NULL,
	[BasicClassId] [int] NOT NULL,
	[ClassTypeId] [int] NOT NULL,
	[newClassId] [int] NULL,
	[OldClassId] [int] NULL,
	[FromStudentClassId] [int] NULL,
	[DischargeReasonId] [int] NULL,
	[DischargeDocumentId] [int] NULL,
	[RelocationDocumentId] [int] NULL,
	[ORESTypeId] [int] NULL,
	[IsFTACOutsourced] [bit] NULL,
	[InstitutionId] [int] NOT NULL,
	[IsNotPresentForm] [bit] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ToDeleteByAdmin] [bit] NOT NULL,
	[EntryDate] [date] NULL,
	[DischargeDate] [date] NULL,
 CONSTRAINT [PK_StudentClass] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[StudentClassTemporalHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportHistory](
	[Id] [int] NOT NULL,
	[ResourceSupportTypeId] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[ResourceSupportReportId] [int] NOT NULL,
	[AdditionalPersonalDevelopmentSupportItemId] [int] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_ResourceSupportHistory] ON [student].[ResourceSupportHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceSupportTypeId] [int] NOT NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[ResourceSupportReportId] [int] NOT NULL,
	[AdditionalPersonalDevelopmentSupportItemId] [int] NULL,
 CONSTRAINT [PK_ResourceSupport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[ResourceSupportHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportSpecialistHistory](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[OrganizationType] [nvarchar](200) NULL,
	[OrganizationName] [nvarchar](200) NULL,
	[SpecialistType] [nvarchar](200) NULL,
	[ResourceSupportSpecialistTypeId] [int] NOT NULL,
	[ResourceSupportId] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[WorkPlaceId] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[WeeklyHours] [real] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_ResourceSupportSpecialistHistory] ON [student].[ResourceSupportSpecialistHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportSpecialist](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[OrganizationType] [nvarchar](200) NULL,
	[OrganizationName] [nvarchar](200) NULL,
	[SpecialistType] [nvarchar](200) NULL,
	[ResourceSupportSpecialistTypeId] [int] NOT NULL,
	[ResourceSupportId] [int] NOT NULL,
	[SysUserID] [int] NULL,
	[WorkPlaceId] [int] NOT NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[WeeklyHours] [real] NULL,
 CONSTRAINT [PK_ResourceSupportSpecialist] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[ResourceSupportSpecialistHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SelfGovernmentTemporalHistory](
	[Id] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NULL,
	[ParticipationId] [int] NOT NULL,
	[PositionId] [int] NOT NULL,
	[ParticipationAdditionalInformation] [nvarchar](2048) NULL,
	[AdditionalInformation] [nvarchar](2048) NULL,
	[InstitutionId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[InstitutionSchoolBoardId] [int] NULL,
	[StaffPositionID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[StudentClassId] [int] NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ix_SelfGovernmentTemporalHistory] ON [student].[SelfGovernmentTemporalHistory]
(
	[ValidTo] ASC,
	[ValidFrom] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SelfGovernment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NULL,
	[ParticipationId] [int] NOT NULL,
	[PositionId] [int] NOT NULL,
	[ParticipationAdditionalInformation] [nvarchar](2048) NULL,
	[AdditionalInformation] [nvarchar](2048) NULL,
	[InstitutionId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[InstitutionSchoolBoardId] [int] NULL,
	[StaffPositionID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[StudentClassId] [int] NULL,
 CONSTRAINT [PK_SelfGovernment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [student].[SelfGovernmentTemporalHistory] )
)
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Absence](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[Excused] [decimal](5, 2) NOT NULL,
	[Unexcused] [decimal](5, 2) NOT NULL,
	[AbsenceImportId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[BasicClassId] [int] NULL,
	[InstitutionId] [int] NOT NULL,
	[ClassId] [int] NULL,
 CONSTRAINT [PK_Absence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_PersonId_SchoolYear_Month_InstitutionId] UNIQUE NONCLUSTERED 
(
	[PersonId] ASC,
	[SchoolYear] ASC,
	[Month] ASC,
	[InstitutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AbsenceASPFlatFile](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NULL,
	[PersonalId] [nvarchar](20) NULL,
	[PersonalIdType] [int] NULL,
	[FirstName] [varchar](25) NULL,
	[MiddleName] [varchar](25) NULL,
	[LastName] [varchar](25) NULL,
	[EduFormId] [int] NULL,
	[BasicClassId] [int] NULL,
	[ASPStatus] [int] NULL,
	[version] [nvarchar](8) NULL,
	[StudentEduFormId] [int] NULL,
	[ClassTypeId] [int] NULL,
	[ClassId] [int] NULL,
	[StudentClassId] [int] NULL,
	[EnrollmentDate] [datetime2](7) NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AbsenceCampaign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](2048) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[FromDate] [date] NOT NULL,
	[ToDate] [date] NOT NULL,
	[IsManuallyActivated] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_AbsenceCampaign_SchoolYear_Month] UNIQUE NONCLUSTERED 
(
	[SchoolYear] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AbsenceExport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[BlobId] [int] NULL,
	[RecordsCount] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsFinalized] [bit] NOT NULL,
	[FinalizedDate] [datetime2](7) NULL,
	[IsSigned] [bit] NOT NULL,
	[SignedDate] [datetime2](7) NULL,
	[Signature] [nvarchar](max) NULL,
	[ZpCount] [int] NULL,
	[OresCount] [int] NULL,
	[ConfCount] [int] NULL,
 CONSTRAINT [PK_AbsenceExport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AbsenceHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AbsenceId] [int] NOT NULL,
	[Excused] [decimal](5, 2) NOT NULL,
	[Unexcused] [decimal](5, 2) NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_AbsenceHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AbsenceImport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[BlobId] [int] NULL,
	[RecordsCount] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ImportType] [tinyint] NULL,
	[IsFinalized] [bit] NOT NULL,
	[FinalizedDate] [datetime2](7) NULL,
	[IsSigned] [bit] NOT NULL,
	[SignedDate] [datetime2](7) NULL,
	[Signature] [nvarchar](max) NULL,
	[IsWithoutAbsences] [bit] NULL,
 CONSTRAINT [PK_AbsenceImport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdditionalPersonalDevelopmentSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PeriodTypeId] [int] NOT NULL,
	[FinalSchoolYear] [smallint] NULL,
	[StudentTypeId] [int] NULL,
	[OrderNumber] [nvarchar](100) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[IsSuspended] [bit] NOT NULL,
	[SuspensionDate] [datetime2](7) NULL,
	[SuspensionReason] [nvarchar](4000) NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_AdditionalPersonalDevelopmentSupport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdditionalPersonalDevelopmentSupportAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdditionalPersonalDevelopmentSupportId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_AdditionalPersonalDevelopmentSupportAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdditionalPersonalDevelopmentSupportItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdditionalPersonalDevelopmentSupportId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[Details] [nvarchar](4000) NULL,
	[IsSuspended] [bit] NOT NULL,
	[SuspensionDate] [datetime2](7) NULL,
	[SuspensionReason] [nvarchar](4000) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_AdditionalPersonalDevelopmentSupportItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdditionalPersonalDevelopmentSupportItemAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdditionalPersonalDevelopmentSupportItemId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_AdditionalPersonalDevelopmentSupportItemAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdditionalSupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_AdditionalSupportType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdmissionDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdmissionDate] [date] NOT NULL,
	[NoteNumber] [nvarchar](20) NOT NULL,
	[NoteDate] [date] NOT NULL,
	[DischargeDate] [date] NULL,
	[AdmissionReasonTypeId] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[RelocationDocumentId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[PositionId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ConfirmationDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[HasHealthStatusDocument] [bit] NOT NULL,
	[HasImmunizationStatusDocument] [bit] NOT NULL,
	[CurrentStudentClassId] [int] NULL,
 CONSTRAINT [PK_AdmissionDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdmissionDocumentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdmissionDocumentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_AdmissionDocumentDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdmissionPermissionRequest](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[RequestingInstitutionId] [int] NOT NULL,
	[RequestingInstitutionSchoolYear] [smallint] NOT NULL,
	[FirstEducationalStateId] [int] NOT NULL,
	[AuthorizingInstitutionId] [int] NOT NULL,
	[AuthorizingInstitutionSchoolYear] [smallint] NOT NULL,
	[SecondEducationalStateId] [int] NULL,
	[AdmissionDocumentId] [int] NULL,
	[DischargeDocumentId] [int] NULL,
	[Note] [nvarchar](2048) NULL,
	[IsPermissionGranted] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[RelocationDocumentId] [int] NULL,
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
CREATE TABLE [student].[AdmissionPermissionRequestAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdmissionPermissionRequestId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_AdmissionPermissionRequestAttachmentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AdmissionReasonType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_AdmissionReasonType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AppSettingsForTenant](
	[InstitutionId] [int] NOT NULL,
	[Key] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[InstitutionId] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ASP_CONF_Wrong](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalId] [varchar](10) NOT NULL,
	[PersonalIdType] [int] NOT NULL,
	[OriginalInstitutionId] [int] NOT NULL,
	[ReasonForDelete] [varchar](255) NULL,
	[Month] [int] NULL,
	[SchoolYear] [int] NULL,
	[ASP2MON_Session_Info] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AspAskingTemp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TARGET_MONTH] [datetime] NOT NULL,
	[ID_NUMBER] [varchar](10) NOT NULL,
	[ID_TYPE] [varchar](1) NOT NULL,
	[FLAG_OSN] [varchar](1) NULL,
	[FLAG_KOR] [varchar](1) NULL,
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
CREATE TABLE [student].[ASPEnrolledStudentsExport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NULL,
	[BlobId] [int] NOT NULL,
	[RecordsCount] [int] NOT NULL,
	[FileType] [int] NOT NULL,
	[Contents] [nvarchar](max) NULL,
	[ContentsDiff] [nvarchar](max) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ASPEnrolledStudentsExport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ASPMonthlyBenefit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[AbsenceCount] [decimal](5, 2) NOT NULL,
	[ASPStatus] [smallint] NOT NULL,
	[DaysCount] [smallint] NOT NULL,
	[OnlineEnvironmentDays] [smallint] NULL,
	[InstitutionId] [int] NOT NULL,
	[MONStatus] [smallint] NOT NULL,
	[AbsenceCorrection] [decimal](5, 2) NULL,
	[DaysCorrection] [smallint] NULL,
	[Reason] [nvarchar](2000) NULL,
	[ASPMonthlyBenefitsImportId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[CurrentInstitutionId] [int] NULL,
	[StatusASPName]  AS (case [ASPStatus] when (0) then N'Отсъствие' when (1) then N'Отписан' when (2) then N'ОРЕС' else N'' end) PERSISTED NOT NULL,
	[StatusNEISPUOName]  AS (case [MONStatus] when (0) then N'За преглед' when (1) then N'Потвърждавам (прекратяването на помощите/брой дни в ОРЕС)' when (2) then N'Отказвам, няма основания' else N'' end) PERSISTED NOT NULL,
 CONSTRAINT [PK_ASPMonthlyBenefit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ASPMonthlyBenefitInstitution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ASPMonthlyBenefitImportId] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[IsSigned] [bit] NOT NULL,
	[SignedDate] [datetime2](7) NULL,
	[Signature] [nvarchar](max) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ASPMonthlyBenefitInstitution] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ASPMonthlyBenefitsImport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[ImportedBlobId] [int] NULL,
	[ExportedBlobId] [int] NULL,
	[ExportedDate] [datetime2](7) NULL,
	[RecordsCount] [int] NOT NULL,
	[ExportedBySysUserID] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[FileStatusCheck] [int] NOT NULL,
	[ImportCompleted] [bit] NOT NULL,
	[ImportFileMessages] [nvarchar](max) NULL,
	[AspSessionNo] [int] NULL,
	[MonSessionNo] [int] NULL,
 CONSTRAINT [PK_ASPMonthlyBenefitsImport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ASPMonthlyBenefitsImport_SchoolYear_Month] UNIQUE NONCLUSTERED 
(
	[SchoolYear] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AspSubmittedDataHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[PersonalId] [varchar](10) NOT NULL,
	[PersonalIdType] [int] NOT NULL,
	[FirstName] [nvarchar](25) NOT NULL,
	[MiddleName] [nvarchar](25) NULL,
	[LastName] [nvarchar](25) NOT NULL,
	[EduFormId] [smallint] NULL,
	[BasicClassId] [smallint] NULL,
	[ASPStatus] [tinyint] NOT NULL,
	[Year] [smallint] NULL,
	[Month] [smallint] NOT NULL,
	[ExportTypeCode] [varchar](10) NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
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
CREATE TABLE [student].[AspZpTemp](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[TargetMonth] [datetime2](7) NOT NULL,
	[PersonalId] [nvarchar](255) NOT NULL,
	[PersonalIdTypeId] [nvarchar](1) NOT NULL,
	[PersonId] [int] NULL,
	[FirstName] [nvarchar](255) NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[InstitutionId] [int] NULL,
	[BasicClassId] [int] NULL,
	[EduFormId] [int] NULL,
	[Status] [int] NULL,
	[EnrollmentDate] [datetime2](7) NULL,
	[DischargeDate] [datetime2](7) NULL,
	[IsCorrection] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AuditEntries](
	[AuditEntryID] [int] IDENTITY(1,1) NOT NULL,
	[EntitySetName] [nvarchar](255) NULL,
	[EntityTypeName] [nvarchar](255) NULL,
	[State] [int] NOT NULL,
	[StateName] [nvarchar](255) NULL,
	[Ip] [nvarchar](40) NULL,
	[UserAgent] [nvarchar](255) NULL,
	[CreatedBySysUserID] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[InstitutionId] [int] NULL,
	[CorrelationId] [nvarchar](36) NULL,
	[Lease] [int] NULL,
	[Description] [nvarchar](1000) NULL,
 CONSTRAINT [PK_Student.AuditEntries] PRIMARY KEY CLUSTERED 
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AuditEntryProperties](
	[AuditEntryPropertyID] [int] IDENTITY(1,1) NOT NULL,
	[AuditEntryID] [int] NOT NULL,
	[RelationName] [nvarchar](255) NULL,
	[PropertyName] [nvarchar](255) NULL,
	[OldValue] [nvarchar](max) NULL,
	[NewValue] [nvarchar](max) NULL,
	[IsKey] [bit] NOT NULL,
 CONSTRAINT [PK_Student.AuditEntryProperties] PRIMARY KEY CLUSTERED 
(
	[AuditEntryPropertyID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AvailableArchitecture](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModernizationDegreeId] [int] NOT NULL,
	[StudentClassID] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_AvailableArchitecture] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Award](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[OrderNumber] [nvarchar](255) NULL,
	[PersonId] [int] NOT NULL,
	[AwardTypeId] [int] NOT NULL,
	[AwardCategoryId] [int] NOT NULL,
	[FounderId] [int] NOT NULL,
	[AwardReasonId] [int] NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[AdditionalInformation] [nvarchar](2048) NULL,
	[InstitutionId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_Award] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AwardCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_AwardCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AwardDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AwardId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_AwardDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AwardReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_AwardReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[AwardTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_AwardTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[BuildingArea](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuildingAreaTypeId] [smallint] NOT NULL,
	[StudentClassID] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_BuildingArea] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[BuildingRoom](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BuildingRoomTypeID] [int] NOT NULL,
	[StudentClassID] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_BuildingRoom] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[CommonPersonalDevelopmentSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_CommonPersonalDevelopmentSupport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_CommonPersonalDevelopmentSupport_Person_SchoolYear] UNIQUE NONCLUSTERED 
(
	[SchoolYear] ASC,
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[CommonPersonalDevelopmentSupportAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommonPersonalDevelopmentSupportId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_OCommonPersonalDevelopmentSupportAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[CommonPersonalDevelopmentSupportItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CommonPersonalDevelopmentSupportId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[Details] [nvarchar](4000) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OldId] [int] NULL,
 CONSTRAINT [PK_CommonPersonalDevelopmentSupportItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[CommonSupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_CommonSupportType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[CommuterType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_CommuterType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ContextualInformation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModuleName] [nvarchar](50) NOT NULL,
	[Key] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](1000) NULL,
	[Description] [nvarchar](4000) NULL,
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
CREATE TABLE [student].[DischargeDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NoteNumber] [nvarchar](20) NOT NULL,
	[NoteDate] [date] NOT NULL,
	[DischargeReasonTypeId] [int] NOT NULL,
	[InstitutionId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[CurrentStudentClassId] [int] NULL,
	[DischargeDate] [date] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[Status] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ConfirmationDate] [datetime2](7) NULL,
 CONSTRAINT [PK_DischargeDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[DischargeDocumentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DischargeDocumentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_DischargeDocumentDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[DischargeReasonType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[IsForDischarge] [bit] NOT NULL,
	[IsForRelocation] [bit] NOT NULL,
	[IsForInternalEnrollment] [bit] NOT NULL,
 CONSTRAINT [PK_DischargeReasonType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Document](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Rowguid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[FileName] [nvarchar](255) NULL,
	[ContentType] [nvarchar](255) NULL,
	[BlobId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Rowguid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[EarlyEvaluationReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_EarlyEvaluationReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Equalization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[ReasonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[InClass] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_Equalization] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[EqualizationAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EqualizationId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_EqualizationAttachmentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[EqualizationDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EqualizationId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[Position] [int] NOT NULL,
	[BasicClassId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[GradeCategory] [int] NOT NULL,
	[Grade] [decimal](5, 2) NULL,
	[SpecialNeedsGrade] [int] NULL,
	[OtherGrade] [int] NULL,
	[SubjectTypeId] [int] NULL,
	[Term] [int] NULL,
	[Horarium] [int] NULL,
	[SessionId] [int] NULL,
 CONSTRAINT [PK_EqualizationDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ExternalEvaluation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[ExternalEvaluationTypeId] [int] NOT NULL,
	[ParentId] [int] NULL,
	[Description] [nvarchar](2048) NULL,
	[SchoolYear] [smallint] NULL,
	[IsRepeat] [bit] NOT NULL,
	[ApplicationDate] [date] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK__External__3214EC07D5A90156] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ExternalEvaluationItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalEvaluationId] [int] NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Points] [decimal](7, 2) NOT NULL,
	[StrEvaluation] [nvarchar](255) NULL,
	[FisrtTermEvaluaton] [nvarchar](255) NULL,
	[SecondTermEvaluaton] [nvarchar](255) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OriginalPoints] [decimal](7, 2) NOT NULL,
	[Grade] [decimal](5, 2) NULL,
	[SubjectId] [int] NOT NULL,
	[Description] [nvarchar](400) NULL,
	[SubjectTypeId] [int] NULL,
	[FLLevel] [nvarchar](50) NULL,
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
CREATE TABLE [student].[ExternalEvaluationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[BasicClassId] [int] NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsUnofficial] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_ExternalEvaluationType_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[FirstToThirdClassGrade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_FirstToThirdClassGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Founder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_Founder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Grade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[IsSpecialGrade] [bit] NOT NULL,
 CONSTRAINT [PK_Grade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[GradeCategory](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_GradeCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[GradeNom](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[GradeTypeId] [int] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SchoolBookGradeId] [tinyint] NULL,
	[Sort] [tinyint] NULL,
 CONSTRAINT [PK_GradeNom] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[GradeType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_GradeType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[GRAOCampaign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[ValidFrom] [date] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[CampaignNumber] [int] NOT NULL,
 CONSTRAINT [PK_GRAOCampaign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[GRAOPerson](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GRAOCampaignID] [int] NOT NULL,
	[PersonalID] [nvarchar](255) NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[MiddleName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthPlaceCountry] [nvarchar](255) NULL,
	[BirthPlaceCountryCode] [nvarchar](10) NULL,
	[PARegion] [nvarchar](25) NULL,
	[PARegionCode] [nvarchar](2) NULL,
	[PAMunicipality] [nvarchar](25) NULL,
	[PAMunicipalityCode] [nvarchar](2) NULL,
	[PADistrictCode] [nvarchar](2) NULL,
	[PATown] [nvarchar](50) NULL,
	[PATownCode] [nvarchar](5) NULL,
	[PAEntity] [nvarchar](30) NULL,
	[PAEntityCode] [nvarchar](5) NULL,
	[PANumber] [nvarchar](10) NULL,
	[PAEntrance] [nvarchar](5) NULL,
	[PAFloor] [nvarchar](5) NULL,
	[PAApartment] [nvarchar](30) NULL,
	[PALastChange] [date] NULL,
	[CARegion] [nvarchar](25) NULL,
	[CARegionCode] [nvarchar](2) NULL,
	[CAMunicipality] [nvarchar](25) NULL,
	[CAMunicipalityCode] [nvarchar](2) NULL,
	[CADistrictCode] [nvarchar](2) NULL,
	[CATown] [nvarchar](50) NULL,
	[CATownCode] [nvarchar](5) NULL,
	[CAEntity] [nvarchar](30) NULL,
	[CAEntityCode] [nvarchar](5) NULL,
	[CANumber] [nvarchar](10) NULL,
	[CAEntrance] [nvarchar](5) NULL,
	[CAFloor] [nvarchar](5) NULL,
	[CAApartment] [nvarchar](30) NULL,
	[CALastChange] [date] NULL,
	[Status] [int] NOT NULL,
	[FatherPersonalID] [nvarchar](255) NULL,
	[MotherPersonalID] [nvarchar](255) NULL,
	[Sibling1PersonalID] [nvarchar](255) NULL,
	[Sibling2PersonalID] [nvarchar](255) NULL,
	[Sibling3PersonalID] [nvarchar](255) NULL,
	[Sibling4PersonalID] [nvarchar](255) NULL,
	[Sibling5PersonalID] [nvarchar](255) NULL,
	[Sibling6PersonalID] [nvarchar](255) NULL,
	[Sibling7PersonalID] [nvarchar](255) NULL,
	[Sibling8PersonalID] [nvarchar](255) NULL,
	[Sibling9PersonalID] [nvarchar](255) NULL,
	[Sibling10PersonalID] [nvarchar](255) NULL,
	[Sibling11PersonalID] [nvarchar](255) NULL,
	[Sibling12PersonalID] [nvarchar](255) NULL,
	[Sibling13PersonalID] [nvarchar](255) NULL,
	[Sibling14PersonalID] [nvarchar](255) NULL,
	[Sibling15PersonalID] [nvarchar](255) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_GRAOPerson] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[HealthInsuranceExport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Year] [smallint] NOT NULL,
	[Month] [smallint] NOT NULL,
	[BlobId] [int] NULL,
	[RecordsCount] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear]  AS (case when [Month]<=(8) then [Year]-(1) else [Year] end),
 CONSTRAINT [PK_HealthInsuranceExport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[HealthInsuranceIncomeRate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MinimalInsuranceIncome] [decimal](10, 2) NOT NULL,
	[MinimaInsuranceIncomePercentage] [tinyint] NOT NULL,
	[MinimalInsuranceIncomeRate]  AS (CONVERT([decimal](10,2),([MinimalInsuranceIncome]*[MinimaInsuranceIncomePercentage])/(100))),
	[InsuranceContributionPercentage] [tinyint] NOT NULL,
	[InsuranceRate]  AS (CONVERT([decimal](10,2),(CONVERT([decimal](10,2),([MinimalInsuranceIncome]*[MinimaInsuranceIncomePercentage])/(100))*[InsuranceContributionPercentage])/(100))),
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[Currency] [varchar](3) NOT NULL,
	[AltCurrency]  AS (case when [Currency]='BGN' then 'EUR' else 'BGN' end) PERSISTED NOT NULL,
	[AltMinimalInsuranceIncome]  AS (case when [Currency]='BGN' then CONVERT([decimal](10,2),round([MinimalInsuranceIncome]/(1.955830),(2))) else CONVERT([decimal](10,2),round([MinimalInsuranceIncome]/(0.511292),(2))) end) PERSISTED,
	[AltMinimalInsuranceIncomeRate]  AS (case when [Currency]='BGN' then CONVERT([decimal](10,2),(round([MinimalInsuranceIncome]/(1.955830),(2))*[MinimaInsuranceIncomePercentage])/(100)) else CONVERT([decimal](10,2),(round([MinimalInsuranceIncome]/(0.511292),(2))*[MinimaInsuranceIncomePercentage])/(100)) end) PERSISTED,
	[AltInsuranceRate]  AS (case when [Currency]='BGN' then CONVERT([decimal](10,2),(CONVERT([decimal](10,2),(round([MinimalInsuranceIncome]/(1.955830),(2))*[MinimaInsuranceIncomePercentage])/(100))*[InsuranceContributionPercentage])/(100)) else CONVERT([decimal](10,2),(CONVERT([decimal](10,2),(round([MinimalInsuranceIncome]/(0.511292),(2))*[MinimaInsuranceIncomePercentage])/(100))*[InsuranceContributionPercentage])/(100)) end) PERSISTED,
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
CREATE TABLE [student].[InstitutionChange](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Version] [datetime2](7) NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_InstitutionChanges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[InternationalMobility](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Project] [nvarchar](1000) NOT NULL,
	[ReceivingInstitution] [nvarchar](1000) NOT NULL,
	[MainObjectives] [nvarchar](2000) NOT NULL,
	[PersonId] [int] NOT NULL,
	[FromDate] [datetime2](7) NULL,
	[ToDate] [datetime2](7) NULL,
	[CountryId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[InstitutionId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_InternationalMobility] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[InternationalMobilityDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InternationalMobilityId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_InternationalMobilityDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[InternationalProtection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocNumber] [nvarchar](50) NULL,
	[DocDate] [date] NULL,
	[ValidFrom] [date] NULL,
	[ValidTo] [date] NULL,
	[PersonId] [int] NOT NULL,
	[ProtectionStatus] [int] NOT NULL,
 CONSTRAINT [PK_InternationalProtection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Language](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LeadTeacher](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StaffPositionId] [int] NOT NULL,
	[ClassId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LeadTeacher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodAssessment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NOT NULL,
	[CurriculumPartId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BasicClassId] [int] NOT NULL,
	[IsSelfEduForm] [bit] NOT NULL,
	[ParentId] [int] NULL,
	[Position] [int] NOT NULL,
	[Horarium] [int] NULL,
	[FlSubjectId] [int] NULL,
	[FlHorarium] [int] NULL,
	[FlLevel] [nvarchar](255) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsImported] [bit] NOT NULL,
 CONSTRAINT [PK_LodAssessment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodAssessmentGrade](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LodAssessmentId] [int] NOT NULL,
	[GradeCategoryId] [int] NOT NULL,
	[GradeId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[DecimalGrade] [decimal](3, 2) NULL,
 CONSTRAINT [PK_LodAssessmentGrade] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodAssessmentTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[BasicClassId] [int] NOT NULL,
	[IsSelfEduForm] [bit] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodAssessmentTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodEvaluationGeneral](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Specialty] [nvarchar](max) NULL,
	[EduForm] [nvarchar](max) NULL,
	[ClassName] [nvarchar](max) NULL,
	[PersonId] [int] NOT NULL,
	[isSelfEduForm] [bit] NOT NULL,
	[LodEvaluationsResultId] [int] NULL,
	[IsFinalized] [bit] NOT NULL,
	[FinalizedDate] [datetime2](7) NULL,
	[StudentClassId] [int] NULL,
	[MajorCourse1] [nvarchar](255) NULL,
	[MajorCourse2] [nvarchar](255) NULL,
	[MajorCourse3] [nvarchar](255) NULL,
	[MajorCourse4] [nvarchar](255) NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodEvaluationGeneral] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodEvaluationSectionAB](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[OrderNum] [int] NOT NULL,
	[SectionAFirstTermGrade] [int] NULL,
	[SectionASecondTermGrade] [int] NULL,
	[SectionAFinalGrade] [int] NULL,
	[SectionASession1Grade] [int] NULL,
	[SectionASession2Grade] [int] NULL,
	[SectionASession3Grade] [int] NULL,
	[SectionBFirstTermGrade] [int] NULL,
	[SectionBSecondTermGrade] [int] NULL,
	[SectionBFinalGrade] [int] NULL,
	[SectionBSession1Grade] [int] NULL,
	[SectionBSession2Grade] [int] NULL,
	[SectionBSession3Grade] [int] NULL,
	[SectionAHours] [int] NULL,
	[SectionBHours] [int] NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[StudentClassId] [int] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodEvaluationSectionAB] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodEvaluationSectionG](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[OrderNum] [int] NOT NULL,
	[SectionGFirstTermGrade] [int] NULL,
	[SectionGSecondTermGrade] [int] NULL,
	[SectionGFinalGrade] [int] NULL,
	[SectionGHours] [int] NULL,
	[PersonId] [int] NOT NULL,
	[isSelfEduForm] [bit] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[StudentClassId] [int] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodEvaluationSectionG] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodEvaluationSectionV](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[OrderNum] [int] NOT NULL,
	[SectionVFirstTermGrade] [int] NULL,
	[SectionVSecondTermGrade] [int] NULL,
	[SectionVFinalGrade] [int] NULL,
	[SectionVSession1Grade] [int] NULL,
	[SectionVSession2Grade] [int] NULL,
	[SectionVSession3Grade] [int] NULL,
	[SectionVHours] [int] NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[StudentClassId] [int] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodEvaluationSectionV] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodEvaluationsResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_LodEvaluationsResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodFinalizationSignatory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LODFinalizationId] [int] NOT NULL,
	[SysRoleId] [int] NOT NULL,
	[Reason] [nvarchar](1000) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Activity] [nvarchar](max) NOT NULL,
	[InstitutionId] [int] NULL,
 CONSTRAINT [PK_LodFinalizationSignatory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodFirstGradeEvaluation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[OrderNum] [int] NOT NULL,
	[FirstTermGrade] [int] NULL,
	[SecondTermGrade] [int] NULL,
	[FinalGrade] [int] NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[StudentClassId] [int] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodFirstGradeEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodFirstGradeResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[QualitativeGrade] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsFinalized] [bit] NOT NULL,
	[FinalizedDate] [datetime2](7) NULL,
	[StudentClassId] [int] NULL,
 CONSTRAINT [PK_LodFirstGradeResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[LodSelfEduFormEvaluation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[OrderNum] [int] NOT NULL,
	[CourseName] [nvarchar](max) NULL,
	[SectionASession0Grade] [int] NULL,
	[SectionASession1Grade] [int] NULL,
	[SectionASession2Grade] [int] NULL,
	[SectionASession3Grade] [int] NULL,
	[SectionAFinalGrade] [int] NULL,
	[SectionBSession0Grade] [int] NULL,
	[SectionBSession1Grade] [int] NULL,
	[SectionBSession2Grade] [int] NULL,
	[SectionBSession3Grade] [int] NULL,
	[SectionBFinalGrade] [int] NULL,
	[SectionAHours] [int] NULL,
	[SectionBHours] [int] NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[StudentClassId] [int] NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_LodSelfEduFormEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Message](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SenderId] [int] NOT NULL,
	[ReceiverId] [int] NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[Contents] [nvarchar](max) NOT NULL,
	[SendDate] [datetime2](7) NOT NULL,
	[ReadDate] [datetime2](7) NULL,
	[IsRead] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[MessageAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[FileSize] [bigint] NOT NULL,
	[BlobId] [int] NOT NULL,
	[ContentType] [nvarchar](255) NULL,
	[MessageId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_MessageAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Ministry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[NameEN] [nvarchar](255) NULL,
	[NameDE] [nvarchar](255) NULL,
	[NameFR] [nvarchar](255) NULL,
 CONSTRAINT [PK_Ministry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Month](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_Month] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[NaturalIndicators](
	[SchoolYear] [smallint] NOT NULL,
	[Period] [int] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[ExecutionTime] [datetime2](7) NOT NULL,
	[col1] [nvarchar](255) NULL,
	[col2] [nvarchar](255) NULL,
	[col3] [nvarchar](255) NULL,
	[col4] [int] NULL,
	[col5] [nvarchar](255) NULL,
	[col6] [nvarchar](255) NULL,
	[col7] [nvarchar](255) NULL,
	[col8] [nvarchar](255) NULL,
	[col9] [int] NULL,
	[col10] [nvarchar](50) NULL,
	[col11] [decimal](18, 2) NULL,
	[col12] [int] NULL,
	[col13] [decimal](18, 2) NULL,
	[col14] [int] NULL,
	[col15] [int] NULL,
	[col16] [int] NULL,
	[col17] [int] NULL,
	[col18] [int] NULL,
	[col19] [int] NULL,
	[col20] [int] NULL,
	[col21] [decimal](18, 2) NULL,
	[col22] [decimal](18, 2) NULL,
	[col23] [int] NULL,
	[col24] [int] NULL,
	[col25] [int] NULL,
	[col26] [int] NULL,
	[col27] [int] NULL,
	[col28] [int] NULL,
	[col29] [int] NULL,
	[col30] [int] NULL,
	[col31] [decimal](18, 2) NULL,
	[col32] [int] NULL,
	[col33] [int] NULL,
	[col34] [int] NULL,
	[col35] [int] NULL,
	[col36] [int] NULL,
	[col37] [int] NULL,
	[col38] [int] NULL,
	[col39] [int] NULL,
	[col40] [int] NULL,
	[col41] [int] NULL,
	[col42] [int] NULL,
	[col43] [int] NULL,
	[col44] [int] NULL,
	[col45] [int] NULL,
	[col46] [int] NULL,
	[col47] [int] NULL,
	[col48] [decimal](18, 2) NULL,
	[col49] [int] NULL,
	[col50] [int] NULL,
	[col51] [decimal](18, 2) NULL,
	[col52] [int] NULL,
	[col53] [decimal](18, 2) NULL,
	[col54] [int] NULL,
	[col55] [decimal](18, 2) NULL,
	[col56] [int] NULL,
	[col57] [decimal](18, 2) NULL,
	[col58] [int] NULL,
	[col59] [bit] NULL,
	[col60] [int] NULL,
	[col61] [bit] NULL,
	[col62] [int] NULL,
	[col63] [int] NULL,
	[col64] [int] NULL,
	[col65] [int] NULL,
	[col66] [int] NULL,
	[col67] [int] NULL,
	[col68] [int] NULL,
	[col69] [int] NULL,
	[col70] [int] NULL,
	[col71] [int] NULL,
	[col72] [int] NULL,
	[col73] [int] NULL,
	[col74] [int] NULL,
	[col75] [int] NULL,
	[col76] [int] NULL,
	[col77] [int] NULL,
	[col78] [int] NULL,
	[col79] [int] NULL,
	[col80] [int] NULL,
	[col81] [int] NULL,
	[col82] [int] NULL,
	[col83] [int] NULL,
	[col84] [int] NULL,
	[col85] [int] NULL,
	[col86] [int] NULL,
	[col87] [int] NULL,
	[col88] [int] NULL,
	[col89] [int] NULL,
	[col90] [int] NULL,
	[col91] [int] NULL,
	[col98] [int] NULL,
	[col99] [int] NULL,
	[col100] [int] NULL,
	[col101] [int] NULL,
	[col106] [int] NULL,
	[col107] [int] NULL,
	[col108] [int] NULL,
	[col109] [int] NULL,
	[col110] [int] NULL,
	[col111] [int] NULL,
	[col112] [int] NULL,
	[col113] [int] NULL,
	[col114] [int] NULL,
	[col115] [int] NULL,
	[col116] [int] NULL,
	[col117] [int] NULL,
	[col118] [int] NULL,
	[col119] [int] NULL,
	[col120] [int] NULL,
	[col121] [int] NULL,
	[col122] [int] NULL,
	[col123] [int] NULL,
	[col124] [int] NULL,
	[col125] [int] NULL,
	[col126] [int] NULL,
	[col127] [int] NULL,
	[col128] [int] NULL,
	[col129] [int] NULL,
	[col130] [int] NULL,
	[col131] [int] NULL,
	[col132] [int] NULL,
	[col133] [int] NULL,
	[col134] [int] NULL,
	[col135] [int] NULL,
	[col136] [int] NULL,
	[col137] [int] NULL,
	[col138] [int] NULL,
	[col139] [int] NULL,
	[col140] [int] NULL,
	[col141] [int] NULL,
	[col142] [int] NULL,
	[col143] [int] NULL,
	[col144] [int] NULL,
	[col145] [int] NULL,
	[col146] [int] NULL,
	[col146a] [int] NULL,
	[col147] [int] NULL,
	[col148] [int] NULL,
	[col149] [int] NULL,
	[col150] [int] NULL,
	[col151] [int] NULL,
	[col150a] [int] NULL,
	[col151a] [int] NULL,
	[col152] [int] NULL,
	[col153] [int] NULL,
	[col154] [int] NULL,
	[col155] [int] NULL,
	[col156] [int] NULL,
	[col157] [int] NULL,
	[col158] [int] NULL,
	[col159] [int] NULL,
	[col160] [int] NULL,
	[col161] [int] NULL,
	[col162] [int] NULL,
	[col163] [int] NULL,
	[col164] [int] NULL,
	[col165] [int] NULL,
	[col166] [int] NULL,
	[col167] [int] NULL,
	[col168] [int] NULL,
	[col169] [int] NULL,
	[col170] [int] NULL,
	[col171] [int] NULL,
	[col172] [int] NULL,
	[col173] [int] NULL,
	[col174] [int] NULL,
	[col175] [int] NULL,
	[col176] [int] NULL,
	[col177] [int] NULL,
	[col178] [int] NULL,
	[col179] [int] NULL,
	[col180] [int] NULL,
	[col181] [int] NULL,
	[col182] [int] NULL,
	[col183] [int] NULL,
	[col184] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SchoolYear] ASC,
	[Period] ASC,
	[InstitutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[NaturalIndicatorsColumn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[DetailedSchoolTypes] [nvarchar](255) NULL,
 CONSTRAINT [PK_NaturalIndicatorsColumn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[NaturalIndicatorsPrice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColumnName] [nvarchar](255) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[Price] [decimal](7, 2) NULL,
	[Currency] [varchar](3) NOT NULL,
	[AltCurrency]  AS (case when [Currency]='BGN' then 'EUR' else 'BGN' end) PERSISTED NOT NULL,
	[AltPrice]  AS (case when [Currency]='BGN' then round([Price]/(1.955830),(6)) else round([Price]*(1.955830),(6)) end) PERSISTED,
 CONSTRAINT [PK_NaturalIndicatorsPrice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Note](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IssueDate] [date] NOT NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[InstitutionId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[NoteDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NoteId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_NoteDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Ores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NULL,
	[OresTypeId] [int] NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[InstitutionId] [int] NULL,
 CONSTRAINT [PK_Ores] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[OresAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OresId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_OresAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[OresToEntity](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OresId] [int] NOT NULL,
	[InstitutionId] [int] NULL,
	[ClassId] [int] NULL,
	[PersonId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[StudentClassId] [int] NULL,
 CONSTRAINT [PK_OresToEntity] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ORESType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ORESType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[OtherDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](400) NULL,
	[RegNumberTotal] [nvarchar](400) NULL,
	[RegNumber] [nvarchar](400) NULL,
	[IssueDate] [date] NULL,
	[DeliveryDate] [date] NULL,
	[InstitutionId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[Series] [nvarchar](50) NULL,
	[FactoryNumber] [nvarchar](50) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[BasicDocumentId] [int] NOT NULL,
	[InstitutionName] [nvarchar](1000) NULL,
 CONSTRAINT [PK_OtherDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[OtherDocumentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OtherDocumentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_OtherDocumentDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[OtherInstitution](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Reason] [nvarchar](2048) NULL,
	[InstitutionId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NULL,
 CONSTRAINT [PK_OtherInstitution] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Participation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[ValidForStudent] [bit] NOT NULL,
 CONSTRAINT [PK_Participation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalDevelopmentAdditionalSupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalDevelopmentSupportId] [int] NOT NULL,
	[AdditionalSupportTypeId] [int] NOT NULL,
	[Information] [nvarchar](2000) NULL,
 CONSTRAINT [PK_PersonalDevelopmentAdditionalSupportType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalDevelopmentCommonSupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalDevelopmentSupportId] [int] NOT NULL,
	[CommonSupportTypeId] [int] NOT NULL,
	[Information] [nvarchar](2000) NULL,
 CONSTRAINT [PK_PersonalDevelopmentCommonSupportType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalDevelopmentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DocumentType] [int] NOT NULL,
	[PersonalDevelopmentSupportId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_PersonalDevelopmentDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalDevelopmentEarlyEvaluationReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalDevelopmentSupportId] [int] NOT NULL,
	[EarlyEvaluationReasonId] [int] NOT NULL,
	[Information] [nvarchar](2000) NULL,
 CONSTRAINT [PK_PersonalDevelopmentEarlyEvaluationReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalDevelopmentSupport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EarlyEvaluationAndEducationalRiskInfo] [nvarchar](2000) NULL,
	[AdditionalModulesNeededForNonBulgarianSpeakingInfo] [nvarchar](2000) NULL,
	[EvaluationConclusionInfo] [nvarchar](2000) NULL,
	[SupportPeriodTypeId] [int] NULL,
	[StudentTypeId] [int] NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_PersonalDevelopmentSupport] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PersonalDevelopmentSupport] UNIQUE NONCLUSTERED 
(
	[SchoolYear] ASC,
	[PersonId] ASC,
	[StudentTypeId] ASC,
	[SupportPeriodTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalEarlyAssessment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[AdditionalInfo] [nvarchar](2048) NULL,
	[BgAdditionalTrainingInfo] [nvarchar](2048) NULL,
	[ConclusionInfo] [nvarchar](2048) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_PersonalEarlyAssessment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalEarlyAssessmentAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalEarlyAssessmentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_PersonalEarlyAssessmentAttachment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalEarlyAssessmentDisabilityReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalEarlyAssessmentId] [int] NOT NULL,
	[ReasonId] [int] NOT NULL,
	[Details] [nvarchar](2048) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_PersonalEarlyAssessmentDisabilityReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PersonalEarlyAssessmentLearningDisability](
	[PersonalEarlyAssessmentId] [int] NOT NULL,
	[AgeRange] [nvarchar](255) NULL,
	[Result] [nvarchar](255) NULL,
	[Score] [decimal](5, 2) NULL,
	[Details] [nvarchar](2048) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_PersonalEarlyAssessmentLearningDisability] PRIMARY KEY CLUSTERED 
(
	[PersonalEarlyAssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PreSchoolEvaluation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SubjectId] [int] NULL,
	[BasicClassId] [int] NOT NULL,
	[StartOfYearEvaluation] [nvarchar](max) NULL,
	[EndOfYearEvaluation] [nvarchar](max) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_PreSchoolEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[PreSchoolReadiness](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[Contents] [nvarchar](max) NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_PreSchoolReadiness] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ReasonForEqualizationType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ReasonForEqualizationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ReasonForReassessmentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ReasonForReassessmentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Reassessment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ReasonId] [int] NOT NULL,
	[BasicClassId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Reassessment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ReassessmentDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReassessmentId] [int] NOT NULL,
	[Grade] [decimal](5, 2) NULL,
	[OtherGrade] [int] NULL,
	[SpecialNeedsGrade] [int] NULL,
	[GradeCategory] [int] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[SubjectTypeId] [int] NULL,
	[Position] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_ReassessmentDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ReassessmentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReassessmentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_ReassessmentDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Recognition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[InstitutionName] [nvarchar](1000) NOT NULL,
	[InstitutionCountryId] [int] NULL,
	[EducationLevelId] [int] NOT NULL,
	[Term] [int] NULL,
	[BasicClassId] [int] NULL,
	[DiplomaNumber] [nvarchar](100) NULL,
	[DiplomaDate] [date] NULL,
	[OrderNumber] [nvarchar](100) NULL,
	[OrderDate] [date] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[RuoDocumentNumber] [nvarchar](100) NULL,
	[RuoDocumentDate] [date] NULL,
	[SPPOOProfessionId] [int] NULL,
	[SPPOOSpecialityId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[IsSelfEduForm] [bit] NOT NULL,
 CONSTRAINT [PK_Recognition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RecognitionDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RecognitionId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_RecognitionDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RecognitionEducationLevel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_EducationLevel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RecognitionEqualization](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OriginalSubject] [nvarchar](400) NULL,
	[OriginalGrade] [nvarchar](100) NULL,
	[SubjectId] [int] NOT NULL,
	[ExamDate] [date] NULL,
	[ExamFinalDate] [date] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[RecognitionId] [int] NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[Position] [int] NOT NULL,
	[GradeCategory] [int] NOT NULL,
	[Grade] [decimal](5, 2) NULL,
	[SpecialNeedsGrade] [int] NULL,
	[OtherGrade] [int] NULL,
	[SubjectTypeId] [int] NULL,
	[BasicClassId] [int] NULL,
 CONSTRAINT [PK_RecognitionEqualization] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RegisterLeftForAbroad](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonalIdType] [int] NOT NULL,
	[PersonalId] [nvarchar](20) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[BirthDate] [date] NOT NULL,
	[InstitutionId] [int] NOT NULL,
	[InstitutionName] [nvarchar](200) NOT NULL,
	[EventDate] [date] NOT NULL,
 CONSTRAINT [PK_RegisterLeftForAbroad] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RelocationDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NoteNumber] [nvarchar](20) NOT NULL,
	[NoteDate] [date] NOT NULL,
	[HostInstitutionId] [int] NULL,
	[Status] [int] NOT NULL,
	[Grades] [nvarchar](max) NULL,
	[PersonId] [int] NOT NULL,
	[RUOOrderNumber] [nvarchar](20) NULL,
	[RUOOrderDate] [date] NULL,
	[CurrentStudentClassId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[DischargeDate] [date] NULL,
	[RelocationReasonTypeId] [int] NULL,
	[SendingInstitutionId] [int] NULL,
	[SendingInstitutionSchoolYear] [smallint] NOT NULL,
	[ConfirmationDate] [datetime2](7) NULL,
	[HostInstitutionSchoolYear] [smallint] NULL,
 CONSTRAINT [PK_RelocationDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RelocationDocumentCurrentTermGrades](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelocationDocumentId] [int] NOT NULL,
	[SubjectName] [nvarchar](2048) NOT NULL,
	[SubjectID] [int] NULL,
	[SubjectTypeID] [int] NULL,
	[Grades] [nvarchar](2048) NULL,
	[Term] [int] NOT NULL,
	[CurriculumPartID] [int] NULL,
	[CurriculumID] [int] NULL,
	[InstitutionId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[IsLoadedFromSchoolbook] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_RelocationDocumentCurrentTermGrades] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RelocationDocumentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RelocationDocumentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_RelocationDocumentDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[RepeaterReason](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_RepeaterReason] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[ResourceSupporReportId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_ResourceSupportDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportSpecialistType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ResourceSupportSpecialistType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportSpecialistWorkPlace](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ResourceSupportSpecialistWorkPlace] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportSpecialistWorkPlaceToResourceSupportType](
	[ResourceSupportSpecialistWorkPlaceId] [int] NOT NULL,
	[ResourceSupportTypeId] [int] NOT NULL,
 CONSTRAINT [PK_ResourceSupportSpecialistWorkPlaceToResourceSupportType] PRIMARY KEY CLUSTERED 
(
	[ResourceSupportSpecialistWorkPlaceId] ASC,
	[ResourceSupportTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ResourceSupportType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ResourceSupportType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Sanction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [datetime2](7) NULL,
	[PersonId] [int] NOT NULL,
	[SanctionTypeId] [int] NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[InstitutionId] [int] NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[OrderNumber] [nvarchar](100) NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[RuoOrderNumber] [nvarchar](100) NULL,
	[RuoOrderDate] [datetime2](7) NULL,
	[CancelOrderNumber] [nvarchar](100) NULL,
	[CancelOrderDate] [datetime2](7) NULL,
	[CancelReason] [nvarchar](1000) NULL,
	[SchoolYear] [smallint] NOT NULL,
	[SourceType] [tinyint] NULL,
 CONSTRAINT [PK_Sanction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SanctionDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SanctionId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_SanctionDocumentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SanctionType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SortOrd] [int] NULL,
 CONSTRAINT [PK_SanctionType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipAmount](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AmountPercent] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ScholarshipAmount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipFinancingOrgan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_ScholarshipFinancingOrgan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipStudentDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScholarshipStudentId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_ScholarshipStudentDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ScholarshipType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[Periodicity] [tinyint] NULL,
 CONSTRAINT [PK_ScholarshipType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SelfGovernmentPosition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[ValidForStudent] [bit] NOT NULL,
 CONSTRAINT [PK_SelfGovernmentPosition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialEquipment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentTypeId] [int] NOT NULL,
	[StudentClassID] [int] NOT NULL,
	[IsAvailable] [bit] NOT NULL,
	[PersonId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_SpecialEquipment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsSubType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[SpecialNeedsTypeId] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_SpecialNeedsSubType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_SpecialNeedsType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SpecialNeedsYearAttachment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SpecialNeedsYearId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_SpecialNeedsYearAttachmentId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Student](
	[PersonID] [int] NOT NULL,
	[HomePhone] [nvarchar](100) NULL,
	[WorkPhone] [nvarchar](100) NULL,
	[MobilePhone] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[LivesWithFosterFamily] [bit] NOT NULL,
	[HasParentConsent] [bit] NOT NULL,
	[GPPhone] [nvarchar](255) NULL,
	[GPName] [nvarchar](255) NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[RepresentedByTheMajor] [bit] NOT NULL,
	[NativeLanguageId] [int] NULL,
	[FamilyEducationWeight] [decimal](18, 2) NOT NULL,
	[FamilyWorkStatusWeight] [decimal](18, 2) NOT NULL,
	[UserManagementIntegrationResult] [nvarchar](4000) NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED 
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
CREATE TABLE [student].[StudentClassDualFormCompany](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StudentClassId] [int] NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[CompanyUic] [nvarchar](30) NOT NULL,
	[CompanyCountry] [nvarchar](255) NULL,
	[CompanyDistrict] [nvarchar](255) NULL,
	[CompanyMunicipality] [nvarchar](255) NULL,
	[CompanySettlement] [nvarchar](255) NULL,
	[CompanyArea] [nvarchar](255) NULL,
	[CompanyAddress] [nvarchar](255) NULL,
	[CompanyPhone] [nvarchar](255) NULL,
	[CompanyEmail] [nvarchar](255) NULL,
	[CompanyUrl] [nvarchar](255) NULL,
	[CompanyRegixData] [nvarchar](2048) NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBySysUserId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_StudentClassDualFormCompany] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[StudentClassHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StudentClassId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ClassId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[StudentSpecialityId] [int] NULL,
	[StudentEduFormId] [int] NOT NULL,
	[ClassNumber] [int] NULL,
	[Status] [int] NOT NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[IsHourlyOrganization] [bit] NULL,
	[IsForSubmissionToNRA] [bit] NULL,
	[IsCurrent] [bit] NOT NULL,
	[RepeaterId] [int] NOT NULL,
	[CommuterTypeId] [int] NULL,
	[HasSuportiveEnvironment] [bit] NULL,
	[SupportiveEnvironment] [nvarchar](1000) NULL,
	[EnrollmentDate] [datetime2](7) NOT NULL,
	[AdmissionDocumentId] [int] NULL,
	[PositionId] [int] NOT NULL,
	[BasicClassId] [int] NOT NULL,
	[ClassTypeId] [int] NOT NULL,
	[FromStudentClassId] [int] NULL,
	[DischargeReasonId] [int] NULL,
	[RelocationDocumentId] [int] NULL,
	[DischargeDocumentId] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
	[ORESTypeId] [int] NULL,
	[IsFTACOutsourced] [bit] NULL,
	[InstitutionId] [int] NOT NULL,
 CONSTRAINT [PK_StudentClassHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[StudentEvaluation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TwoYearsAgoEvaluation] [int] NULL,
	[OneYearAgoEvaluation] [int] NULL,
	[FirstTermEvaluation] [int] NULL,
	[SecondTermEvaluation] [int] NULL,
	[AnnualEvaluation] [int] NULL,
	[SubjectId] [int] NOT NULL,
	[RelocationDocumentId] [int] NULL,
	[DischargeDocumentId] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[CurriculumPartId] [int] NULL,
	[BasicClassId] [int] NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_StudentEvaluation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[StudentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_StudentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[SupportPeriod](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](2048) NULL,
	[IsValid] [bit] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
 CONSTRAINT [PK_SupportPeriod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[temp_DRReport](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AzureId] [uniqueidentifier] NOT NULL,
	[Role] [varchar](50) NOT NULL,
	[PersonId] [int] NULL,
	[InstitutionId] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[temp_PISA](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicEduNumber] [varchar](50) NULL,
	[SubjectId] [int] NULL,
	[SubjectName] [varchar](50) NULL,
	[Points] [decimal](18, 2) NULL,
	[Grade] [decimal](18, 2) NULL,
	[PersonId] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Temp_StdsForDel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicEduNumber] [varchar](50) NULL,
	[PersonalId] [varchar](50) NULL,
	[AzureId] [varchar](36) NULL,
	[PersonId] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[tr_AppLock](
	[Id] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tr_AppLock] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[tr_Object](
	[Id] [varchar](255) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_tr_Object] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[tr_Set](
	[Id] [varchar](255) NOT NULL,
	[Member] [varchar](255) NOT NULL,
 CONSTRAINT [PK_tr_Set] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Member] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[tr_String](
	[Id] [varchar](255) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_tr_String] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[Validation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonId] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ExamType] [int] NOT NULL,
	[ProtocolNumber] [nvarchar](100) NOT NULL,
	[ProtocolDate] [date] NOT NULL,
	[SubjectId] [int] NOT NULL,
	[BasicClassId] [int] NULL,
	[ExamDate] [date] NOT NULL,
	[FinalGrade] [decimal](3, 2) NOT NULL,
	[FinalGradeText] [nvarchar](100) NOT NULL,
	[StudentEduFormId] [int] NOT NULL,
	[CreatedBySysUserID] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[ModifiedBySysUserId] [int] NULL,
	[ModifyDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Validation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student].[ValidationDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ValidationId] [int] NOT NULL,
	[DocumentId] [int] NOT NULL,
 CONSTRAINT [PK_ValidationDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Absence_AbsenceImportId] ON [student].[Absence]
(
	[AbsenceImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Absence_ClassId] ON [student].[Absence]
(
	[ClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentAbsene_SchoolYear_Month] ON [student].[Absence]
(
	[SchoolYear] ASC,
	[Month] ASC
)
INCLUDE ( 	[PersonId],
	[Excused],
	[Unexcused],
	[InstitutionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AbsenceASPFlatFile_PersonalId] ON [student].[AbsenceASPFlatFile]
(
	[PersonalId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AbsenceCampaign_SchoolYear_Month] ON [student].[AbsenceCampaign]
(
	[SchoolYear] ASC,
	[Month] ASC
)
INCLUDE ( 	[FromDate],
	[ToDate],
	[IsManuallyActivated]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AbsenceHistory_AbsenceId] ON [student].[AbsenceHistory]
(
	[AbsenceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Absence_Institution_SchoolYear_Month] ON [student].[AbsenceImport]
(
	[InstitutionId] ASC,
	[SchoolYear] ASC,
	[Month] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupport_PersonId] ON [student].[AdditionalPersonalDevelopmentSupport]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_AdditionalPersonalDevelopmentSupport_Person_SchoolTear_StudentType] ON [student].[AdditionalPersonalDevelopmentSupport]
(
	[PersonId] ASC,
	[SchoolYear] ASC,
	[StudentTypeId] ASC
)
WHERE ([IsSuspended]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupportAttachment_AdditionalPersonalDevelopmentSupportId] ON [student].[AdditionalPersonalDevelopmentSupportAttachment]
(
	[AdditionalPersonalDevelopmentSupportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupportAttachment_Type] ON [student].[AdditionalPersonalDevelopmentSupportAttachment]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupportItem_AdditionalPersonalDevelopmentSupportId] ON [student].[AdditionalPersonalDevelopmentSupportItem]
(
	[AdditionalPersonalDevelopmentSupportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupportItemAttachment_Item] ON [student].[AdditionalPersonalDevelopmentSupportItemAttachment]
(
	[AdditionalPersonalDevelopmentSupportItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AdditionalPersonalDevelopmentSupportItemAttachment_Item_Type] ON [student].[AdditionalPersonalDevelopmentSupportItemAttachment]
(
	[AdditionalPersonalDevelopmentSupportItemId] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionDocument_PersonId] ON [student].[AdmissionDocument]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionDocument_StudentClassId] ON [student].[AdmissionDocument]
(
	[CurrentStudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionDocumentDocument] ON [student].[AdmissionDocumentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionDocumentDocument_AdmissionDocumentId] ON [student].[AdmissionDocumentDocument]
(
	[AdmissionDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionPermissionRequest_AuthorizingInstitutionId_IsPermissionGranted] ON [student].[AdmissionPermissionRequest]
(
	[AuthorizingInstitutionId] ASC,
	[IsPermissionGranted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionPermissionRequest_PersonId] ON [student].[AdmissionPermissionRequest]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionPermissionRequest_RequestingInstitutionId_IsPermissionGranted] ON [student].[AdmissionPermissionRequest]
(
	[RequestingInstitutionId] ASC,
	[IsPermissionGranted] ASC
)
INCLUDE ( 	[AdmissionDocumentId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionPermissionRequestAttachmen_AdmissionPermissionRequestId] ON [student].[AdmissionPermissionRequestAttachment]
(
	[AdmissionPermissionRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AdmissionPermissionRequestAttachment] ON [student].[AdmissionPermissionRequestAttachment]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AspAskingTemp_ID_NUMBER] ON [student].[AspAskingTemp]
(
	[ID_NUMBER] ASC
)
INCLUDE ( 	[ID_TYPE]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AspAskingTemp_TARGET_MONTH] ON [student].[AspAskingTemp]
(
	[TARGET_MONTH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ASPMonthlyBenefit_ASPMonthlyBenefitsImportId] ON [student].[ASPMonthlyBenefit]
(
	[ASPMonthlyBenefitsImportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ASPMonthlyBenefit_CurrentInstitutionId] ON [student].[ASPMonthlyBenefit]
(
	[CurrentInstitutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ASPMonthlyBenefitInstitution_InstitutionId_ASPMonthlyBenefitImportId] ON [student].[ASPMonthlyBenefitInstitution]
(
	[InstitutionId] ASC,
	[ASPMonthlyBenefitImportId] ASC
)
INCLUDE ( 	[IsSigned]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ASPMonthlyBenefitsImport_AspSessionNo] ON [student].[ASPMonthlyBenefitsImport]
(
	[AspSessionNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AspSubmittedDataHistory_PersonalId_PersonalIdType] ON [student].[AspSubmittedDataHistory]
(
	[PersonalId] ASC,
	[PersonalIdType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_AspSubmittedDataHistory_Year_Month_ExportTypeCode] ON [student].[AspSubmittedDataHistory]
(
	[Year] ASC,
	[Month] ASC,
	[ExportTypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Stuent_AuditEntries_CreatedBySysUserId] ON [student].[AuditEntries]
(
	[CreatedBySysUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Stuent_AuditEntryID] ON [student].[AuditEntryProperties]
(
	[AuditEntryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Award_PersonId_SchoolYear] ON [student].[Award]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_AwardDocument] ON [student].[AwardDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CommonPersonalDevelopmentSupport_PersonId] ON [student].[CommonPersonalDevelopmentSupport]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CommonPersonalDevelopmentSupportAttachment_CommonPersonalDevelopmentSupportId] ON [student].[CommonPersonalDevelopmentSupportAttachment]
(
	[CommonPersonalDevelopmentSupportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CommonPersonalDevelopmentSupportItem_CommonPersonalDevelopmentSupportId] ON [student].[CommonPersonalDevelopmentSupportItem]
(
	[CommonPersonalDevelopmentSupportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_Student_ContextualInformation_Modulename_Key] ON [student].[ContextualInformation]
(
	[ModuleName] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DischargeDocument_CurrentStudentClass] ON [student].[DischargeDocument]
(
	[CurrentStudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DischargeDocument_PersonId] ON [student].[DischargeDocument]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DischargeDocumentDocument] ON [student].[DischargeDocumentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_DischargeDocumentDocument_DischargeDocumentId] ON [student].[DischargeDocumentDocument]
(
	[DischargeDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Document_BlobId] ON [student].[Document]
(
	[BlobId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Equalization_PersonId] ON [student].[Equalization]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EqualizationAttachment] ON [student].[EqualizationAttachment]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EqualizationAttachment_OtherDocumentId] ON [student].[EqualizationAttachment]
(
	[EqualizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ExternalEvaluation_ParentId] ON [student].[ExternalEvaluation]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ExternalEvaluation_PersonId] ON [student].[ExternalEvaluation]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ExternalEvaluationItem_ExternalEvaluationId] ON [student].[ExternalEvaluationItem]
(
	[ExternalEvaluationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_GradeNom_GradeType_Name] ON [student].[GradeNom]
(
	[Name] ASC,
	[GradeTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_GradeNom_SchoolBookGradeId] ON [student].[GradeNom]
(
	[SchoolBookGradeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InternationalMobility_PersonId_SchoolYear] ON [student].[InternationalMobility]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InternationalMobilityDocument] ON [student].[InternationalMobilityDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InternationalProtection_PersonId] ON [student].[InternationalProtection]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodAssessment_Institution_SchoolYear_IsImported] ON [student].[LodAssessment]
(
	[InstitutionId] ASC,
	[SchoolYear] ASC,
	[IsImported] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodAssessment_Parent] ON [student].[LodAssessment]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodAssessment_Person] ON [student].[LodAssessment]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodAssessmentGrade_LodAssessmentId] ON [student].[LodAssessmentGrade]
(
	[LodAssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodAssessmentTemplate_InstitutionId] ON [student].[LodAssessmentTemplate]
(
	[InstitutionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationGeneral_PersonId] ON [student].[LodEvaluationGeneral]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationGeneral_StudentClassId] ON [student].[LodEvaluationGeneral]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionAB_PersonId] ON [student].[LodEvaluationSectionAB]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionAB_StudentClassId] ON [student].[LodEvaluationSectionAB]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_StudentClass] ON [student].[LodEvaluationSectionAB]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionG_PersonId] ON [student].[LodEvaluationSectionG]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionG_StudentClassId] ON [student].[LodEvaluationSectionG]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionV_PersonId] ON [student].[LodEvaluationSectionV]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodEvaluationSectionV_StudentClassId] ON [student].[LodEvaluationSectionV]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LODFinalization_Document] ON [student].[LODFinalization]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LODFinalization_PersonId_SchoolYear] ON [student].[LODFinalization]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)
INCLUDE ( 	[IsFinalized],
	[IsApproved]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LODFinalization_SchoolYear_IsFinalized] ON [student].[LODFinalization]
(
	[SchoolYear] ASC,
	[IsFinalized] ASC
)
INCLUDE ( 	[PersonId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodFinalizationSignatory_LODFinalizationId] ON [student].[LodFinalizationSignatory]
(
	[LODFinalizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodFirstGradeEvaluation_PersonId] ON [student].[LodFirstGradeEvaluation]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodFirstGradeEvaluation_StudentClassId] ON [student].[LodFirstGradeEvaluation]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodFirstGradeResult_PersonId] ON [student].[LodFirstGradeResult]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodFirstGradeResult_StudentClassId] ON [student].[LodFirstGradeResult]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodSelfEduFormEvaluation_PersonId] ON [student].[LodSelfEduFormEvaluation]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_LodSelfEduFormEvaluation_StudentClassId] ON [student].[LodSelfEduFormEvaluation]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Message_ReceiverId] ON [student].[Message]
(
	[ReceiverId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_MessageAttachment_MessageId] ON [student].[MessageAttachment]
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_NoteDocument] ON [student].[NoteDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OresAttachment] ON [student].[OresAttachment]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OresAttachment_OresId] ON [student].[OresAttachment]
(
	[OresId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OresToEntity_OresId] ON [student].[OresToEntity]
(
	[OresId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_OresToEntity_ClassId] ON [student].[OresToEntity]
(
	[OresId] ASC,
	[ClassId] ASC
)
WHERE ([ClassId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_OresToEntity_InstitutionId] ON [student].[OresToEntity]
(
	[OresId] ASC,
	[InstitutionId] ASC
)
WHERE ([InstitutionId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_OresToEntity_PersonId] ON [student].[OresToEntity]
(
	[OresId] ASC,
	[PersonId] ASC
)
WHERE ([PersonId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OtherDocument_PersonId_SchoolYea] ON [student].[OtherDocument]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OtherDocumentDocument] ON [student].[OtherDocumentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OtherDocumentDocument_OtherDocumentId] ON [student].[OtherDocumentDocument]
(
	[OtherDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_OtherInstitution_InstitutionId_PersonId] ON [student].[OtherInstitution]
(
	[InstitutionId] ASC,
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonalDevelopmentDocument] ON [student].[PersonalDevelopmentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_PersonalEarlyAssessment_PersonId] ON [student].[PersonalEarlyAssessment]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonalEarlyAssessmentAttachment] ON [student].[PersonalEarlyAssessmentAttachment]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonalEarlyAssessmentAttachment_PersonalEarlyAssessmentId] ON [student].[PersonalEarlyAssessmentAttachment]
(
	[PersonalEarlyAssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PersonalEarlyAssessmentDisabilityReason_PersonalEarlyAssessmentId] ON [student].[PersonalEarlyAssessmentDisabilityReason]
(
	[PersonalEarlyAssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PreSchoolEvaluation_PersonId] ON [student].[PreSchoolEvaluation]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Reassessment_PersonId] ON [student].[Reassessment]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ReassessmentDetails_ReassessmentId] ON [student].[ReassessmentDetails]
(
	[ReassessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ReassessmentDocument_ReassessmentId] ON [student].[ReassessmentDocument]
(
	[ReassessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Recognition_PersonId] ON [student].[Recognition]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RecognitionDocument] ON [student].[RecognitionDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_RecognitionEducationLevel_Name] ON [student].[RecognitionEducationLevel]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocument_CurrentStudentClass] ON [student].[RelocationDocument]
(
	[CurrentStudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocument_PersonId] ON [student].[RelocationDocument]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocumentCurrentTermGrades_CurriculumId] ON [student].[RelocationDocumentCurrentTermGrades]
(
	[CurriculumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocumentCurrentTermGrades_RelocationDocumentId] ON [student].[RelocationDocumentCurrentTermGrades]
(
	[RelocationDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocumentDocument] ON [student].[RelocationDocumentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RelocationDocumentDocument_RelocationDocumentId] ON [student].[RelocationDocumentDocument]
(
	[RelocationDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupport_AdditionalPersonalDevelopmentSupportItemId] ON [student].[ResourceSupport]
(
	[AdditionalPersonalDevelopmentSupportItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupport_ResourceSupportReportId] ON [student].[ResourceSupport]
(
	[ResourceSupportReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportDocument_ResourceSupportReportId] ON [student].[ResourceSupport]
(
	[ResourceSupportReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportDocument] ON [student].[ResourceSupportDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportDocument_ResourceSupporReportId] ON [student].[ResourceSupportDocument]
(
	[ResourceSupporReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportReport_PersonId] ON [student].[ResourceSupportReport]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportReport_SchoolYear_PersonId] ON [student].[ResourceSupportReport]
(
	[SchoolYear] ASC,
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ResourceSupportSpecialist_ResourceSupportReportId] ON [student].[ResourceSupportSpecialist]
(
	[ResourceSupportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Sanction_PersonId_SchoolYear] ON [student].[Sanction]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SanctionDocument] ON [student].[SanctionDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScholarshipStudent_PersonId] ON [student].[ScholarshipStudent]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScholarshipStudent_StudentClassId] ON [student].[ScholarshipStudent]
(
	[StudentClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ScholarshipStudentDocument] ON [student].[ScholarshipStudentDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SchoolTypeLodAccess_DetailedSchoolTypeId_isLodAccessAllowed] ON [student].[SchoolTypeLodAccess]
(
	[DetailedSchoolTypeId] ASC,
	[isLodAccessAllowed] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SelfGovernment_PersonId] ON [student].[SelfGovernment]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SelfGovernment_SchoolYear_StaffPositionID] ON [student].[SelfGovernment]
(
	[SchoolYear] ASC,
	[StaffPositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SelfGovernment_SchoolYear_StudentClassId] ON [student].[SelfGovernment]
(
	[SchoolYear] ASC,
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialNeeds_YearId] ON [student].[SpecialNeeds]
(
	[SpecialNeedsYearId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialNeedsYear_PersonId] ON [student].[SpecialNeedsYear]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialNeedsYear_PersonId_SchoolYear] ON [student].[SpecialNeedsYear]
(
	[PersonId] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialNeedsYearAttachment] ON [student].[SpecialNeedsYearAttachment]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_SpecialNeedsYearAttachment_SpecialNeedsYearId] ON [student].[SpecialNeedsYearAttachment]
(
	[SpecialNeedsYearId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_StudentClass_SchoolYear_ClassId_PersonId] ON [student].[StudentClass]
(
	[SchoolYear] ASC,
	[ClassId] ASC,
	[PersonId] ASC
)
WHERE ([IsNotPresentForm]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ix_StudentClass_AdmissionDocument] ON [student].[StudentClass]
(
	[AdmissionDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_ClassId_IsCurrent] ON [student].[StudentClass]
(
	[ClassId] ASC,
	[IsCurrent] ASC
)
INCLUDE ( 	[PersonId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_ClassTypeId_SchoolYear] ON [student].[StudentClass]
(
	[ClassTypeId] ASC,
	[SchoolYear] ASC
)
INCLUDE ( 	[PersonId],
	[EnrollmentDate],
	[InstitutionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_DischargeDocument] ON [student].[StudentClass]
(
	[DischargeDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_FromStudentClass] ON [student].[StudentClass]
(
	[FromStudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_IsCurrent_EnvCharacteristics_Opt] ON [student].[StudentClass]
(
	[IsCurrent] ASC
)
INCLUDE ( 	[SchoolYear],
	[ClassId],
	[PersonId],
	[BasicClassId],
	[ClassTypeId],
	[InstitutionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_IsCurrent_InstitutionId] ON [student].[StudentClass]
(
	[IsCurrent] ASC,
	[InstitutionId] ASC
)
INCLUDE ( 	[PersonId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_IsCurrent_Position] ON [student].[StudentClass]
(
	[IsCurrent] ASC,
	[PositionId] ASC
)
INCLUDE ( 	[SchoolYear],
	[ClassId],
	[PersonId],
	[InstitutionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_PersonId] ON [student].[StudentClass]
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_RelocationDocument] ON [student].[StudentClass]
(
	[RelocationDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_SchoolYear] ON [student].[StudentClass]
(
	[SchoolYear] ASC
)
INCLUDE ( 	[PersonId],
	[EnrollmentDate],
	[BasicClassId],
	[ClassTypeId],
	[InstitutionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_SchoolYear_ClassId] ON [student].[StudentClass]
(
	[SchoolYear] ASC,
	[ClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClass_SchoolYear_InstitutionId] ON [student].[StudentClass]
(
	[SchoolYear] ASC,
	[InstitutionId] ASC
)
INCLUDE ( 	[ClassId],
	[PersonId],
	[BasicClassId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentCLass_SchoolYear_IsCurrent] ON [student].[StudentClass]
(
	[SchoolYear] ASC,
	[IsCurrent] ASC
)
INCLUDE ( 	[PersonId],
	[BasicClassId],
	[ClassTypeId],
	[DischargeDocumentId],
	[RelocationDocumentId],
	[InstitutionId],
	[DischargeDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassDualFormCompany_StudentClassId] ON [student].[StudentClassDualFormCompany]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_AdmissionDocumentId] ON [student].[StudentClassHistory]
(
	[AdmissionDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_ClassId] ON [student].[StudentClassHistory]
(
	[ClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_CreateDate] ON [student].[StudentClassHistory]
(
	[CreateDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_FromStudentClass] ON [student].[StudentClassHistory]
(
	[FromStudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassHistory_StudentClassId] ON [student].[StudentClassHistory]
(
	[StudentClassId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassTemporalHistory_IDValidFromTo] ON [student].[StudentClassTemporalHistory]
(
	[ID] ASC,
	[ValidFrom] ASC,
	[ValidTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StudentClassTemporalHistory_PersonId_IsCurrent_ValidFrom_ValidTo] ON [student].[StudentClassTemporalHistory]
(
	[PersonId] ASC,
	[IsCurrent] ASC,
	[ValidFrom] ASC,
	[ValidTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ValidationDocument] ON [student].[ValidationDocument]
(
	[DocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
