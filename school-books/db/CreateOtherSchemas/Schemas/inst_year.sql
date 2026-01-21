SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[ChangeYearData](
	[ChangeYearDataID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[FromSchoolYear] [smallint] NOT NULL,
	[ToSchoolYear] [smallint] NOT NULL,
	[ChangeYearStatusID] [int] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[ModifiedOn] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_ChangeYearData] PRIMARY KEY CLUSTERED 
(
	[ChangeYearDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_InstitutionID_FromSchoolYear] UNIQUE NONCLUSTERED 
(
	[InstitutionID] ASC,
	[FromSchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[ChangeYearDataStudentAmendatory](
	[ChangeYearDataStudentAmendatoryID] [int] IDENTITY(1,1) NOT NULL,
	[ChangeYearDataID] [int] NOT NULL,
	[StudentClassID] [int] NOT NULL,
 CONSTRAINT [PK_ChangeYearDataStudentAmendatory] PRIMARY KEY CLUSTERED 
(
	[ChangeYearDataStudentAmendatoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[ClassGroup](
	[ClassID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[InstitutionDepartmentID] [int] NULL,
	[ClassGroupNum] [int] NULL,
	[ClassName] [nvarchar](255) NOT NULL,
	[ParalellClassName] [nvarchar](255) NULL,
	[ParentClassID] [int] NULL,
	[BasicClassID] [int] NULL,
	[ClassTypeID] [int] NULL,
	[AreaID] [int] NULL,
	[ClassEduFormID] [int] NULL,
	[ClassEduDurationID] [int] NULL,
	[ClassShiftID] [int] NULL,
	[BudgetingClassTypeID] [int] NULL,
	[EntranceLevelID] [int] NULL,
	[ClassSpecialityID] [int] NULL,
	[FLTypeID] [int] NULL,
	[FLID] [int] NULL,
	[IsProfModule] [bit] NULL,
	[StudentCountPlaces] [int] NULL,
	[Notes] [nvarchar](1024) NULL,
	[IsCombined] [bit] NULL,
	[IsNoList] [bit] NULL,
	[IsSpecNeed] [bit] NULL,
	[IsWholeClass] [bit] NULL,
	[IsNotPresentForm] [bit] NULL,
	[tmp_ClassID] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[PrevYearID] [int] NULL,
	[IsClosed] [bit] NOT NULL,
	[IsNotNPO109] [bit] NOT NULL,
	[NPO109Doc] [int] NULL,
 CONSTRAINT [PK_ClassGroup] PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[ClassGroupTemp2](
	[ClassID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[InstitutionDepartmentID] [int] NOT NULL,
	[ClassGroupNum] [int] NULL,
	[ClassName] [nvarchar](255) NOT NULL,
	[ParalellClassName] [nvarchar](255) NULL,
	[ParentClassID] [int] NULL,
	[BasicClassID] [int] NULL,
	[ClassTypeID] [int] NULL,
	[AreaID] [int] NULL,
	[ClassEduFormID] [int] NULL,
	[ClassEduDurationID] [int] NULL,
	[ClassShiftID] [int] NULL,
	[BudgetingClassTypeID] [int] NULL,
	[EntranceLevelID] [int] NULL,
	[ClassSpecialityID] [int] NULL,
	[FLTypeID] [int] NULL,
	[FLID] [int] NULL,
	[IsProfModule] [bit] NULL,
	[StudentCountPlaces] [int] NULL,
	[Notes] [nvarchar](1024) NULL,
	[IsCombined] [bit] NULL,
	[IsNoList] [bit] NULL,
	[IsSpecNeed] [bit] NULL,
	[IsWholeClass] [bit] NULL,
	[IsNotPresentForm] [bit] NULL,
	[tmp_ClassID] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[PrevYearID] [int] NULL,
 CONSTRAINT [PK_ClassGroupTemp2] PRIMARY KEY CLUSTERED 
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[Curriculum](
	[CurriculumID] [int] IDENTITY(1,1) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[CurriculumGroupNum] [smallint] NULL,
	[SubjectID] [int] NULL,
	[SubjectTypeID] [int] NULL,
	[WeeksFirstTerm] [smallint] NULL,
	[HoursWeeklyFirstTerm] [real] NULL,
	[WeeksSecondTerm] [smallint] NULL,
	[HoursWeeklySecondTerm] [real] NULL,
	[IsFL] [bit] NULL,
	[FLSubjectID] [int] NULL,
	[IsIndividualLesson] [int] NULL,
	[NormaS] [int] NULL,
	[InstitutionDepartmentID] [int] NULL,
	[ParentCurriculumID] [int] NULL,
	[CurriculumPartID] [int] NULL,
	[SortOrder] [int] NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[tmp_CurricID_orig] [int] NULL,
	[tmp_ClassID_orig] [int] NULL,
	[tmp_SubjectID_orig] [smallint] NULL,
	[tmp_DepID_orig] [int] NULL,
	[tmp_ProfSubjID] [int] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[IsWholeClass] [bit] NOT NULL,
	[IsAllStudents] [bit] NULL,
	[AzureID] [varchar](100) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[IsCombined] [bit] NOT NULL,
	[TotalTermHours] [int] NULL,
	[StudentsCount] [int] NULL,
	[MainSubjectID] [int] NULL,
	[PrevYearID] [int] NULL,
 CONSTRAINT [PK_Curriculum] PRIMARY KEY CLUSTERED 
(
	[CurriculumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumClass](
	[CurriculumClassesID] [int] IDENTITY(1,1) NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_CurriculumClass] PRIMARY KEY CLUSTERED 
(
	[CurriculumClassesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumClassTemp](
	[CurriculumClassesID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumClassTemp2](
	[CurriculumClassesID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[ClassID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_CurriculumClassTemp2] PRIMARY KEY CLUSTERED 
(
	[CurriculumClassesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumStudent](
	[CurriculumStudentID] [int] IDENTITY(1,1) NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StudentID] [int] NOT NULL,
	[PersonID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[isAzureEnrolled] [int] NOT NULL,
	[WeeksFirstTerm] [smallint] NULL,
	[HoursWeeklyFirstTerm] [real] NULL,
	[WeeksSecondTerm] [smallint] NULL,
	[HoursWeeklySecondTerm] [real] NULL,
 CONSTRAINT [PK_CurriculumStudent] PRIMARY KEY CLUSTERED 
(
	[CurriculumStudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumStudentTemp](
	[CurriculumStudentID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StudentID] [int] NULL,
	[PersonID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumStudentTemp2](
	[CurriculumStudentID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StudentID] [int] NULL,
	[PersonID] [int] NOT NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_CurriculumStudentTemp2] PRIMARY KEY CLUSTERED 
(
	[CurriculumStudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumTeacher](
	[CurriculumTeacherID] [int] IDENTITY(1,1) NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StaffPositionID] [int] NOT NULL,
	[IsNotRegularTeacher] [bit] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[NormaTS] [int] NOT NULL,
	[StaffPositionStartDate] [date] NULL,
	[StaffPositionTerminationDate] [date] NULL,
	[isAzureEnrolled] [int] NOT NULL,
	[NoReplacement] [bit] NOT NULL,
 CONSTRAINT [PK_CurriculumTeacher] PRIMARY KEY CLUSTERED 
(
	[CurriculumTeacherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumTeacherTemp](
	[CurriculumTeacherID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StaffPositionID] [int] NOT NULL,
	[IsNotRegularTeacher] [bit] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[NormaTS] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumTeacherTemp2](
	[CurriculumTeacherID] [int] NOT NULL,
	[CurriculumID] [int] NOT NULL,
	[StaffPositionID] [int] NOT NULL,
	[IsNotRegularTeacher] [bit] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[NormaTS] [int] NOT NULL,
 CONSTRAINT [PK_CurriculumTeacherTemp2] PRIMARY KEY CLUSTERED 
(
	[CurriculumTeacherID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumTemp](
	[CurriculumID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[CurriculumGroupNum] [smallint] NULL,
	[SubjectID] [int] NULL,
	[SubjectTypeID] [int] NULL,
	[WeeksFirstTerm] [smallint] NULL,
	[HoursWeeklyFirstTerm] [real] NULL,
	[WeeksSecondTerm] [smallint] NULL,
	[HoursWeeklySecondTerm] [real] NULL,
	[IsFL] [bit] NULL,
	[FLSubjectID] [int] NULL,
	[IsIndividualLesson] [bit] NULL,
	[NormaS] [int] NULL,
	[InstitutionDepartmentID] [int] NULL,
	[ParentCurriculumID] [int] NULL,
	[CurriculumPartID] [int] NULL,
	[SortOrder] [int] NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[tmp_CurricID_orig] [int] NULL,
	[tmp_ClassID_orig] [int] NULL,
	[tmp_SubjectID_orig] [smallint] NULL,
	[tmp_DepID_orig] [int] NULL,
	[tmp_ProfSubjID] [int] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[IsWholeClass] [bit] NULL,
	[IsAllStudents] [bit] NULL,
	[AzureID] [varchar](100) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[IsCombined] [bit] NOT NULL,
	[TotalTermHours] [int] NULL,
	[StudentsCount] [int] NULL,
	[MainSubjectID] [int] NULL,
	[PrevYearID] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[CurriculumTemp2](
	[CurriculumID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[CurriculumGroupNum] [smallint] NULL,
	[SubjectID] [int] NULL,
	[SubjectTypeID] [int] NULL,
	[WeeksFirstTerm] [smallint] NULL,
	[HoursWeeklyFirstTerm] [real] NULL,
	[WeeksSecondTerm] [smallint] NULL,
	[HoursWeeklySecondTerm] [real] NULL,
	[IsFL] [bit] NULL,
	[FLSubjectID] [int] NULL,
	[IsIndividualLesson] [int] NULL,
	[NormaS] [int] NULL,
	[InstitutionDepartmentID] [int] NULL,
	[ParentCurriculumID] [int] NULL,
	[CurriculumPartID] [int] NULL,
	[SortOrder] [int] NULL,
	[IsIndividualCurriculum] [bit] NULL,
	[tmp_CurricID_orig] [int] NULL,
	[tmp_ClassID_orig] [int] NULL,
	[tmp_SubjectID_orig] [smallint] NULL,
	[tmp_DepID_orig] [int] NULL,
	[tmp_ProfSubjID] [int] NULL,
	[ValidFrom] [datetime2](7) NULL,
	[ValidTo] [datetime2](7) NULL,
	[SysUserID] [int] NULL,
	[IsWholeClass] [bit] NOT NULL,
	[IsAllStudents] [bit] NULL,
	[AzureID] [varchar](100) NULL,
	[ExternalID] [nvarchar](100) NULL,
	[IsValid] [bit] NOT NULL,
	[IsCombined] [bit] NOT NULL,
	[TotalTermHours] [int] NULL,
	[StudentsCount] [int] NULL,
	[MainSubjectID] [int] NULL,
	[PrevYearID] [int] NULL,
 CONSTRAINT [PK_CurriculumTemp2] PRIMARY KEY CLUSTERED 
(
	[CurriculumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[InstitutionOtherData](
	[InstitutionOtherDataID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[SchoolShiftTypeID] [int] NULL,
	[PedagogStaffCount] [float] NULL,
	[NonpedagogStaffCount] [float] NULL,
	[StaffCountAll] [float] NULL,
	[PedagogStaffSalary] [float] NULL,
	[NonpedagogStaffSalary] [float] NULL,
	[YearlyBudget] [decimal](12, 2) NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_InstitutionOtherData] PRIMARY KEY CLUSTERED 
(
	[InstitutionOtherDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[PedStaffData](
	[PedStaffDataID] [int] IDENTITY(1,1) NOT NULL,
	[StaffPositionID] [int] NOT NULL,
	[NormaS] [float] NULL,
	[Norma] [float] NULL,
	[NormaT] [float] NULL,
	[DeficitNorma] [float] NULL,
	[DeficitNormaT] [float] NULL,
	[ReductionCoef] [float] NULL,
	[DiffNorma] [float] NULL,
	[HoursForReduction] [float] NULL,
	[ReductionHours] [float] NULL,
	[Lect] [real] NULL,
	[LectYear] [real] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
	[IsValid] [bit] NULL,
	[ExternalID] [nvarchar](100) NULL,
	[SchoolYear] [smallint] NOT NULL,
 CONSTRAINT [PK_PedStaffData] PRIMARY KEY CLUSTERED 
(
	[PedStaffDataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UC_PedStaffData_StaffPositionID_SchoolYear] UNIQUE NONCLUSTERED 
(
	[StaffPositionID] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[ProfileClass](
	[ProfileClassID] [int] IDENTITY(1,1) NOT NULL,
	[ParentClassID] [int] NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[ClassID] [int] NULL,
	[ProfSubjType] [int] NULL,
	[ProfSubjID] [int] NULL,
	[ValidFrom] [datetime2](7) NOT NULL,
	[ValidTo] [datetime2](7) NOT NULL,
	[SysUserID] [int] NULL,
 CONSTRAINT [PK_ProfileClass] PRIMARY KEY CLUSTERED 
(
	[ProfileClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[StaffPositionMainClass](
	[StaffPositionMainClassID] [int] IDENTITY(1,1) NOT NULL,
	[StaffPositionID] [int] NOT NULL,
	[MainClassID] [int] NULL,
	[ValidFrom] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[ValidTo] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
	[SysUserID] [int] NULL,
	[IsValid] [bit] NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_StaffPositionMainClass] PRIMARY KEY CLUSTERED 
(
	[StaffPositionMainClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [inst_year].[VacantStaff](
	[VacantStaffID] [int] IDENTITY(1,1) NOT NULL,
	[InstitutionID] [int] NOT NULL,
	[StaffTypeID] [int] NOT NULL,
	[NKPDPositionID] [int] NOT NULL,
	[PositionCount] [real] NULL,
	[PositionSubjectGroupID] [int] NULL,
	[SysUserID] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[IsValid] [bit] NOT NULL,
	[ExternalID] [nvarchar](100) NULL,
 CONSTRAINT [PK_VacantStaff] PRIMARY KEY CLUSTERED 
(
	[VacantStaffID] ASC,
	[InstitutionID] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ChangeYearData_PersonId] ON [inst_year].[ChangeYearData]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_ClassId_SchoolYear_InstitutionId_IsValid] ON [inst_year].[ClassGroup]
(
	[ClassID] ASC,
	[SchoolYear] ASC,
	[InstitutionID] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_ClassSpecialityId] ON [inst_year].[ClassGroup]
(
	[ClassSpecialityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_ClassTypeID_IsValid_ParentClassId_BasicClassID] ON [inst_year].[ClassGroup]
(
	[ClassTypeID] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC,
	[BasicClassID] ASC
)
INCLUDE ( 	[InstitutionID],
	[ClassName],
	[ClassEduFormID],
	[IsNotPresentForm]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_InstitutionDepartmentId] ON [inst_year].[ClassGroup]
(
	[InstitutionDepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_InstitutionId] ON [inst_year].[ClassGroup]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_InstitutionId_ClassTypeID] ON [inst_year].[ClassGroup]
(
	[ClassTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_InstitutionId_SchoolYear_InstitutionDeparmentId_IsValid] ON [inst_year].[ClassGroup]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC,
	[InstitutionDepartmentID] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_InstitutionId_SchoolYear_IsValid_ParentClassID] ON [inst_year].[ClassGroup]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_IsValid_ParentClassID_BasicClassID_IsNotPresentForm] ON [inst_year].[ClassGroup]
(
	[IsValid] ASC,
	[ParentClassID] ASC,
	[BasicClassID] ASC,
	[IsNotPresentForm] ASC
)
INCLUDE ( 	[InstitutionID],
	[ClassName],
	[ClassTypeID],
	[ClassEduFormID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_ParentClassId] ON [inst_year].[ClassGroup]
(
	[ParentClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_SchoolYear_InstitutionId_IsValid_ParentClassID_BasicClassID_ClassTypeID_IsNotPresentForm] ON [inst_year].[ClassGroup]
(
	[SchoolYear] ASC,
	[InstitutionID] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC,
	[BasicClassID] ASC,
	[ClassTypeID] ASC,
	[IsNotPresentForm] ASC
)
INCLUDE ( 	[ClassName],
	[EntranceLevelID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_SchoolYear_InstitutionId_IsValid_ParentClassID_BasicClassID_IsNotPresentForm] ON [inst_year].[ClassGroup]
(
	[SchoolYear] ASC,
	[InstitutionID] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC,
	[BasicClassID] ASC,
	[IsNotPresentForm] ASC
)
INCLUDE ( 	[ClassName],
	[ClassTypeID],
	[ClassEduFormID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_SchoolYear_InstitutionId_IsValid_ParentClassID_ClassShiftID_IsNotPresentForm] ON [inst_year].[ClassGroup]
(
	[SchoolYear] ASC,
	[InstitutionID] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC,
	[ClassShiftID] ASC,
	[IsNotPresentForm] ASC
)
INCLUDE ( 	[ClassName],
	[ClassTypeID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE NONCLUSTERED INDEX [IX_ClassGroup_SchoolYear_InstitutionId_IsValid_ParentClassId_IsNotPresentForm] ON [inst_year].[ClassGroup]
(
	[SchoolYear] ASC,
	[InstitutionID] ASC,
	[IsValid] ASC,
	[ParentClassID] ASC,
	[IsNotPresentForm] ASC
)
INCLUDE ( 	[ClassName],
	[AreaID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Curriculum_InstitutionDepartmentID] ON [inst_year].[Curriculum]
(
	[InstitutionDepartmentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Curriculum_InstitutionId_SchoolYear_IsValid] ON [inst_year].[Curriculum]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Curriculum_InstitutionId_SchoolYear_SubjectId] ON [inst_year].[Curriculum]
(
	[InstitutionID] ASC,
	[SchoolYear] ASC
)
INCLUDE ( 	[SubjectID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Curriculum_ParentCurriculumID] ON [inst_year].[Curriculum]
(
	[ParentCurriculumID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Curriculum_SchoolYear] ON [inst_year].[Curriculum]
(
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_CurriculumID_ClassID_SchoolYear_IsValid] ON [inst_year].[CurriculumClass]
(
	[CurriculumID] ASC,
	[ClassID] ASC,
	[SchoolYear] ASC
)
WHERE ([IsValid]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumClass_ClassID] ON [inst_year].[CurriculumClass]
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumClass_CurriculumId_SchoolYear_IsValid] ON [inst_year].[CurriculumClass]
(
	[CurriculumID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_CurriculumID_StudentID_SchoolYear_IsValid] ON [inst_year].[CurriculumStudent]
(
	[CurriculumID] ASC,
	[StudentID] ASC,
	[SchoolYear] ASC
)
WHERE ([IsValid]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumStudent_CurriculumID_SchoolYear_IsValid] ON [inst_year].[CurriculumStudent]
(
	[CurriculumID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumStudent_PersonID] ON [inst_year].[CurriculumStudent]
(
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumStudent_SchoolYear] ON [inst_year].[CurriculumStudent]
(
	[SchoolYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumStudent_StudentID] ON [inst_year].[CurriculumStudent]
(
	[StudentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumStudent_StudentID_SchoolYear] ON [inst_year].[CurriculumStudent]
(
	[CurriculumID] ASC
)
INCLUDE ( 	[StudentID],
	[SchoolYear]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [AK_CurriculumID_StaffPositionID_SchoolYear_IsValid] ON [inst_year].[CurriculumTeacher]
(
	[CurriculumID] ASC,
	[StaffPositionID] ASC,
	[SchoolYear] ASC
)
WHERE ([IsValid]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumTeacher_CurriculumID_SchoolYear_IsValid] ON [inst_year].[CurriculumTeacher]
(
	[CurriculumID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CurriculumTeacher_StaffPositionID_SchoolYear_IsValid] ON [inst_year].[CurriculumTeacher]
(
	[StaffPositionID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_InstitutionOtherData_InstitutionId] ON [inst_year].[InstitutionOtherData]
(
	[InstitutionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_PedStaffData_StaffPositionID_SchoolYear_IsValid] ON [inst_year].[PedStaffData]
(
	[StaffPositionID] ASC,
	[SchoolYear] ASC,
	[IsValid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProfileClass_ClassID] ON [inst_year].[ProfileClass]
(
	[ClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ProfileClass_ParentClassID] ON [inst_year].[ProfileClass]
(
	[ParentClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPositionMainClass_MainClassID] ON [inst_year].[StaffPositionMainClass]
(
	[MainClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_StaffPositionMainClass_StaffPositionID] ON [inst_year].[StaffPositionMainClass]
(
	[StaffPositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
