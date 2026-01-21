USE [neispuo]
GO
/****** Object:  StoredProcedure [reginst_basic].[institutionCreateUpdate]    Script Date: 7/15/2022 3:24:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [reginst_basic].[institutionCreateUpdate] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

DECLARE @instUpdateID int;
DECLARE @maxOrd234 int;
DECLARE @riProcedureForDeactivate int;
DECLARE @procStatusUpdate bit;
DECLARE @riProcUpdateID int;
DECLARE @sysuserid int;
DECLARE @procedureType int;
DECLARE @procedureTransformTypeID int;

/* ************ OperationType = 1 -> CREATE NEW PROCEDURE *************** */
IF (@OperationType = 1 OR @OperationType = 3)
BEGIN
	PRINT 'INFO: OperationType: 1 / 3 - CREATE / COMPLETE.'
	-- ---------- define context param2: sysuserid --------------
	--DECLARE @sysuserid int;
	SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
	WITH (   
			sysuserid int			'$.sysuserid'
	)
	--DECLARE @procedureType int;
	SELECT @procedureType = procedureTypeID FROM OPENJSON(@_json)
	WITH (   
				procedureTypeID int			'$.procedureTypeID'
	)
	
	/* ************ TransformType *************** */
	--DECLARE @procedureTransformTypeID int;
	SELECT @procedureTransformTypeID = procedureTransformTypeID FROM OPENJSON(@_json)
	WITH (   
				procedureTransformTypeID int			'$.procedureTransformTypeID'
	)

	/* ************ InstType *************** */
	DECLARE @instKind int;
	SELECT @instKind = instKind FROM OPENJSON(@_json)
	WITH (   
				instKind int			'$.instKind'
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

		---------------define param for procedureType = 1 -> maxOrd---------------
		DECLARE @maxOrd1 int;
		SELECT @maxOrd1 = max(Ord)+1
		FROM [neispuo].[reginst_basic].[RIProcedure]
		WHERE InstitutionID=@instID
		GROUP BY [InstitutionID]


		--DECLARE @procStatus bit;
		--IF (@OperationType = 3)  -- IF complete
		--	BEGIN
		--		SELECT @procStatus = 1;
		--	END
		--ELSE 
		--	SELECT @procStatus = 0;

		INSERT INTO [reginst_basic].[RIProcedure]
			([InstitutionID]
			,[ProcedureTypeID]
			,[ProcedureDate]
			,[YearDue]
			,[StatusTypeID]
			,[TransformTypeID]
			,[TransformDetails]
			,[IsActive]
			,[IsAnnuled]
			,[Ord]
			,[SysUserID]) 
		SELECT @instID, *, 1, 0 as IsAnnuled, ISNULL(@maxOrd1,1) as Ord, @sysuserid
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
			,[IsDataDue]
			,[SysUserID])
		SELECT @riProcID, @instID, *, @sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcID,*,1,@sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcID,*,0, @sysuserid
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
				   ,[CPLRAreaTypeID]
				   ,[SysUserID])
		SELECT @riProcID,*,@sysuserid
		FROM OPENJSON(@_json, '$.cplrAreas')
		WITH 
		(
				   CPLRAreaTypeID int			'$.cplrAreaType'
		)
		
		INSERT INTO [reginst_basic].[RIProfile]
				   ([RIprocedureID]
				   ,[BasicProfileID]
				   ,[SysUserID])
		SELECT @riProcID,*,@sysuserid
		FROM OPENJSON(@_json, '$.basicProfiles')
		WITH 
		(
				   BasicProfileID int			'$.schoolBasicProfile'
		)
				
		INSERT INTO [reginst_basic].[RISpeciality]
				   ([RIprocedureID]
				   ,[SpecialityID]
				   ,[SysUserID])
		SELECT @riProcID,*,@sysuserid
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
				   ,[PremInventory]
				   ,[SysUserID])
		SELECT @riProcID,*,@sysuserid
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
				   ,[DocumentFile]
				   ,[SysUserID])
		SELECT @riProcID,*,@sysuserid
		FROM OPENJSON(@_json, '$.InternalDocuments')
		WITH 
		(
					DocumentNo nvarchar(255)		'$.documentNumber',
					DocumentDate date				'$.documentDate',
					DocumentNotes nvarchar(2048)	'$.documentNotes',
					StateNewspaperData nvarchar(50) '$.stateNewspaperData',
					DocumentFile int				'$.fileId'
		)

		
		IF (@instKind=3)
			INSERT INTO [reginst_basic].[RICertificate]
				   ([RIprocedureID]
					,[CertificateNo]
					,[CertificateDate]
					,[CertificateNotes]
					,[GenerateCertificateFile]
					,[CertificateFileScaned]
					,[RIParentCertificateID]
					,[IsNonValid]
					,[ValidFrom]
					,[ValidTo]
					,[SysUserID])
		
			VALUES (
				@riProcID,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				NULL,
				0,	
				GETDATE(),
				GETDATE(),
				@sysuserid)

		SELECT @instID AS codeNEISPUO, @riProcID AS riProcedureID, @riInstID AS riInstitutionID, @primaryAddressID AS riPrimaryAddressID;

		IF (@procedureTransformTypeID=5 OR @procedureTransformTypeID=25)

			EXEC [reginst_basic].[neispuoInstitutionCreate] @riProcID  


	END  /* End of ProcedureType = 1 -> CREATE NEW INSTITUTION *************** */

	/* ************ ProcedureType = 2,3,4 -> CREATE NEW PROCEDURE *************** */
