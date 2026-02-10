USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[personCUD]    Script Date: 9/7/2022 10:23:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [inst_basic].[personCUD] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
PRINT 'INFO: PROCEDURE personCreateUpdateDRAFT started! Start initializing parameters...';
-- ---------- define context param: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int			'$.instid'
)


-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define Master object 1: [core].[Person] ID param --------------
DECLARE @personid int;
SELECT @personid = personID FROM OPENJSON(@_json)
WITH (   
		personID int			'$.personid'
)
-- ---------- define param: IsTerminatedContract --------------
DECLARE @isTerminatedContract bit;
SELECT @isTerminatedContract = isTerminatedContract FROM OPENJSON(@_json)
WITH (   
		isTerminatedContract bit			'$.isTerminatedContract'
);

DECLARE @StaffPositionVar TABLE  
(  
	id INT
);
DECLARE @StaffPositionString NVARCHAR(4000);

-- ---------- define Master object 2: [core].[EducationalState] ID param --------------
DECLARE @educationalStateID int;
/* SELECT @educationalStateID = educationalState FROM OPENJSON(@_json)
WITH (   
		educationalState int			'$.educationalStateID'
) */

-- ---------- define Master object 3: [inst_basic].[PersonDetail] ID param --------------
DECLARE @personDetailID int;
/* SELECT @personDetailID = personDetail FROM OPENJSON(@_json)
WITH (   
		personDetail int			'$.personDetailID'
) */

-- ---------- define Additional params --------------
DECLARE @operationResultType int; -- (0) Warning. It is sent only if forceOperation = 0; (-1) Error; (1) Successfully completed operation
DECLARE @forceOperation int; -- (0) no force action required, send warning to the customer; (1) operation is forced to be executed
DECLARE @hasRefDataCount int; -- sum of found referenced record (could be more tan one source)
DECLARE @messageCode int;

SELECT @forceOperation = ForceOperation FROM OPENJSON(@_json)
WITH (   
		forceOperation int
)

-- ---------- check for Ref Data --------------
DECLARE @hasRefDataCurriculumTeacher int;
SET @hasRefDataCurriculumTeacher = (SELECT COUNT(CurriculumTeacherID) FROM [inst_year].[CurriculumTeacher] CT
										JOIN [inst_basic].[StaffPosition] SP ON CT.StaffPositionID = SP.StaffPositionID
										WHERE SP.[PersonID] = @personid AND SP.InstitutionID=@instID);

DECLARE @hasRefDataPedStaffData int;
SET @hasRefDataPedStaffData = (SELECT COUNT(PedStaffDataID) FROM [inst_year].[PedStaffData] PSD
										JOIN [inst_basic].[StaffPosition] SP ON PSD.StaffPositionID = SP.StaffPositionID
										WHERE SP.[PersonID] = @personid AND SP.InstitutionID=@instID);

DECLARE @curriculumDeletedString NVARCHAR(4000);

DECLARE @firstName NVARCHAR(255), @middleName NVARCHAR(255), @lastName NVARCHAR(255);
SELECT @firstName = TRIM(firstName)
		,@middleName = TRIM(middleName)
		,@lastName = TRIM(lastName) FROM OPENJSON(@_json)
WITH (   
		firstName nvarchar(255)			'$.firstName'
		,middleName nvarchar(255)			'$.middleName'
		,lastName nvarchar(255)			'$.lastName'
);

-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID);

	--if the school used external data provider --- then it cannot write to the database
	--DECLARE @isExtDataProvider bit;
	--SELECT @isExtDataProvider = IIF(COUNT(SOExtProviderID)>0,1,0)
	--FROM [core].[InstitutionConfData] 
	--WHERE InstitutionID = @instID 
	--	AND SchoolYear = @schoolYear;

PRINT 'INFO: PROCEDURE personCUD started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12));

