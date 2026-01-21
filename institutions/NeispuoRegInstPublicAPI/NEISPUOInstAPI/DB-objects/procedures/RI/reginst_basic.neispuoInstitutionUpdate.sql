USE [neispuo]
GO
/****** Object:  StoredProcedure [reginst_basic].[neispuoInstitutionUpdate]    Script Date: 8.5.2022 г. 14:07:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER
	

 PROCEDURE [reginst_basic].[neispuoInstitutionUpdate]
	-- Add the parameters for the stored procedure here
	--@InstitutionID INT
	@RIProcedureID INT
AS
DECLARE @InstitutionID INT = (
		SELECT InstitutionID
		FROM RINEISPUOTransfer
		WHERE RIProcedureID = @RIProcedureID
		)
DECLARE @ValidFrom DATETIME2 = GetDate()
DECLARE @ValidTo DATETIME2 = (
		SELECT CONVERT(DATETIME2, '31/12/9999', 103)
		)
DECLARE @SysUserId INT = 1
DECLARE @IsFinalized BIT = 0
DECLARE @IsCurrent BIT = 1
DECLARE @Email NVARCHAR(50) = NULL
DECLARE @Website NVARCHAR(50) = NULL
DECLARE @ConstitActFirst NVARCHAR(1024) = NULL
DECLARE @ConstitActLast NVARCHAR(1024) = NULL
DECLARE @IsODZ BIT = 0
DECLARE @IsProfSchool BIT = 0
DECLARE @IsProvideEduServ BIT = 0
DECLARE @IsDelegateBudget BIT = 0
DECLARE @IsNonIndDormitory BIT = 0
DECLARE @IsInternContract BIT

SET @IsInternContract = (
		SELECT CASE 
				WHEN noms.getInstKind(detailedSchoolTypeID, financialSchoolTypeID, ek.RegionID) = 13
					THEN 1
				ELSE 0
				END
		FROM reginst_basic.RINEISPUOTransfer rt
		LEFT JOIN [inst_basic].[EKATTE] ek
			ON rt.TownID = ek.TownID
		WHERE RIprocedureID = @RIProcedureID
		)

DECLARE @IsAppInnovSystem BIT
DECLARE @BankIBAN NVARCHAR(255) = NULL
DECLARE @BankBIC NVARCHAR(255) = NULL
DECLARE @BankName NVARCHAR(255) = NULL
DECLARE @BankAccountHolder NVARCHAR(255) = NULL
DECLARE @IsInnovative BIT = 0
DECLARE @IsCentral BIT = 0
DECLARE @IsProtected BIT = 0
DECLARE @IsStateFunded BIT = 0
DECLARE @HasMunDecisionFor4 BIT = 0
DECLARE @SOVersion BIGINT = 0
DECLARE @CBVersion BIGINT = 0
DECLARE @SOExtProviderID INT = 0
DECLARE @CBExtProviderID INT = 0
DECLARE @tmp_DepID INT = 0
DECLARE @IsValid BIT = 1
--
DECLARE @ClassGroupNum INT = 0
DECLARE @ClassName NVARCHAR(255) = 'служебна'
DECLARE @ParalellClassName NVARCHAR(255) = NULL
DECLARE @ParentClassID INT = NULL
DECLARE @BasicClassID INT = NULL
DECLARE @ClassTypeID INT = NULL
DECLARE @AreaID INT = NULL
DECLARE @ClassEduFormID INT = NULL
DECLARE @ClassEduDurationID INT = NULL
DECLARE @ClassShiftID INT = NULL
DECLARE @BudgetingClassTypeID INT = NULL
DECLARE @EntranceLevelID INT = NULL
DECLARE @ClassSpecialityID INT = NULL
DECLARE @FLTypeID INT = NULL
DECLARE @FLID INT = NULL
DECLARE @IsProfModule BIT = NULL
DECLARE @StudentCountPlaces INT = NULL
DECLARE @Notes NVARCHAR(1024) = NULL
DECLARE @IsCombined BIT = NULL
DECLARE @IsNoList BIT = NULL
DECLARE @IsSpecNeed BIT = NULL
DECLARE @IsWholeClass BIT = 0
DECLARE @IsNotPresentForm BIT = 1
DECLARE @tmp_ClassID INT = NULL
DECLARE @ExternalID NVARCHAR(100) = NULL

BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Update statements for procedure here
	/* ------------------------------------------------------
	 *    Update core.Institution
	 * ------------------------------------------------------ */
	UPDATE core.Institution
	SET core.Institution.InstitutionID = rt.InstitutionID,
		core.Institution.Name = rt.Name,
		core.Institution.Abbreviation = rt.Abbreviation,
		core.Institution.Bulstat = rt.Bulstat,
		core.Institution.CountryID = rt.CountryID,
		core.Institution.LocalAreaID = rt.LocalAreaID,
		core.Institution.FinancialSchoolTypeID = rt.FinancialSchoolTypeID,
		core.Institution.DetailedSchoolTypeID = rt.DetailedSchoolTypeID,
		core.Institution.BudgetingSchoolTypeID = rt.BudgetingSchoolTypeID,
		core.Institution.TownID = rt.TownID,
		core.Institution.BaseSchoolTypeID = rt.BaseSchoolTypeID,
		core.Institution.SysUserID = rt.SysUserID
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN core.Institution ci
		ON ci.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 *    Update core.InstitutionSchoolYear
	 * ------------------------------------------------------ */
	UPDATE core.InstitutionSchoolYear
	SET core.InstitutionSchoolYear.InstitutionId = rt.InstitutionId,
		core.InstitutionSchoolYear.SchoolYear = rt.YearDue,
		core.InstitutionSchoolYear.IsFinalized = @IsFinalized,
		core.InstitutionSchoolYear.Name = rt.Name,
		core.InstitutionSchoolYear.Abbreviation = rt.Abbreviation,
		core.InstitutionSchoolYear.Bulstat = rt.Bulstat,
		core.InstitutionSchoolYear.CountryID = rt.CountryID,
		core.InstitutionSchoolYear.LocalAreaID = rt.LocalAreaID,
		core.InstitutionSchoolYear.FinancialSchoolTypeID = rt.FinancialSchoolTypeID,
		core.InstitutionSchoolYear.DetailedSchoolTypeID = rt.DetailedSchoolTypeID,
		core.InstitutionSchoolYear.BudgetingSchoolTypeID = rt.BudgetingSchoolTypeID,
		core.InstitutionSchoolYear.TownID = rt.TownID,
		core.InstitutionSchoolYear.BaseSchoolTypeID = rt.BaseSchoolTypeID,
		core.InstitutionSchoolYear.IsCurrent = @IsCurrent,
		core.InstitutionSchoolYear.SysUserID = rt.SysUserID,
		core.InstitutionSchoolYear.ValidFrom = rt.ValidFrom,
		core.InstitutionSchoolYear.ValidTo = rt.ValidTo
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN core.InstitutionSchoolYear
		ON core.InstitutionSchoolYear.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 *    Update inst_basic.InstitutionDepartment
	 * ------------------------------------------------------ */
	UPDATE inst_basic.InstitutionDepartment
	SET inst_basic.InstitutionDepartment.InstitutionID = rt.InstitutionID,
		inst_basic.InstitutionDepartment.Name = rt.Name,
		inst_basic.InstitutionDepartment.CountryID = rt.CountryID,
		inst_basic.InstitutionDepartment.TownID = rt.TownID,
		inst_basic.InstitutionDepartment.LocalAreaID = rt.LocalAreaID,
		inst_basic.InstitutionDepartment.Address = rt.Address,
		inst_basic.InstitutionDepartment.PostCode = rt.PostCode,
		inst_basic.InstitutionDepartment.IsMain = rt.IsMain,
		inst_basic.InstitutionDepartment.ValidFrom = rt.ValidFrom,
		inst_basic.InstitutionDepartment.ValidTo = rt.ValidTo,
		inst_basic.InstitutionDepartment.SysUserID = rt.SysUserID,
		inst_basic.InstitutionDepartment.tmp_DepID = @tmp_DepID,
		inst_basic.InstitutionDepartment.IsValid = @IsValid
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN inst_basic.InstitutionDepartment id
		ON id.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 *    Update inst_basic.InstitutionDetail
	 * ------------------------------------------------------ */
	UPDATE inst_basic.InstitutionDetail
	SET inst_basic.InstitutionDetail.InstitutionID = rt.InstitutionID,
		inst_basic.InstitutionDetail.Email = @Email,
		inst_basic.InstitutionDetail.Website = @Website,
		inst_basic.InstitutionDetail.EstablishedYear = rt.YearDue,
		inst_basic.InstitutionDetail.ConstitActFirst = @ConstitActFirst,
		inst_basic.InstitutionDetail.ConstitActLast = @ConstitActLast,
		inst_basic.InstitutionDetail.IsODZ = @IsODZ,
		inst_basic.InstitutionDetail.IsProfSchool = @IsProfSchool,
		inst_basic.InstitutionDetail.IsNational = rt.IsNational,
		inst_basic.InstitutionDetail.IsProvideEduServ = @IsProvideEduServ,
		inst_basic.InstitutionDetail.IsDelegateBudget = @IsDelegateBudget,
		inst_basic.InstitutionDetail.IsNonIndDormitory = @IsNonIndDormitory,
		inst_basic.InstitutionDetail.IsInternContract = @IsInternContract,
		inst_basic.InstitutionDetail.IsAppInnovSystem = @IsAppInnovSystem,
		inst_basic.InstitutionDetail.BankIBAN = @BankIBAN,
		inst_basic.InstitutionDetail.BankBIC = @BankBIC,
		inst_basic.InstitutionDetail.BankName = @BankName,
		inst_basic.InstitutionDetail.BankAccountHolder = @BankAccountHolder,
		inst_basic.InstitutionDetail.ValidFrom = @ValidFrom,
		inst_basic.InstitutionDetail.ValidTo = @ValidTo,
		inst_basic.InstitutionDetail.SysUserID = @SysUserID
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN inst_basic.InstitutionDetail id
		ON id.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 *    Update inst_nom.InstAdminData
	 * ------------------------------------------------------ */
	UPDATE inst_nom.InstAdminData
	SET inst_nom.InstAdminData.SchoolYear = rt.YearDue,
		inst_nom.InstAdminData.InstitutionID = rt.InstitutionID,
		inst_nom.InstAdminData.IsInnovative = @IsInnovative,
		inst_nom.InstAdminData.IsCentral = @IsCentral,
		inst_nom.InstAdminData.IsProtected = @IsProtected,
		inst_nom.InstAdminData.IsStateFunded = @IsStateFunded,
		inst_nom.InstAdminData.HasMunDecisionFor4 = @HasMunDecisionFor4
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN inst_nom.InstAdminData iad
		ON iad.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 *    Update core.InstitutionConfData
	 * ------------------------------------------------------
	 */
	UPDATE core.InstitutionConfData
	SET core.InstitutionConfData.InstitutionID = rt.InstitutionID,
		core.InstitutionConfData.SOVersion = @SOVersion,
		core.InstitutionConfData.CBVersion = @CBVersion,
		core.InstitutionConfData.SysUserID = @SysUserID,
		core.InstitutionConfData.SchoolYear = rt.YearDue,
		core.InstitutionConfData.SOExtProviderID = @SOExtProviderID,
		core.InstitutionConfData.CBExtProviderID = @CBExtProviderID
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN core.InstitutionConfData icd
		ON icd.InstitutionID = rt.InstitutionID
	WHERE RIprocedureID = @RIProcedureID

	/* ------------------------------------------------------
	 * Update inst_basic.DistanceLearning
	 *
	 * !!! To add an update if necessary
	 * ------------------------------------------------------ 
	 */
	/*
	;
	WITH tblConditions (Condition)
	AS (
		SELECT 'Осигурена онлайн достъпност в непрекъснат режим 365/7/24 до необходимите учебни материали и електронни модули за обучение и текущо оценяване, разположени в уеббазирана система за дистанционно обучение на специализирани сървъри с гарантиран високоскоростен достъп до интернет минимум 100 Мbps' AS Condition
		
		UNION
		
		SELECT 'Осигурена възможност на учениците за директна онлайн комуникация с учител за провеждане на консултации по учебните предмети за съответния клас и за текущи изпитвания по съответния предмет' AS Condition
		
		UNION
		
		SELECT 'Наличие на учители, квалифицирани за работа с уеббазирана система за дистанционно обучение и с електронното съдържание' AS Condition
		
		UNION
		
		SELECT 'Осигурена възможност за функциониране на уеббазирана система, както и на технически екип за контрол, мониторинг, поддръжка и оптимизация на системата и електронното съдържание, и обезпечаване на непрекъсваемост на процеса на дистанционно обучение' AS Condition
		)
	INSERT INTO inst_basic.DistanceLearningCondition (
		InstitutionID
		,Condition
		)
	SELECT inst.InstitutionID
		,cond.Condition
	FROM tblConditions cond
	FULL JOIN core.Institution inst ON 1 = 1
	WHERE inst.InstitutionID = @InstitutionID 
	*/
	/* ------------------------------------------------------
	 *    UPDATE inst_year.ClassGroup
	 * ------------------------------------------------------ 
	 */
	UPDATE inst_year.ClassGroup
	SET inst_year.ClassGroup.SchoolYear = rt.YearDue,
		inst_year.ClassGroup.InstitutionID = rt.InstitutionID,
		inst_year.ClassGroup.InstitutionDepartmentID = ib.InstitutionDepartmentID,
		inst_year.ClassGroup.ClassGroupNum = @ClassGroupNum,
		inst_year.ClassGroup.ClassName = @ClassName,
		inst_year.ClassGroup.ParalellClassName = @ParalellClassName,
		inst_year.ClassGroup.ParentClassID = @ParentClassID,
		inst_year.ClassGroup.BasicClassID = @BasicClassID,
		inst_year.ClassGroup.ClassTypeID = @ClassTypeID,
		inst_year.ClassGroup.AreaID = @AreaID,
		inst_year.ClassGroup.ClassEduFormID = @ClassEduFormID,
		inst_year.ClassGroup.ClassEduDurationID = @ClassEduDurationID,
		inst_year.ClassGroup.ClassShiftID = @ClassShiftID,
		inst_year.ClassGroup.BudgetingClassTypeID = @BudgetingClassTypeID,
		inst_year.ClassGroup.EntranceLevelID = @EntranceLevelID,
		inst_year.ClassGroup.ClassSpecialityID = @ClassSpecialityID,
		inst_year.ClassGroup.FLTypeID = @FLTypeID,
		inst_year.ClassGroup.FLID = @FLID,
		inst_year.ClassGroup.IsProfModule = @IsProfModule,
		inst_year.ClassGroup.StudentCountPlaces = @StudentCountPlaces,
		inst_year.ClassGroup.Notes = @Notes,
		inst_year.ClassGroup.IsCombined = @IsCombined,
		inst_year.ClassGroup.IsNoList = @IsNoList,
		inst_year.ClassGroup.IsSpecNeed = @IsSpecNeed,
		inst_year.ClassGroup.IsWholeClass = @IsWholeClass,
		inst_year.ClassGroup.IsNotPresentForm = @IsNotPresentForm,
		inst_year.ClassGroup.ValidFrom = @ValidFrom,
		inst_year.ClassGroup.ValidTo = @ValidTo,
		inst_year.ClassGroup.SysUserID = @SysUserID,
		inst_year.ClassGroup.tmp_ClassID = @tmp_ClassID,
		inst_year.ClassGroup.ExternalID = @ExternalID
	FROM reginst_basic.RINEISPUOTransfer rt
	JOIN inst_year.ClassGroup cg
		ON cg.InstitutionID = rt.InstitutionID
	JOIN inst_basic.InstitutionDepartment ib
		ON rt.InstitutionID = ib.InstitutionID
			AND ib.IsMain = 1
	WHERE RIprocedureID = @RIProcedureID
END