ELSE IF (@procedureType = 2) OR (@procedureType = 3) OR (@procedureType = 4)
	BEGIN
		PRINT 'INFO: ProcedureType = 2,3,4 -> CREATE NEW PROCEDURE.'

		--DECLARE @instUpdateID int;
		SELECT @instUpdateID = institutionID FROM OPENJSON(@_json)
		WITH (   
				   institutionID int			'$.codeNEISPUO'
		)
		
		---------------define param for procedureType IN (2,3,4) -> maxOrd---------------
		--DECLARE @maxOrd234 int;
		SELECT @maxOrd234 = max(Ord)+1
		FROM [neispuo].[reginst_basic].[RIProcedure]
		WHERE InstitutionID=@instUpdateID
		GROUP BY [InstitutionID]

			BEGIN
				SELECT @riProcedureForDeactivate = (SELECT RIProcedureID from reginst_basic.RIProcedure where InstitutionID = @instUpdateID and IsActive = 1);
				SELECT @procStatusUpdate = 1;
			END


		INSERT INTO [reginst_basic].[RIProcedure]
			([InstitutionID]
			,[ProcedureTypeID]
			,[ProcedureDate]
			,[YearDue]
			,[StatusTypeID]
			,[TransformTypeID]
			,[TransformDetails]
			,[IsActive]
			,[IsAnnuled]
			,[Ord]
			,[SysUserID])
		SELECT @instUpdateID, *, @procStatusUpdate as IsActive, 0 as IsAnnuled, ISNULL(@maxOrd234,1) as Ord, @sysuserid
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
		--DECLARE @riProcUpdateID int;
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
			,[IsDataDue]
			,[SysUserID])
		SELECT @riProcUpdateID, @instUpdateID, *, @sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,1, @sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,0, @sysuserid
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
				   ,[CPLRAreaTypeID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
		FROM OPENJSON(@_json, '$.cplrAreas')
		WITH 
		(
				   CPLRAreaTypeID int			'$.cplrAreaType'
		)
		
		INSERT INTO [reginst_basic].[RIProfile]
				   ([RIprocedureID]
				   ,[BasicProfileID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*, @sysuserid
		FROM OPENJSON(@_json, '$.basicProfiles')
		WITH 
		(
				   BasicProfileID int			'$.schoolBasicProfile'
		)
				
		INSERT INTO [reginst_basic].[RISpeciality]
				   ([RIprocedureID]
				   ,[SpecialityID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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
				   ,[PremInventory]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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
				   ,[DocumentFile]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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
/* ************ OperationType = 7 ->  Cancellation procedure *************** */
ELSE IF (@OperationType = 7 )
BEGIN
	DECLARE @CancelErrMsg NVARCHAR(4000);

	DECLARE @instCancelID int;
	SELECT @instCancelID = institutionID FROM OPENJSON(@_json)
										 WITH ( institutionID int	'$.codeNEISPUO');

	DECLARE @riCancelProcedureID INT;
	SET @riCancelProcedureID  = (SELECT RIProcedureID 
								   FROM reginst_basic.RIProcedure 
								  WHERE InstitutionID = @instCancelID and IsActive = 1);
	
	DECLARE @isFirstAndOnlyOne INT = 0;
	SET @isFirstAndOnlyOne = ( SELECT count(*) 	   
								 FROM reginst_basic.RIProcedure 
								WHERE InstitutionID = @instCancelID );
	

	DECLARE @isInstCreated INT = 0;
	SET @isInstCreated = ( SELECT count(*) 
						   FROM core.Institution WHERE InstitutionID = @instCancelID);

	DECLARE @riCancelProcOrd INT;
	DECLARE @riCancelProcYear INT;

	SELECT  @riCancelProcOrd  = Ord,
			@riCancelProcYear = YearDue 
	  FROM reginst_basic.RIProcedure 
	 WHERE RIprocedureID = @riCancelProcedureID;

	 DECLARE @riPrevProcID INT, @riPrevOrd INT, @riPrevProcYear INT;

	 SELECT  top(1) @riPrevProcID =  RIprocedureID, 
					@riPrevOrd = Ord, 
					@riPrevProcYear = YearDue
		FROM [reginst_basic].[RIProcedure]
		WHERE InstitutionID= @instCancelID and ord < @riCancelProcOrd
		ORDER  BY ord DESC;

	IF @isFirstAndOnlyOne <= 1 
		BEGIN
			IF @isInstCreated = 0 
				BEGIN

				-- PRINT 'INFO: Will deactivate old procedures with RIProcedureID=' + CAST(@riCancelProcedureID AS VARCHAR(12));

				UPDATE top (1) [reginst_basic].[RIProcedure]
					SET IsAnnuled = 1
					WHERE (RIProcedureID = @riCancelProcedureID);
				END
			ELSE 
				BEGIN
					SELECT @CancelErrMsg = CONCAT('Данни за институцията вече са налични в модул «Институции» на НЕИСПУО. ',
													'Процедурата не може да бъде анулирана. Моля, свържете се с разработващия екип');
				END
		END
	ELSE
		BEGIN
			IF @riCancelProcYear = @riPrevProcYear
				BEGIN
					UPDATE top (1) [reginst_basic].[RIProcedure]
					   SET IsAnnuled = 1,
							IsActive = 0
					 WHERE (RIProcedureID = @riCancelProcedureID);
				
					UPDATE 	top (1) [reginst_basic].[RIProcedure]
					   SET IsActive = 1
					 WHERE RIprocedureID = @riPrevProcID;
				END
			ELSE
				BEGIN 
					DECLARE @isInstSchoolYearCreated INT = 0;
					SET @isInstSchoolYearCreated = (SELECT count(*) 
													  FROM core.InstitutionSchoolYear 
													 WHERE InstitutionId = @instCancelID
													   and SchoolYear = @riPrevProcYear );
					IF @isInstSchoolYearCreated = 0 
						BEGIN
							UPDATE top (1) [reginst_basic].[RIProcedure]
							   SET IsAnnuled = 1,
									IsActive = 0
							 WHERE (RIProcedureID = @riCancelProcedureID);
				
							UPDATE 	top (1) [reginst_basic].[RIProcedure]
							   SET IsActive = 1
							 WHERE RIprocedureID = @riPrevProcID;

						END
					ELSE
						BEGIN
							SELECT @CancelErrMsg = CONCAT('Данни за институцията вече са налични в модул «Институции» на НЕИСПУО. ',
															'Процедурата не може да бъде анулирана. Моля, свържете се с разработващия екип.');
						END
				END
		END


END /* End of OperationType = 7 -> Cancellation procedure *************** */

/* ************ OperationType = 8 ->  Update procedure *************** */
ELSE IF (@OperationType = 8 )
BEGIN
		PRINT 'INFO: OperationType = 8 ->  Update procedure'

		-- DECLARE @instUpdateID int;
		SELECT @instUpdateID = institutionID FROM OPENJSON(@_json)
		WITH (   
				   institutionID int			'$.codeNEISPUO'
		)
		
		---------------define param for procedureType IN (2,3,4) -> maxOrd---------------
		-- DECLARE @maxOrd234 int;
		SELECT @maxOrd234 = max(Ord)+1
		FROM [neispuo].[reginst_basic].[RIProcedure]
		WHERE InstitutionID=@instUpdateID
		GROUP BY [InstitutionID]


			BEGIN
				SELECT @riProcedureForDeactivate = (SELECT RIProcedureID from reginst_basic.RIProcedure where InstitutionID = @instUpdateID and IsActive = 1);
				SELECT @procStatusUpdate = 1;
			END

		INSERT INTO [reginst_basic].[RIProcedure]
			([InstitutionID]
			,[ProcedureTypeID]
			,[ProcedureDate]
			,[YearDue]
			,[StatusTypeID]
			,[TransformTypeID]
			,[TransformDetails]
			,[IsActive]
			,[IsAnnuled]
			,[Ord]
			,[SysUserID])
		SELECT @instUpdateID, *, @procStatusUpdate as IsActive, 0 as IsAnnuled, ISNULL(@maxOrd234,1) as Ord, @sysuserid
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
		-- DECLARE @riProcUpdateID int;
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
			,[IsDataDue]
			,[SysUserID])
		SELECT @riProcUpdateID, @instUpdateID, *, @sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,1, @sysuserid
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
				   ,[IsMain]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,0, @sysuserid
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
				   ,[CPLRAreaTypeID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
		FROM OPENJSON(@_json, '$.cplrAreas')
		WITH 
		(
				   CPLRAreaTypeID int			'$.cplrAreaType'
		)
		
		INSERT INTO [reginst_basic].[RIProfile]
				   ([RIprocedureID]
				   ,[BasicProfileID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*, @sysuserid
		FROM OPENJSON(@_json, '$.basicProfiles')
		WITH 
		(
				   BasicProfileID int			'$.schoolBasicProfile'
		)
				
		INSERT INTO [reginst_basic].[RISpeciality]
				   ([RIprocedureID]
				   ,[SpecialityID]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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
				   ,[PremInventory]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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
				   ,[DocumentFile]
				   ,[SysUserID])
		SELECT @riProcUpdateID,*,@sysuserid
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

	
END /* ************ OperationType = 8 ->  Update procedure *************** */



/* ############################################################ */
ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal OperationType! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', instID=' + ISNULL(CAST(@instID AS VARCHAR(12)),'');

	THROW 99011, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 101, 'RIInstitution', null, null;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 101, 'RIInstitution', null, null;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
	DECLARE @ErrorNumber INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(),
		@ErrorNumber = ERROR_NUMBER(); 

	PRINT 'INFO: [RIInstitution] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
