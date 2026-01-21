USE [neispuo]
GO
/****** Object:  StoredProcedure [reginst_basic].[neispuoInstitutionCreate]    Script Date: 8.5.2022 г. 14:04:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [reginst_basic].[neispuoInstitutionCreate]
	-- Add the parameters for the stored procedure here
	--@InstitutionID INT
	@riProcID INT
AS
BEGIN TRY
	BEGIN TRANSACTION

	DECLARE @InstitutionID INT = (
			SELECT InstitutionID
			FROM RINEISPUOTransfer
			WHERE RIProcedureID = @riProcID
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
			LEFT JOIN inst_basic.EKATTE ek
				ON rt.TownID = ek.TownID
			WHERE RIprocedureID = @riProcID
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
	DECLARE @SOExtProviderID INT = null
	--(
	--		SELECT TOP (1) ExtSystemID
	--		FROM neispuo.core.ExtSystem
	--		WHERE IsValid = 1
	--		ORDER BY ExtSystemID
	--		)
	DECLARE @CBExtProviderID INT = null
	--(
	--		SELECT TOP (1) ExtSystemID
	--		FROM neispuo.core.ExtSystem
	--		WHERE IsValid = 1
	--		ORDER BY ExtSystemID
	--		)
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

	--CREATE Institution
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		-- Insert statements for procedure here
		/* ------------------------------------------------------
	 *    Insert into core.Institution
	 * ------------------------------------------------------ */
		INSERT INTO core.Institution (
			InstitutionID,
			Name,
			Abbreviation,
			Bulstat,
			CountryID,
			LocalAreaID,
			FinancialSchoolTypeID,
			DetailedSchoolTypeID,
			BudgetingSchoolTypeID,
			SysUserID,
			TownID,
			BaseSchoolTypeID
			)
		--,ValidFrom
		--,ValidTo
		SELECT InstitutionID,
			Name,
			Abbreviation,
			Bulstat,
			CountryID,
			LocalAreaID,
			FinancialSchoolTypeID,
			DetailedSchoolTypeID,
			BudgetingSchoolTypeID,
			@SysUserId,
			TownID,
			BaseSchoolTypeID
		--,@ValidFrom
		--,@ValidTo
		FROM RINEISPUOTransfer
		WHERE RIProcedureID = @riProcID

		/* ------------------------------------------------------
	 *    Insert into core.InstitutionSchoolYear
	 * ------------------------------------------------------ */
		INSERT INTO core.InstitutionSchoolYear (
			InstitutionId,
			SchoolYear,
			IsFinalized,
			Name,
			Abbreviation,
			Bulstat,
			CountryID,
			LocalAreaID,
			FinancialSchoolTypeID,
			DetailedSchoolTypeID,
			BudgetingSchoolTypeID,
			TownID,
			BaseSchoolTypeID,
			IsCurrent,
			SysUserID,
			ValidFrom,
			ValidTo
			)
		SELECT InstitutionId,
			YearDue,
			@IsFinalized,
			Name,
			Abbreviation,
			Bulstat,
			CountryID,
			LocalAreaID,
			FinancialSchoolTypeID,
			DetailedSchoolTypeID,
			BudgetingSchoolTypeID,
			TownID,
			BaseSchoolTypeID,
			@IsCurrent,
			@SysUserID,
			@ValidFrom,
			@ValidTo
		FROM reginst_basic.RINEISPUOTransfer
		WHERE RIprocedureID = @riProcID

		/* ------------------------------------------------------
	 *    Insert into inst_basic.InstitutionDepartment
	 * ------------------------------------------------------ */
		INSERT INTO inst_basic.InstitutionDepartment (
			InstitutionID,
			Name,
			CountryID,
			TownID,
			LocalAreaID,
			Address,
			PostCode,
			IsMain,
			ValidFrom,
			ValidTo,
			SysUserID,
			tmp_DepID,
			IsValid
			)
		SELECT InstitutionID,
			Name,
			CountryID,
			TownID,
			LocalAreaID,
			Address,
			PostCode,
			IsMain,
			ValidFrom,
			ValidTo,
			SysUserID,
			@tmp_DepID,
			@IsValid
		FROM RINEISPUOTransfer
		WHERE RIprocedureID = @riProcID

		/* ------------------------------------------------------
	 *    Insert into inst_basic.InstitutionDetail
	 * ------------------------------------------------------ */
		INSERT INTO inst_basic.InstitutionDetail (
			InstitutionID,
			Email,
			Website,
			EstablishedYear,
			ConstitActFirst,
			ConstitActLast,
			IsODZ,
			IsProfSchool,
			IsNational,
			IsProvideEduServ,
			IsDelegateBudget,
			IsNonIndDormitory,
			IsInternContract,
			IsAppInnovSystem,
			BankIBAN,
			BankBIC,
			BankName,
			BankAccountHolder,
			ValidFrom,
			ValidTo,
			SysUserID
			)
		SELECT InstitutionID,
			@Email,
			@Website,
			YearDue,
			@ConstitActFirst,
			@ConstitActLast,
			@IsODZ,
			@IsProfSchool,
			IsNational,
			@IsProvideEduServ,
			@IsDelegateBudget,
			@IsNonIndDormitory,
			@IsInternContract,
			@IsAppInnovSystem,
			@BankIBAN,
			@BankBIC,
			@BankName,
			@BankAccountHolder,
			@ValidFrom,
			@ValidTo,
			@SysUserID
		FROM reginst_basic.RINEISPUOTransfer
		WHERE RIprocedureID = @riProcID

		/* ------------------------------------------------------
	 *    Insert into inst_nom.InstAdminData
	 * ------------------------------------------------------ */
		INSERT INTO inst_nom.InstAdminData (
			SchoolYear,
			InstitutionID,
			IsInnovative,
			IsCentral,
			IsProtected,
			IsStateFunded,
			HasMunDecisionFor4
			)
		SELECT YearDue,
			InstitutionID,
			@IsInnovative,
			@IsCentral,
			@IsProtected,
			@IsStateFunded,
			@HasMunDecisionFor4
		FROM reginst_basic.RINEISPUOTransfer
		WHERE RIprocedureID = @riProcID

		/* ------------------------------------------------------
	 *    Insert into core.InstitutionConfData
	 * ------------------------------------------------------
	 */
		INSERT INTO core.InstitutionConfData (
			InstitutionID,
			SOVersion,
			CBVersion,
			SysUserID,
			SchoolYear,
			SOExtProviderID,
			CBExtProviderID
			)
		SELECT InstitutionID,
			@SOVersion,
			@CBVersion,
			@SysUserID,
			YearDue,
			@SOExtProviderID,
			@CBExtProviderID
		FROM reginst_basic.RINEISPUOTransfer
		WHERE RIprocedureID = @riProcID
			/* ------------------------------------------------------
	 *    Insert into inst_basic.DistanceLearning
	 * ------------------------------------------------------ 
	 */
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
			InstitutionID,
			Condition
			)
		SELECT inst.InstitutionID,
			cond.Condition
		FROM tblConditions cond
		FULL JOIN core.Institution inst
			ON 1 = 1
		WHERE inst.InstitutionID = @InstitutionID

		/* ------------------------------------------------------
	 *    Insert into inst_year.ClassGroup
	 * ------------------------------------------------------ 
	 */
		INSERT INTO inst_year.ClassGroup (
			SchoolYear,
			InstitutionID,
			InstitutionDepartmentID,
			ClassGroupNum,
			ClassName,
			ParalellClassName,
			ParentClassID,
			BasicClassID,
			ClassTypeID,
			AreaID,
			ClassEduFormID,
			ClassEduDurationID,
			ClassShiftID,
			BudgetingClassTypeID,
			EntranceLevelID,
			ClassSpecialityID,
			FLTypeID,
			FLID,
			IsProfModule,
			StudentCountPlaces,
			Notes,
			IsCombined,
			IsNoList,
			IsSpecNeed,
			IsWholeClass,
			IsNotPresentForm,
			ValidFrom,
			ValidTo,
			SysUserID,
			tmp_ClassID,
			ExternalID
			)
		SELECT YearDue,
			rb.InstitutionID,
			ib.InstitutionDepartmentID,
			@ClassGroupNum,
			@ClassName,
			@ParalellClassName,
			@ParentClassID,
			@BasicClassID,
			@ClassTypeID,
			@AreaID,
			@ClassEduFormID,
			@ClassEduDurationID,
			@ClassShiftID,
			@BudgetingClassTypeID,
			@EntranceLevelID,
			@ClassSpecialityID,
			@FLTypeID,
			@FLID,
			@IsProfModule,
			@StudentCountPlaces,
			@Notes,
			@IsCombined,
			@IsNoList,
			@IsSpecNeed,
			@IsWholeClass,
			@IsNotPresentForm,
			@ValidFrom,
			@ValidTo,
			@SysUserID,
			@tmp_ClassID,
			@ExternalID
		FROM reginst_basic.RINEISPUOTransfer rb
		JOIN inst_basic.InstitutionDepartment ib
			ON rb.InstitutionID = ib.InstitutionID
				AND ib.IsMain = 1
		WHERE RIprocedureID = @riProcID
	END


		--EXEC logs.logEvent @_json, 1, 103, 'neispuoInstitutionCreate', @riProcID, NULL;
	COMMIT TRANSACTION
END TRY

BEGIN CATCH
	ROLLBACK TRANSACTION

	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC logs.logError 'neispuoInstitutionCreate',
		1,
		103,
		'neispuoInstitutionCreate',
		@riProcID,
		NULL;

	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT @ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE();

	PRINT 'INFO: neispuoInstitutionCreate failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (
			@ErrorMessage,
			@ErrorSeverity,
			@ErrorState
			)
END CATCH