/* ############################## Start OperationType = 1 -> CREATE/UPDATE Person data ############################## */
IF ((@OperationType = 1) AND (@instID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 1 -> Will CREATE/UPDATE Person data with InstitutionID=' + CAST(@instID AS VARCHAR(12));
IF (@personid IS NULL) -- Should INSERT [Person, PersonDetail, EducationalState] object into the DB...
	BEGIN	
		DECLARE @ExternalID NVARCHAR(100) = NULL;

		--IF @isExtDataProvider = 1
		--BEGIN	
		--	SET @operationResultType = 2;
		--	SET @messageCode = 1023;
		--	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @instID AS instID; 

		--	COMMIT TRANSACTION
		--	RETURN
		--END
		PRINT 'INFO: OperationType = 1 -> Will INSERT [core].[Person] with personid=' + CAST(@personid AS VARCHAR(12));

		INSERT INTO [core].[Person]
			   ([FirstName]
			   ,[MiddleName]
			   ,[LastName]
			   ,[PermanentAddress]
			   ,[PermanentTownID]
			   ,[CurrentAddress]
			   ,[CurrentTownID]
			   ,[PersonalIDType]
			   ,[PersonalID]
			   ,[NationalityID]
			   ,[BirthDate]
			   ,[BirthPlaceTownID]
			   ,[BirthPlaceCountry]
			   ,[Gender]
			   ,[PublicEduNumber]
			   --,[SysUserID]
			   )
		SELECT @firstName, @middleName, @lastName,*, 
		CONVERT(nvarchar,CHECKSUM(NEWID())) -- RANDOM PUBLIC EDUNumber
		-- , @sysuserid 
		FROM OPENJSON(@_json)
		WITH (   
				permanentAddress nvarchar(2048)	'$.permanentAddress'
				,permanentTown int					'$.permanentTown'
				,currentAddress nvarchar(2048)		'$.currentAddress'
				,currentTown int					'$.currentTown'
				--,publicEduNumber bigint				'$.publicEduNumber'
				,personalIDType int					'$.personalIDType'
				,personalID nvarchar(255)			'$.personalID'
				,nationalityID int					'$.nationalityID'
				,birthDate date						'$.birthDate'
				,birthPlaceTownID int				'$.birthPlaceTownID'
				,birthPlaceCountry int				'$.birthPlaceCountry'
				,gender int							'$.gender'
			);
		SELECT @personid = SCOPE_IDENTITY();
		PRINT 'INFO: [core].[Person] INSERTED successfully with id=' + CAST(@personid AS VARCHAR(12));

		PRINT 'INFO: OperationType = 1 -> Will INSERT [core].[EducationalState] with personid=' + CAST(@personid AS VARCHAR(12));
		INSERT INTO [core].[EducationalState]
				   ([PersonID]
				   ,[InstitutionID]
				   ,[PositionID])
		VALUES (@personid, @instID, 2);

		SELECT @educationalStateID = SCOPE_IDENTITY();
		PRINT 'INFO: [core].[EducationalState] INSERTED successfully with id=' + CAST(@educationalStateID AS VARCHAR(12));

		PRINT 'INFO: OperationType = 1 -> Will INSERT [inst_basic].[PersonDetail] with personid=' + CAST(@personid AS VARCHAR(12));
		INSERT INTO [inst_basic].[PersonDetail]
				   ([PersonID]
				   ,[Title]
				   ,[PhoneNumber]
				   ,[IsExtendStudent]
				   ,[IsPensioneer]
				   ,[Email]
				   ,[SysUserID]
				   )
		SELECT @personid, *, @sysuserid
		FROM OPENJSON(@_json)
		WITH (   
			 title nvarchar(100)		'$.title'
			,phoneNumber nvarchar(100)	'$.phoneNumber'
			,isExtendStudent bit		'$.isExtendStudent'
			,isPensioneer bit			'$.isPensioneer'
			,email nvarchar(100)		'$.email'
			);
		SELECT @personDetailID = SCOPE_IDENTITY();
		PRINT 'INFO: [PersonDetail] INSERTED successfully with id=' + CAST(@personDetailID AS VARCHAR(12));

	END
ELSE -- @personid IS NOT NULL => Should UPDATE [Person, PersonDetail] objects into the DB... 
	BEGIN			
	--	IF @isExtDataProvider = 1
	--	BEGIN	
	--		DECLARE @personDetailExist int;
	--		SELECT @personDetailExist = COUNT(PersonDetailID)
	--		FROM [inst_basic].[PersonDetail] pd
	--		WHERE PersonID = @personid
			
	--		IF @personDetailExist = 0 
	--			BEGIN
	--				PRINT 'INFO: OperationType = 1 -> Will INSERT [inst_basic].[PersonDetail] with personid=' + CAST(@personid AS VARCHAR(12));
	--				INSERT INTO [inst_basic].[PersonDetail]
	--						   ([PersonID]
	--						   ,[Title]
	--						   ,[PhoneNumber]
	--						   ,[IsExtendStudent]
	--						   ,[IsPensioneer]
	--						   ,[Email]
	--						   ,[SysUserID]
	--						   )
	--				SELECT @personid, *, @sysuserid
	--				FROM OPENJSON(@_json)
	--				WITH (   
	--					 title nvarchar(100)		'$.title'
	--					,phoneNumber nvarchar(100)	'$.phoneNumber'
	--					,isExtendStudent bit		'$.isExtendStudent'
	--					,isPensioneer bit			'$.isPensioneer'
	--					,email nvarchar(100)		'$.email'
	--					);
	--			END
	--		ELSE
	--			BEGIN
	--				UPDATE top (1) [inst_basic].[PersonDetail]
	--				SET  [Title] = JPD.title
	--					,[PhoneNumber] = JPD.phoneNumber
	--					,[IsExtendStudent] = JPD.isExtendStudent
	--					,[IsPensioneer] = JPD.isPensioneer
	--					,[Email] = JPD.email
	--					,[SysUserID] = @sysuserid
	--				FROM OPENJSON(@_json)
	--				WITH (   
	--					 title nvarchar(100)		
	--					,phoneNumber nvarchar(100)	
	--					,isExtendStudent bit
	--					,isPensioneer bit
	--					,email nvarchar(100)
	--					) JPD
	--				WHERE
	--					[inst_basic].[PersonDetail].PersonID = @personid;
	--				PRINT 'INFO: [core].[PersonDetail] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));
	--			END
	--		PRINT 'INFO: OperationType = 1 -> Will INSERT [core].[EducationalState] with personid=' + CAST(@personid AS VARCHAR(12));
	--		INSERT INTO [core].[EducationalState]
	--				   ([PersonID]
	--				   ,[InstitutionID]
	--				   ,[PositionID])
	--		VALUES (@personid, @instID, 2);

	--		SELECT @educationalStateID = SCOPE_IDENTITY();
	--		PRINT 'INFO: [core].[EducationalState] INSERTED successfully with id=' + CAST(@educationalStateID AS VARCHAR(12));
	   
	--		SELECT @personid AS personid, @instID AS instID, @educationalStateID AS educationalStateID; -- return back to the caller i.e. front-end
	--	END
	--ELSE
	--	BEGIN
			PRINT 'INFO: OperationType = 1 -> Will UPDATE master objects: [Person, PersonDetail] with personid='+ CAST(@personid AS VARCHAR(12));
			UPDATE top (1) [core].[Person]
			SET  [FirstName] = @firstName
				,[MiddleName] = @middleName
				,[LastName] = @lastName
				,[PermanentAddress] = JP.permanentAddress
				,[PermanentTownID] = JP.permanentTown
				,[CurrentAddress] = JP.currentAddress
				,[CurrentTownID] = JP.currentTown
				,[PublicEduNumber] = JP.publicEduNumber
				,[PersonalIDType] = JP.personalIDType
				,[PersonalID] = JP.personalID
				,[NationalityID] = JP.nationalityID
				,[BirthDate] = JP.birthDate
				,[BirthPlaceTownID] = JP.birthPlaceTownID
				,[BirthPlaceCountry] = JP.birthPlaceCountry
				,[Gender] = JP.gender
				--,[SysUserID] = @sysuserid
			FROM OPENJSON(@_json)
			WITH (   
				permanentAddress nvarchar(2048)
				,permanentTown int				
				,currentAddress nvarchar(2048)	
				,currentTown int				
				,publicEduNumber nvarchar(max)			
				,personalIDType int				
				,personalID nvarchar(255)
				,nationalityID int
				,birthDate date					
				,gender int	
				,birthPlaceTownID int
				,birthPlaceCountry int
				) JP				 
			WHERE
				[core].[Person].PersonID = @personid;
			PRINT 'INFO: [core].[Person] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));

			DECLARE @personDetailExist int;
			SELECT @personDetailExist = COUNT(PersonDetailID)
			FROM [inst_basic].[PersonDetail] pd
			WHERE PersonID = @personid
			
			IF @personDetailExist = 0 
				BEGIN
					PRINT 'INFO: OperationType = 1 -> Will INSERT [inst_basic].[PersonDetail] with personid=' + CAST(@personid AS VARCHAR(12));
					INSERT INTO [inst_basic].[PersonDetail]
							   ([PersonID]
							   ,[Title]
							   ,[PhoneNumber]
							   ,[IsExtendStudent]
							   ,[IsPensioneer]
							   ,[Email]
							   ,[SysUserID]
							   )
					SELECT @personid, *, @sysuserid
					FROM OPENJSON(@_json)
					WITH (   
						 title nvarchar(100)		'$.title'
						,phoneNumber nvarchar(100)	'$.phoneNumber'
						,isExtendStudent bit		'$.isExtendStudent'
						,isPensioneer bit			'$.isPensioneer'
						,email nvarchar(100)		'$.email'
						);
				END
			ELSE
				BEGIN
					UPDATE top (1) [inst_basic].[PersonDetail]
					SET  [Title] = JPD.title
						,[PhoneNumber] = JPD.phoneNumber
						,[IsExtendStudent] = JPD.isExtendStudent
						,[IsPensioneer] = JPD.isPensioneer
						,[Email] = JPD.email
						,[SysUserID] = @sysuserid
					FROM OPENJSON(@_json)
					WITH (   
						 title nvarchar(100)		
						,phoneNumber nvarchar(100)	
						,isExtendStudent bit
						,isPensioneer bit
						,email nvarchar(100)
						) JPD
					WHERE
						[inst_basic].[PersonDetail].PersonID = @personid;
					PRINT 'INFO: [core].[PersonDetail] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));
				END
			PRINT 'INFO: OperationType = 1 -> Will INSERT [core].[EducationalState] with personid=' + CAST(@personid AS VARCHAR(12));
			INSERT INTO [core].[EducationalState]
					   ([PersonID]
					   ,[InstitutionID]
					   ,[PositionID])
			VALUES (@personid, @instID, 2);

			SELECT @educationalStateID = SCOPE_IDENTITY();
			PRINT 'INFO: [core].[EducationalState] INSERTED successfully with id=' + CAST(@educationalStateID AS VARCHAR(12));

	   
			SELECT @personid AS personid, @instID AS instID, @educationalStateID AS educationalStateID; -- return back to the caller i.e. front-end
		END
	--END
END  /* End OperationType = 1 -> CREATE/UPDATE Person data *************** */

/* ############################## Start OperationType = 2 -> UPDATE Person data ############################## */
ELSE IF ((@OperationType = 2) AND (@instID IS NOT NULL) AND (@personid IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Person data with personid=' + CAST(@personid AS VARCHAR(12));

	IF @isTerminatedContract = 1
		BEGIN
			--check whether the teacher has active positions
			DECLARE @hasActivePositions int;
			SELECT @hasActivePositions = COUNT(StaffPositionID) FROM inst_basic.StaffPosition WHERE PersonID = @personid AND InstitutionID = @instID AND IsValid = 1

			IF @hasActivePositions = 0
				BEGIN
					DELETE FROM [core].[EducationalState]
					WHERE [EducationalState].PersonID = @personid AND [EducationalState].InstitutionID=@instID;
					PRINT 'INFO: [EducationalState] DELETED for personid=' + CAST(@personid AS VARCHAR(12));
				
					COMMIT TRANSACTION
					RETURN --exit procedure
				END
			ELSE
				BEGIN					
					SET @operationResultType = 0;
					SET @messageCode = 1020;
					SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personid AS personid;
 
					COMMIT TRANSACTION
					RETURN --exit procedure
				END
		END
	--IF @isExtDataProvider =1 
	--	BEGIN
	--		UPDATE top (1) [inst_basic].[PersonDetail]
	--		SET [Title] = JPD.title
	--			,[PhoneNumber] = JPD.phoneNumber
	--			,[IsExtendStudent] = JPD.isExtendStudent
	--			,[IsPensioneer] = JPD.isPensioneer
	--			,[Email] = JPD.email
	--			,[SysUserID] = @sysuserid
	--		FROM OPENJSON(@_json)
	--		WITH (   
	--				title nvarchar(100)		
	--			,phoneNumber nvarchar(100)	
	--			,isExtendStudent bit
	--			,isPensioneer bit
	--			,email nvarchar(100)
	--			) JPD
	--		WHERE
	--			[inst_basic].[PersonDetail].PersonID = @personid;
	--		PRINT 'INFO: [core].[PersonDetail] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));
	   
	--		SELECT @personid AS personid, @instID AS instID; -- return back to the caller i.e. front-end
	--	END
	--ELSE
	--	BEGIN
			UPDATE top (1) [core].[Person]
			SET [FirstName] = @firstName
				,[MiddleName] = @middleName
				,[LastName] = @lastName
				,[PermanentAddress] = JP.permanentAddress
				,[PermanentTownID] = JP.permanentTown
				,[CurrentAddress] = JP.currentAddress
				,[CurrentTownID] = JP.currentTown
				,[PublicEduNumber] = JP.publicEduNumber
				,[PersonalIDType] = JP.personalIDType
				,[PersonalID] = JP.personalID
				,[NationalityID] = JP.nationalityID
				,[BirthDate] = JP.birthDate
				,[BirthPlaceTownID] = JP.birthPlaceTownID
				,[BirthPlaceCountry] = JP.birthPlaceCountry
				,[Gender] = JP.gender
				--,[SysUserID] =@sysuserid
			FROM OPENJSON(@_json)
			WITH (   
				permanentAddress nvarchar(2048)
				,permanentTown int				
				,currentAddress nvarchar(2048)	
				,currentTown int				
				,publicEduNumber nvarchar(max)			
				,personalIDType int				
				,personalID nvarchar(255)	
				,nationalityID int
				,birthDate date					
				,gender int	
				,birthPlaceTownID int
				,birthPlaceCountry int
				) JP				 
			WHERE
				[core].[Person].PersonID = @personid;
			PRINT 'INFO: [core].[Person] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));

			UPDATE top (1) [inst_basic].[PersonDetail]
			SET 
					[Title] = JPD.title
				,[PhoneNumber] = JPD.phoneNumber
				,[IsExtendStudent] = JPD.isExtendStudent
				,[IsPensioneer] = JPD.isPensioneer
				,[Email] = JPD.email
				,[SysUserID] = @sysuserid
			FROM OPENJSON(@_json)
			WITH (   
					title nvarchar(100)		
				,phoneNumber nvarchar(100)	
				,isExtendStudent bit
				,isPensioneer bit
				,email nvarchar(100)
				) JPD
			WHERE
				[inst_basic].[PersonDetail].PersonID = @personid;
			PRINT 'INFO: [core].[PersonDetail] UPDATED successfully with id=' + CAST(@personid AS VARCHAR(12));
	   
			SELECT @personid AS personid, @instID AS instID; -- return back to the caller i.e. front-end
		--END
END  /* End OperationType = 2 -> UPDATE Person data *************** */
/* ############################################################ */

/* ############################## Start OperationType = 3 -> DELETE Person ############################## */
ELSE IF ((@OperationType = 3) AND (@instID IS NOT NULL) AND (@personid IS NOT NULL) AND (@forceOperation IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [core].[Person] with personid=' + CAST(@personid AS VARCHAR(12));
	
	--check if teacher has positions
	DECLARE @hasTeacherActivePositions bit;
	SELECT @hasTeacherActivePositions = COUNT(1) 
	FROM [inst_basic].[StaffPosition] 
	WHERE [StaffPosition].PersonID = @personid 
		AND [StaffPosition].InstitutionID = @instID 
		--AND [StaffPosition].IsValid = 1

	IF @hasTeacherActivePositions > 0
		BEGIN
			PRINT 'INFO: the teacher has active positions';
			SET @operationResultType = 2;
			SET @messageCode = 1022;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personid AS personid;
		
			COMMIT TRANSACTION
			RETURN -- exit procedure
		END

	/* FIRST, check for referenced data */
	PRINT 'INFO: Will calculate referenced data records in: [CurriculumTeacher], [PedStaffData] with personid=' + CAST(@personid AS VARCHAR(12));
		
	SET @hasRefDataCount = @hasRefDataCurriculumTeacher + @hasRefDataPedStaffData;

	PRINT 'INFO: CHECK for referenced data found in [CurriculumTeacher], [PedStaffData] in total: ' + CAST(@hasRefDataCount AS VARCHAR(12)) + ' records';
	
	IF (@hasRefDataCount = 0) -- there is NO referenced data, will delete [EducationalState] and [CurriculumTeacher]
	BEGIN
		PRINT 'INFO: There are no references to [CurriculumTeacher], [PedStaffData] with personid=' + CAST(@personid AS VARCHAR(12)) + ' [EducationalState] and [CurriculumTeacher] records will be deleted...';
		
		IF (@forceOperation = 0) -- to synch DELETE with call to Azure - first operation is Azure and if successfully done, do here
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with generic message to synch with Azure call. Do nothing for now!';
			SET @operationResultType = 0;
			SET @messageCode = 1018;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personid AS personid;
		
			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done after Azure call is done successfully, now do DELETE here
		BEGIN			
			UPDATE [inst_basic].[StaffPosition] SET [inst_basic].[StaffPosition].IsValid = 0
				OUTPUT INSERTED.StaffPositionID INTO @StaffPositionVar
				WHERE [inst_basic].[StaffPosition].PersonID = @personid AND [inst_basic].[StaffPosition].InstitutionID = @instID ;
			PRINT 'INFO: [inst_basic].[StaffPosition] with PersonID=' + CAST(@personid AS VARCHAR(12)) + ' set to isValid=0 successfully';	

			SET @StaffPositionString = (SELECT STRING_AGG(id, ',') FROM @StaffPositionVar);

			UPDATE [inst_year].[StaffPositionMainClass] SET [inst_year].[StaffPositionMainClass].IsValid = 0
				WHERE [inst_year].[StaffPositionMainClass].StaffPositionID IN (SELECT id FROM @StaffPositionVar);
			PRINT 'INFO: [inst_basic].[StaffPositionMainClass] for StaffPositionID=' + @StaffPositionString + ' set to isValid=0 successfully';	

			UPDATE [inst_year].[PedStaffData] SET [inst_year].[PedStaffData].IsValid = 0
				WHERE [inst_year].[PedStaffData].StaffPositionID IN (SELECT id FROM @StaffPositionVar);
			PRINT 'INFO: [inst_year].[PedStaffData] for StaffPositionID=' + @StaffPositionString + ' set to isValid=0 successfully';	

			-- ------------------------------------- DELETE record [EducationalState]  --------------------------
			PRINT 'INFO: @forceOperation = 1 => [EducationalState] records will be deleted...';
					
			DELETE FROM [core].[EducationalState]
				WHERE [EducationalState].PersonID = @personid AND [EducationalState].InstitutionID=@instID;
			PRINT 'INFO: [EducationalState] DELETED for personid=' + CAST(@personid AS VARCHAR(12));

			-- ------------------------------------- DELETE record [student].[SelfGovernment]  --------------------------
			DELETE FROM [student].[SelfGovernment]
				WHERE [student].[SelfGovernment].PersonId=@personid AND [student].[SelfGovernment].InstitutionId = @instID;
			PRINT 'INFO: [student].[SelfGovernment] DELETED for personid=' + CAST(@personid AS VARCHAR(12)) + ' through [StaffPosition] where HelpStudentGovernment = 1';

			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END
	ELSE IF (@hasRefDataCount > 0) -- there is referenced data, will ask for confirmation with specific question for referral data
	BEGIN
		PRINT 'INFO: There are references to [CurriculumTeacher], [PedStaffData] with personid=' + CAST(@personid AS VARCHAR(12));
		IF (@forceOperation = 0) -- need to ask for operation confirmation IF there is referenced data
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with specific message. Do nothing now!';
			SET @operationResultType = 0;
			SET @messageCode = 1002;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personid AS personid;
		
			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done, do specific operations based on ref. data
		BEGIN
			PRINT 'INFO: @forceOperation = 1 => StaffPosition, StaffPositionMainClass and PedStaffData records will be set to isValid=0 for personid=' + CAST(@personid AS VARCHAR(12));
			
			UPDATE [inst_basic].[StaffPosition] SET [inst_basic].[StaffPosition].IsValid=0
				OUTPUT INSERTED.StaffPositionID INTO @StaffPositionVar
				WHERE [inst_basic].[StaffPosition].PersonID = @personid AND [inst_basic].[StaffPosition].InstitutionID=@instID;
			PRINT 'INFO: [inst_basic].[StaffPosition] with PersonID=' + CAST(@personid AS VARCHAR(12)) + ' set to isValid=0 successfully';	

			SET @StaffPositionString = (SELECT STRING_AGG(id, ',') FROM @StaffPositionVar);

			UPDATE [inst_year].[StaffPositionMainClass] SET [inst_year].[StaffPositionMainClass].IsValid=0
				WHERE [inst_year].[StaffPositionMainClass].StaffPositionID IN (SELECT id FROM @StaffPositionVar);
			PRINT 'INFO: [inst_basic].[StaffPositionMainClass] for StaffPositionID=' + @StaffPositionString + ' set to isValid=0 successfully';	

			UPDATE [inst_year].[PedStaffData] SET [inst_year].[PedStaffData].IsValid=0
				WHERE [inst_year].[PedStaffData].StaffPositionID IN (SELECT id FROM @StaffPositionVar);
			PRINT 'INFO: [inst_year].[PedStaffData] for StaffPositionID=' + @StaffPositionString + ' set to isValid=0 successfully';	

			-- ------------------------------------- DELETE records [EducationalState] and [CurriculumTeacher] --------------------------
			PRINT 'INFO: @forceOperation = 1 => [EducationalState] and [CurriculumTeacher] records will be deleted...';
					
			SELECT @curriculumDeletedString = (SELECT STRING_AGG(CurriculumID, ',')  
				FROM [inst_year].[CurriculumTeacher]
				WHERE [CurriculumTeacher].CurriculumTeacherID IN (
					SELECT (CurriculumTeacherID) FROM [inst_year].[CurriculumTeacher] CT
						JOIN [inst_basic].[StaffPosition] SP ON CT.StaffPositionID = SP.StaffPositionID WHERE SP.[PersonID] = @personid AND SP.InstitutionID=@instID));
			
			DELETE FROM [core].[EducationalState]
				WHERE [EducationalState].PersonID = @personid AND [EducationalState].InstitutionID=@instID;
			PRINT 'INFO: [EducationalState] DELETED for personid=' + CAST(@personid AS VARCHAR(12));

			DELETE [inst_year].[CurriculumTeacher] OUTPUT deleted.* INTO [inst_year].[CurriculumTeacherTemp]
				WHERE [CurriculumTeacher].CurriculumTeacherID IN (
					SELECT (CurriculumTeacherID) FROM [inst_year].[CurriculumTeacher] CT
						JOIN [inst_basic].[StaffPosition] SP ON CT.StaffPositionID = SP.StaffPositionID WHERE SP.[PersonID] = @personid AND SP.InstitutionID=@instID);
			PRINT 'INFO: [inst_year].[CurriculumTeacher] DELETED for personid=' + CAST(@personid AS VARCHAR(12));

			-- ------------------------------------- DELETE record [student].[SelfGovernment]  --------------------------
			DELETE FROM [student].[SelfGovernment]
				WHERE [student].[SelfGovernment].PersonId=@personid AND [student].[SelfGovernment].InstitutionId = @instID;
			PRINT 'INFO: [student].[SelfGovernment] DELETED for personid=' + CAST(@personid AS VARCHAR(12)) + ' through [StaffPosition] where HelpStudentGovernment = 1';

			
			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END
		
	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personid AS personid, @curriculumDeletedString AS curriculumDeletedString;
END /* End OperationType = 3 -> DELETE *************** */

ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', instID=' + ISNULL(CAST(@instID AS VARCHAR(12)),'') +
		', personid=' + ISNULL(CAST(@personid AS VARCHAR(12)),'');
	THROW 99000, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'Person', @personid, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'Person', @personid, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [personCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */