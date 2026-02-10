USE [neispuo]
GO

/****** Object:  StoredProcedure [reginst_basic].[institutionCreateUpdate]    Script Date: 1.3.2021 ã. 1:35:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [reginst_basic].[institutionCreateUpdate] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS
BEGIN

/* ************ OperationType = 1 -> CREATE NEW PROCEDURE *************** */
IF (@OperationType = 1 OR @OperationType = 3)
BEGIN
	PRINT 'INFO: OperationType: 1 / 3 - CREATE / COMPLETE.'

	DECLARE @procedureType int;
	SELECT @procedureType = procedureTypeID FROM OPENJSON(@_json)
	WITH (   
				procedureTypeID int			'$.procedureTypeID'
	)

	/* ************ ProcedureType = 1 -> CREATE NEW INSTITUTION *************** */
	IF (@procedureType = 1)
	BEGIN
		PRINT 'INFO: ProcedureType = 1 -> CREATE NEW INSTITUTION.'
		DECLARE @primaryAddrRegion int;
		SELECT @primaryAddrRegion = primaryAddrRegionID FROM OPENJSON(@_json, '$.primaryAddress')
		WITH (   
					primaryAddrRegionID int			'$.primaryAddrRegion'
		)

		DECLARE @instID int;
		SELECT @instID = noms.getNewInstID(@primaryAddrRegion);

		DECLARE @procStatus bit;
		IF (@OperationType = 3)  -- IF complete
			BEGIN
				SELECT @procStatus = 1;
			END
		ELSE 
			SELECT @procStatus = 0;

		INSERT INTO [reginst_basic].[RIProcedure]
			([InstitutionID]
			,[ProcedureTypeID]
			,[ProcedureDate]
			,[YearDue]
			,[StatusTypeID]
			,[TransformTypeID]
			,[TransformDetails]
			,[IsActive])
		SELECT @instID, *, @procStatus as IsActive
		FROM OPENJSON(@_json)
		WITH (   
			ProcedureTypeID int				'$.procedureTypeID',  
			ProcedureDate datetime			'$.procedureDate',
			YearDue int						'$.procedureYearDue',
			StatusTypeID int				'$.procedureStatusTypeID',
			TransformTypeID int				'$.procedureTransformTypeID',
			TransformDetails nvarchar(4000)	'$.procedureTransformDetails'
		 ); 

		-- GET INSERTED RIProcedure ID (autoincrement)
		DECLARE @riProcID int;
		SELECT @riProcID = SCOPE_IDENTITY();

		-- INSERT INTO RIInstitution WITH @procID 
		INSERT INTO [reginst_basic].[RIInstitution]
			([RIprocedureID]
			,[InstitutionID]
			,[Bulstat]
			,[Name]
			,[Abbreviation]
			,[BaseSchoolTypeID]
			,[DetailedSchoolTypeID]
			,[FinancialSchoolTypeID]
			,[BudgetingSchoolTypeID]
			,[TRCountryID]
			,[TRTownID]
			,[TRLocalAreaID]
			,[TRAddress]
			,[TRPostCode]
			,[ReligInstDetails]
			,[HeadFirstName]
			,[HeadMiddleName]
			,[HeadLastName]
			,[IsNational]
			,[PersonnelCount]
			,[AuthProgram]
			,[IsDataDue])
		SELECT @riProcID, @instID, *
		FROM OPENJSON(@_json)
		WITH (   
			Bulstat nvarchar(13)			'$.bulstat',
			Name nvarchar(1024)				'$.name',
			Abbreviation nvarchar(255)		'$.abbreviation',
			BaseSchoolTypeID int			'$.baseSchoolTypeID',
			DetailedSchoolTypeID int		'$.detailedSchoolTypeID',
			FinancialSchoolTypeID int		'$.financialSchoolTypeID',
			BudgetingSchoolTypeID int		'$.budgetingSchoolTypeID',
			TRCountryID int					'$.settlementCountry',
			TRTownID int					'$.settlementTown',
			TRLocalAreaID int				'$.settlementLocalArea',
			TRAddress nvarchar(255)			'$.settlementAddress',
			TRPostCode int					'$.settlementPostCode',
			ReligInstDetails nvarchar(max)	'$.religInstDetails',
			HeadFirstName nvarchar(255)		'$.headFirstName',
			HeadMiddleName nvarchar(255)	'$.headMiddleName',
			HeadLastName nvarchar(255)		'$.headLastName',
			IsNational bit					'$.isNational',
			PersonnelCount int				'$.personnelCount',
			AuthProgram nvarchar(max)		'$.authProgram',
			IsDataDue bit					'$.isDataDue'
		 ) 

		-- GET INSERTED RIInstitution ID (autoincrement)
		DECLARE @riInstID int;
		SELECT @riInstID = SCOPE_IDENTITY();

		-- INSERT INTO RIInstitutionDepartment WITH @procID 
		INSERT INTO [reginst_basic].[RIInstitutionDepartment]
				   ([RIprocedureID]
				   ,[Name]
				   ,[CountryID]
				   ,[TownID]
				   ,[LocalAreaID]
				   ,[Address]
				   ,[PostCode]
				   ,[CadasterCode]
				   ,[Notes]
				   ,[IsMain])
		SELECT @riProcID,*,1
		FROM OPENJSON(@_json, '$.primaryAddress')
		WITH 
		(
				   Name nvarchar(1024)			'$.primaryAddrName',
				   CountryID int				'$.primaryAddrCountry',
				   TownID int					'$.primaryAddrTown',
				   LocalAreaID int				'$.primaryAddrLocalArea',
				   Address nvarchar(255)		'$.primaryAddrAddress',
				   PostCode int					'$.primaryAddrPostCode',
				   CadasterCode nvarchar(255)	'$.primaryAddrCadasterCode',
				   Notes nvarchar(255)			'$.primaryAddrNotes'
		)

		-- GET INSERTED RIInstitutionDepartment primaryAddress ID (autoincrement)
		DECLARE @primaryAddressID int;
		SELECT @primaryAddressID = SCOPE_IDENTITY();

		INSERT INTO [reginst_basic].[RIInstitutionDepartment]
				   ([RIprocedureID]
				   ,[Name]
				   ,[CountryID]
				   ,[TownID]
				   ,[LocalAreaID]
				   ,[Address]
				   ,[PostCode]
				   ,[CadasterCode]
				   ,[Notes]
				   ,[IsMain])
		SELECT @riProcID,*,0
		FROM OPENJSON(@_json, '$.institutionDepartments')
		WITH 
		(
				   Name nvarchar(255)			'$.departmentName',
				   CountryID int				'$.departmentCountry',
				   TownID int					'$.departmentTown',
				   LocalAreaID int				'$.departmentLocalArea',
				   Address nvarchar(255)		'$.departmentAddress',
				   PostCode int					'$.departmentPostalCode',
				   CadasterCode nvarchar(255)	'$.departmentCadasterCode',
				   Notes nvarchar(255)			'$.departmentNotes'
		)
		
		INSERT INTO [reginst_basic].[RICPLRArea]
				   ([RIprocedureID]
				   ,[CPLRAreaTypeID])
		SELECT @riProcID,*
		FROM OPENJSON(@_json, '$.cplrAreas')
		WITH 
		(
				   CPLRAreaTypeID int			'$.cplrAreaType'
		)
		
		INSERT INTO [reginst_basic].[RIProfile]
				   ([RIprocedureID]
				   ,[BasicProfileID])
		SELECT @riProcID,*
		FROM OPENJSON(@_json, '$.basicProfiles')
		WITH 
		(
				   BasicProfileID int			'$.schoolBasicProfile'
		)
				
		INSERT INTO [reginst_basic].[RISpeciality]
				   ([RIprocedureID]
				   ,[SpecialityID])
		SELECT @riProcID,*
		FROM OPENJSON(@_json, '$.SPPOOSpecialities')
		WITH 
		(
				   SpecialityID int				'$.SPPOOSpeciality'
		)
				
		INSERT INTO [reginst_basic].[RIPremInstitution]
				   ([RIprocedureID]
				   ,[PremInstitutionID]
				   ,[PremStudents]
				   ,[PremDocs]
				   ,[PremInventory])
		SELECT @riProcID,*
		FROM OPENJSON(@_json, '$.premInstitutions')
		WITH 
		(
					PremInstitutionID int		'$.premInstitution',
					PremStudents nvarchar(max)	'$.premStudents',
					PremDocs nvarchar(max)		'$.premDocuments',
					PremInventory nvarchar(max) '$.premInventory'
		)
				
		INSERT INTO [reginst_basic].[RIDocument]
				   ([RIprocedureID]
				   ,[DocumentNo]
				   ,[DocumentDate]
				   ,[DocumentNotes]
				   ,[StateNewspaperData]
				   ,[DocumentFile])
		SELECT @riProcID,*
		FROM OPENJSON(@_json, '$.InternalDocuments')
		WITH 
		(
					DocumentNo nvarchar(255)		'$.documentNumber',
					DocumentDate date				'$.documentDate',
					DocumentNotes nvarchar(2048)	'$.documentNotes',
					StateNewspaperData nvarchar(50) '$.stateNewspaperData',
					DocumentFile int				'$.fileId'
		)
		
		SELECT @instID AS codeNEISPUO, @riProcID AS riProcedureID, @riInstID AS riInstitutionID, @primaryAddressID AS riPrimaryAddressID, @procStatus AS procStatus;

	END  /* End of ProcedureType = 1 -> CREATE NEW INSTITUTION *************** */

	/* ************ ProcedureType = 2,3,4 -> CREATE NEW PROCEDURE *************** */
	IF (@procedureType = 2 OR @procedureType = 3 OR @procedureType = 4)
	BEGIN
		PRINT 'INFO: ProcedureType = 2,3,4 -> CREATE NEW PROCEDURE.'

		DECLARE @instUpdateID int;
		SELECT @instUpdateID = institutionID FROM OPENJSON(@_json)
		WITH (   
				   institutionID int			'$.codeNEISPUO'
		)

		DECLARE @riProcedureForDeactivate int;
		DECLARE @procStatusUpdate bit;
		IF (@OperationType = 3)  -- IF complete
			BEGIN
				SELECT @riProcedureForDeactivate = (SELECT RIProcedureID from reginst_basic.RIProcedure where InstitutionID = @instUpdateID and IsActive = 1);
				SELECT @procStatusUpdate = 1;
			END
		ELSE 
			SELECT @procStatusUpdate = 0;

		INSERT INTO [reginst_basic].[RIProcedure]
			([InstitutionID]
			,[ProcedureTypeID]
			,[ProcedureDate]
			,[YearDue]
			,[StatusTypeID]
			,[TransformTypeID]
			,[TransformDetails]
			,[IsActive])
		SELECT @instUpdateID, *, @procStatusUpdate as IsActive
		FROM OPENJSON(@_json)
		WITH (   
			ProcedureTypeID int				'$.procedureTypeID',  
			ProcedureDate datetime			'$.procedureDate',
			YearDue int						'$.procedureYearDue',
			StatusTypeID int				'$.procedureStatusTypeID',
			TransformTypeID int				'$.procedureTransformTypeID',
			TransformDetails nvarchar(4000)	'$.procedureTransformDetails'
		 ); 

		/* Deactive old procedure  */
		IF (@riProcedureForDeactivate IS NOT NULL)
		BEGIN
			PRINT 'INFO: Will deactivate old procedures with RIProcedureID=' + CAST(@riProcedureForDeactivate AS VARCHAR(12));
			UPDATE top (1) [reginst_basic].[RIProcedure]
				SET [IsActive] = 0
				WHERE (RIProcedureID = @riProcedureForDeactivate);
		END

		-- GET INSERTED RIProcedure ID (autoincrement)
		DECLARE @riProcUpdateID int;
		SELECT @riProcUpdateID = SCOPE_IDENTITY();

		/* DECLARE @riInstUpdateID int;
		SELECT @riInstUpdateID = riInstitutionID FROM OPENJSON(@_json)
		WITH (   
				   riInstitutionID int			'$.riInstitutionID'
		) */

		-- INSERT INTO RIInstitution WITH @procID 
		INSERT INTO [reginst_basic].[RIInstitution]
			([RIprocedureID]
			,[InstitutionID]
			,[Bulstat]
			,[Name]
			,[Abbreviation]
			,[BaseSchoolTypeID]
			,[DetailedSchoolTypeID]
			,[FinancialSchoolTypeID]
			,[BudgetingSchoolTypeID]
			,[TRCountryID]
			,[TRTownID]
			,[TRLocalAreaID]
			,[TRAddress]
			,[TRPostCode]
			,[ReligInstDetails]
			,[HeadFirstName]
			,[HeadMiddleName]
			,[HeadLastName]
			,[IsNational]
			,[PersonnelCount]
			,[AuthProgram]
			,[IsDataDue])
		SELECT @riProcUpdateID, @instUpdateID, *
		FROM OPENJSON(@_json)
		WITH (   
			Bulstat nvarchar(13)			'$.bulstat',
			Name nvarchar(1024)				'$.name',
			Abbreviation nvarchar(255)		'$.abbreviation',
			BaseSchoolTypeID int			'$.baseSchoolTypeID',
			DetailedSchoolTypeID int		'$.detailedSchoolTypeID',
			FinancialSchoolTypeID int		'$.financialSchoolTypeID',
			BudgetingSchoolTypeID int		'$.budgetingSchoolTypeID',
			TRCountryID int					'$.settlementCountry',
			TRTownID int					'$.settlementTown',
			TRLocalAreaID int				'$.settlementLocalArea',
			TRAddress nvarchar(255)			'$.settlementAddress',
			TRPostCode int					'$.settlementPostCode',
			ReligInstDetails nvarchar(max)	'$.religInstDetails',
			HeadFirstName nvarchar(255)		'$.headFirstName',
			HeadMiddleName nvarchar(255)	'$.headMiddleName',
			HeadLastName nvarchar(255)		'$.headLastName',
			IsNational bit					'$.isNational',
			PersonnelCount int				'$.personnelCount',
			AuthProgram nvarchar(max)		'$.authProgram',
			IsDataDue bit					'$.isDataDue'
		 ) 

		-- GET INSERTED RIInstitution ID (autoincrement)
		/* DECLARE @riInstID int;
		SELECT @riInstID = SCOPE_IDENTITY(); */

		-- INSERT INTO RIInstitutionDepartment WITH @procID 
		INSERT INTO [reginst_basic].[RIInstitutionDepartment]
				   ([RIprocedureID]
				   ,[Name]
				   ,[CountryID]
				   ,[TownID]
				   ,[LocalAreaID]
				   ,[Address]
				   ,[PostCode]
				   ,[CadasterCode]
				   ,[Notes]
				   ,[IsMain])
		SELECT @riProcUpdateID,*,1
		FROM OPENJSON(@_json, '$.primaryAddress')
		WITH 
		(
				   Name nvarchar(1024)			'$.primaryAddrName',
				   CountryID int				'$.primaryAddrCountry',
				   TownID int					'$.primaryAddrTown',
				   LocalAreaID int				'$.primaryAddrLocalArea',
				   Address nvarchar(255)		'$.primaryAddrAddress',
				   PostCode int					'$.primaryAddrPostCode',
				   CadasterCode nvarchar(255)	'$.primaryAddrCadasterCode',
				   Notes nvarchar(255)			'$.primaryAddrNotes'
		)

		-- GET INSERTED RIInstitutionDepartment primaryAddress ID (autoincrement)
		/* DECLARE @primaryAddressID int;
		SELECT @primaryAddressID = SCOPE_IDENTITY(); */

		INSERT INTO [reginst_basic].[RIInstitutionDepartment]
				   ([RIprocedureID]
				   ,[Name]
				   ,[CountryID]
				   ,[TownID]
				   ,[LocalAreaID]
				   ,[Address]
				   ,[PostCode]
				   ,[CadasterCode]
				   ,[Notes]
				   ,[IsMain])
		SELECT @riProcUpdateID,*,0
		FROM OPENJSON(@_json, '$.institutionDepartments')
		WITH 
		(
				   Name nvarchar(255)			'$.departmentName',
				   CountryID int				'$.departmentCountry',
				   TownID int					'$.departmentTown',
				   LocalAreaID int				'$.departmentLocalArea',
				   Address nvarchar(255)		'$.departmentAddress',
				   PostCode int					'$.departmentPostalCode',
				   CadasterCode nvarchar(255)	'$.departmentCadasterCode',
				   Notes nvarchar(255)			'$.departmentNotes'
		)
		
		INSERT INTO [reginst_basic].[RICPLRArea]
				   ([RIprocedureID]
				   ,[CPLRAreaTypeID])
		SELECT @riProcUpdateID,*
		FROM OPENJSON(@_json, '$.cplrAreas')
		WITH 
		(
				   CPLRAreaTypeID int			'$.cplrAreaType'
		)
		
		INSERT INTO [reginst_basic].[RIProfile]
				   ([RIprocedureID]
				   ,[BasicProfileID])
		SELECT @riProcUpdateID,*
		FROM OPENJSON(@_json, '$.basicProfiles')
		WITH 
		(
				   BasicProfileID int			'$.schoolBasicProfile'
		)
				
		INSERT INTO [reginst_basic].[RISpeciality]
				   ([RIprocedureID]
				   ,[SpecialityID])
		SELECT @riProcUpdateID,*
		FROM OPENJSON(@_json, '$.SPPOOSpecialities')
		WITH 
		(
				   SpecialityID int				'$.SPPOOSpeciality'
		)
				
		INSERT INTO [reginst_basic].[RIPremInstitution]
				   ([RIprocedureID]
				   ,[PremInstitutionID]
				   ,[PremStudents]
				   ,[PremDocs]
				   ,[PremInventory])
		SELECT @riProcUpdateID,*
		FROM OPENJSON(@_json, '$.premInstitutions')
		WITH 
		(
					PremInstitutionID int		'$.premInstitution',
					PremStudents nvarchar(max)	'$.premStudents',
					PremDocs nvarchar(max)		'$.premDocuments',
					PremInventory nvarchar(max) '$.premInventory'
		)
				
		INSERT INTO [reginst_basic].[RIDocument]
				   ([RIprocedureID]
				   ,[DocumentNo]
				   ,[DocumentDate]
				   ,[DocumentNotes]
				   ,[StateNewspaperData]
				   ,[DocumentFile])
		SELECT @riProcUpdateID,*
		FROM OPENJSON(@_json, '$.InternalDocuments')
		WITH 
		(
					DocumentNo nvarchar(255)		'$.documentNumber',
					DocumentDate date				'$.documentDate',
					DocumentNotes nvarchar(2048)	'$.documentNotes',
					StateNewspaperData nvarchar(50) '$.stateNewspaperData',
					DocumentFile int				'$.fileId'
		)

		SELECT @instUpdateID AS instUpdateID, @riProcUpdateID AS riProcUpdateID, @procStatusUpdate AS procStatusUpdate, @riProcedureForDeactivate AS riProcedureForDeactivate;

	END  /* End of ProcedureType = 2,3,4 -> CREATE NEW PROCEDURE *************** */

END  /* End of OperationType = 1 -> CREATE NEW PROCEDURE *************** */


END
GO


