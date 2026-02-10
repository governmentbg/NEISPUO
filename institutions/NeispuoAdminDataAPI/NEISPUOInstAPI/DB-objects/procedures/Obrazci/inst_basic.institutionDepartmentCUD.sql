USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[institutionDepartmentCUD]    Script Date: 7/31/2022 4:21:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[institutionDepartmentCUD] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
-- ---------- define context param: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int			'$.instid'
)

-- ---------- define Master object: [InstitutionDepartment] ID param --------------
DECLARE @аddressID int;
SELECT @аddressID = departmentAddrID FROM OPENJSON(@_json)
WITH (   
		departmentAddrID int			'$.instdepid'
)

-- ---------- define Details object: [DepartmentPhone] ID and string params ---
DECLARE @departmentPhonesCreatedVar TABLE  
(  
	id INT
);
DECLARE @departmentPhonesCreatedString NVARCHAR(4000);

DECLARE @departmentPhonesUpdatedVar TABLE  
(  
	id INT
);
DECLARE @departmentPhonesUpdatedString NVARCHAR(4000);

DECLARE @departmentPhonesDeletedString NVARCHAR(4000);

-- ---------- define context param: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)
-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID)

-- ---------- define Additional params --------------
DECLARE @operationResultType int; -- (0) Warning. It is sent only if forceOperation = 0; (-1) Error; (1) Successfully completed operation
DECLARE @forceOperation int; -- (0) no force action required, send warning to the customer; (1) operation is forced to be executed
DECLARE @hasRefDataCount int; -- sum of found referenced record (could be more tan one source)
DECLARE @messageCode int;

SELECT @forceOperation = ForceOperation FROM OPENJSON(@_json)
WITH (   
		forceOperation int			'$.forceOperation'
)

-- ---------- check for Ref Data --------------
DECLARE @hasRefDataBuilding int;
SET @hasRefDataBuilding = (SELECT COUNT(BuildingID) FROM [inst_basic].[Building]
	WHERE [inst_basic].[Building].[InstitutionDepartmentID] = @аddressID);

DECLARE @hasRefDataClassGroup int;
SET @hasRefDataClassGroup = (SELECT COUNT(ClassID) FROM [inst_year].[ClassGroup]
	WHERE [inst_year].[ClassGroup].[InstitutionDepartmentID] = @аddressID);

DECLARE @hasRefDataCurriculum int;
SET @hasRefDataCurriculum = (SELECT COUNT(CurriculumID) FROM [inst_year].[Curriculum]
	WHERE [inst_year].[Curriculum].[InstitutionDepartmentID] = @аddressID);

PRINT 'INFO: PROCEDURE institutionDepartmentCreateUpdate started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW InstitutionDepartment *************** */
IF ((@OperationType = 1) AND (@instID IS NOT NULL) AND (@аddressID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [InstitutionDepartment]...';
	INSERT INTO [inst_basic].[InstitutionDepartment]
           ([InstitutionID]
			,[Name]
			,[IsValid]
			,[CountryID]
			,[TownID]
			,[LocalAreaID]
			,[Address]
			,[PostCode]
			,[IsMain]
			,[SysUserID])
	SELECT @instID, *, 0, @sysuserid
	FROM OPENJSON(@_json)
	WITH (   
		Name nvarchar(255)		'$.departmentName'
		,IsValid bit			'$.isValid'
		,CountryID int			'$.country'
		,TownID int				'$.town'
		,LocalAreaID int		'$.localArea'
		,Address nvarchar(255)	'$.departmentAddress'
		,PostCode int			'$.departmentPostalCode'
		); 

	-- GET INSERTED @InstitutionDepartmentID (autoincrement)
	SELECT @аddressID = SCOPE_IDENTITY();
	PRINT 'INFO: [InstitutionDepartment] created with id=' + CAST(@аddressID AS VARCHAR(12));

	/* *********************** START INSERT INTO [InstitutionDepartmentPhone] ************************************ */ 
	INSERT INTO [inst_basic].[InstitutionPhone]
				([InstitutionID]
				,[DepartmentID]
				,[PhoneTypeID]
				,[PhoneCode]
				,[PhoneNumber]
				,[ContactKind]
				,[IsInstitution]
				,[IsMain]
				,[SysUserID])
	OUTPUT INSERTED.InstitutionPhoneID INTO @departmentPhonesCreatedVar
	SELECT @instID, @аddressID, *, 0, @sysuserid
	FROM OPENJSON(@_json, '$.departmentPhones')
	WITH (  
		 phoneTypeID int
		,phoneCode nvarchar(10)
		,phoneNumber nvarchar(50)
		,contactKind nvarchar(255)
		,isMain bit
		); 
	SET @departmentPhonesCreatedString = (SELECT STRING_AGG(id, ',') FROM @departmentPhonesCreatedVar);
	PRINT 'INFO: [InstitutionPhone] INSERTED with ids=' + @departmentPhonesCreatedString;
	/* *********************** END INSERT INTO [InstitutionPhone] ************************************ */

	SELECT @аddressID AS id, @instID AS instID, @departmentPhonesCreatedString AS departmentPhoneIDs; -- return back to the caller i.e. front-end

END  /* End of OperationType = 1 -> CREATE NEW Institution department *************** */

/* ############################## Start OperationType = 2 -> UPDATE Institution department ############################## */
ELSE IF ((@OperationType = 2) AND (@instID IS NOT NULL) AND (@аddressID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [InstitutionDepartment] with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[InstitutionDepartment]
	SET 
		[Name] = JID.departmentName
		,[IsValid] = JID.isValid
		,[CountryID] = JID.country
		,[TownID] = JID.town
		,[LocalAreaID] = JID.localArea
		,[Address] = JID.departmentAddress
		,[PostCode] = JID.departmentPostalCode
		,[SysUserID] = @sysuserid

	FROM OPENJSON(@_json)
	WITH (   
		departmentName nvarchar(255)	
	    ,isValid bit
		,country int
		,town int
		,localArea int
		,departmentAddress nvarchar(255)
		,departmentPostalCode int
		) JID				 
	WHERE
		[inst_basic].[InstitutionDepartment].InstitutionDepartmentID = @аddressID;
	PRINT 'INFO: [InstitutionDepartment] UPDATED successfully!';

	/* ----------------------- UPDATE Institution department phones ----------------------- */
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Detail objects in collection: [InstitutionPhone] with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));

	/* *********************** CREATE newly added [InstitutionPhone] objects in collection ******************************** */ 
	INSERT INTO [inst_basic].[InstitutionPhone]
		([InstitutionID]
		,[DepartmentID]
		,[IsInstitution]
		,[SysUserID]
		,[PhoneTypeID]
		,[PhoneCode]
		,[PhoneNumber]
		,[ContactKind]
		,[IsMain]
		)
	OUTPUT INSERTED.InstitutionPhoneID INTO @departmentPhonesCreatedVar
	SELECT @instID AS InstitutionID, @аddressID AS DepartmentID, 0, @sysuserid, *
	FROM OPENJSON(@_json, '$.departmentPhonesCreated')
	WITH (  
			phoneTypeID int,
			phoneCode nvarchar(10),
			phoneNumber nvarchar(50),
			contactKind nvarchar(255),
			isMain bit
		);
	SET @departmentPhonesCreatedString = (SELECT STRING_AGG(id, ',') FROM @departmentPhonesCreatedVar);
	PRINT 'INFO: [InstitutionPhone] INSERTED with departmentPhonesIDs=' + @departmentPhonesCreatedString;

	/* *********************** UPDATE existing/changed [InstitutionPhone] objects in collection ******************************** */ 
	UPDATE [inst_basic].[InstitutionPhone]
	SET
			 [DepartmentID] = @аddressID
			,[IsInstitution] = 0
			,[SysUserID] = @sysuserid
			,[PhoneTypeID] = JIP.phoneTypeID
			,[PhoneCode] = JIP.phoneCode
			,[PhoneNumber] = JIP.phoneNumber
			,[ContactKind] = JIP.contactKind
			,[IsMain] = JIP.isMain
	OUTPUT INSERTED.InstitutionPhoneID INTO @departmentPhonesUpdatedVar
	FROM OPENJSON(@_json, '$.departmentPhonesUpdated')
	WITH (   
			institutionPhoneID int		'$.id'
			,phoneTypeID int			'$.phoneTypeID'
			,phoneCode nvarchar(10)		'$.phoneCode'
			,phoneNumber nvarchar(50)	'$.phoneNumber'
			,contactKind nvarchar(255)	'$.contactKind'
			,isMain bit					'$.isMain'
		) JIP
	WHERE
		[inst_basic].[InstitutionPhone].InstitutionPhoneID = JIP.institutionPhoneID;
	SET @departmentPhonesUpdatedString = (SELECT STRING_AGG(id, ',') FROM @departmentPhonesUpdatedVar);
	PRINT 'INFO: [InstitutionPhone] UPDATED with InstitutionPhoneIDs=' + @departmentPhonesUpdatedString;

	/* *********************** DELETE removed [InstitutionPhone] objects in collection **************************** */
	SELECT @departmentPhonesDeletedString = (SELECT STRING_AGG(departmentPhonesDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.departmentPhonesDeleted')
		WITH (   
			departmentPhonesDeletedIDs int			'$.id'
		));

	DELETE FROM [inst_basic].[InstitutionPhone]
		WHERE [inst_basic].[InstitutionPhone].InstitutionPhoneID IN (			
			SELECT departmentPhonesDeletedIDs FROM OPENJSON(@_json, '$.departmentPhonesDeleted')
				WITH (   
					departmentPhonesDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [InstitutionPhone] DELETED with InstitutionPhoneIDs=' + CAST(@departmentPhonesDeletedString AS VARCHAR(12)); 

	SELECT @instID AS instID, @аddressID AS аddressID, 
		@departmentPhonesCreatedString AS departmentPhonesCreatedIDs, @departmentPhonesUpdatedString AS departmentPhonesUpdatedIDs, @departmentPhonesDeletedString AS departmentPhonesDeletedIDs; -- return back to the caller i.e. front-end
END  /* *************** End OperationType = 2 -> UPDATE InstitutionDepartment *************** */

/* ############################## Start OperationType = 3 -> DELETE InstitutionDepartment ############################## */
ELSE IF ((@OperationType = 3) AND (@instID IS NOT NULL) AND (@аddressID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [InstitutionDepartment] with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));
	
	DECLARE @secureAddress int;
	SELECT @secureAddress = COUNT(1) 
	FROM inst_nom.InstAdminData 
	WHERE InstitutionID = @instID
		AND SchoolYear = @schoolYear
		AND InstitutionDepartmentID = @аddressID

	IF @secureAddress <> 0 
		BEGIN
			SET @operationResultType = 2;
			SET @messageCode = 1031;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @аddressID AS instdepid; -- return back to the caller i.e. front-end		
			COMMIT TRANSACTION
			RETURN -- exit procedure

		END

	/* FIRST, check for referenced data */
	PRINT 'INFO: Will calculate referenced data records in: [inst_basic].[Building], [inst_year].[ClassGroup], [inst_year].[Curriculum] with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));
		
	SET @hasRefDataCount = @hasRefDataBuilding + @hasRefDataClassGroup + @hasRefDataCurriculum;

	PRINT 'INFO: CHECK for referenced data found in [inst_basic].[Building], [inst_year].[ClassGroup], [inst_year].[Curriculum] in total: ' + CAST(@hasRefDataCount AS VARCHAR(12)) + ' records';

	IF (@hasRefDataCount = 0) -- there is NO referenced data, will delete master and detailed (children) records
	BEGIN
		PRINT 'INFO: There are no references to master record with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12)) + ' Master and detailed (children) records will be deleted...';
		-- Delete master and detailed records
		DELETE FROM [inst_basic].[InstitutionPhone]
			WHERE [inst_basic].[InstitutionPhone].DepartmentID = @аddressID;
		PRINT 'INFO: [InstitutionPhone] DELETED with DepartmentID=' + CAST(@аddressID AS VARCHAR(12));

		DELETE FROM [inst_basic].[InstitutionDepartment]
			WHERE [inst_basic].[InstitutionDepartment].InstitutionDepartmentID = @аddressID;
		PRINT 'INFO: [InstitutionDepartment] DELETED with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));

		SET @operationResultType = 1;
		SET @messageCode = 1;
	END
	ELSE IF (@hasRefDataCount > 0) -- there is referenced data, will ask for confirmation with specific question for referral data
	BEGIN
		PRINT 'INFO: There are references to master record with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));
		IF (@forceOperation = 0) -- need to ask for operation confirmation IF there is referenced data
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with specific message. Do nothing now!';
			SET @operationResultType = 0;
			SET @messageCode = 1001;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @аddressID AS instdepid; -- return back to the caller i.e. front-end
		
			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done, do specific operations based on ref. data
		BEGIN
			PRINT 'INFO: @forceOperation = 1 => Will set [InstitutionDepartment] to isValid=0 for InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12));
			UPDATE TOP(1) [inst_basic].[InstitutionDepartment] SET [inst_basic].[InstitutionDepartment].IsValid=0
				WHERE [inst_basic].[InstitutionDepartment].InstitutionDepartmentID = @аddressID;
			PRINT 'INFO: [InstitutionDepartment] with InstitutionDepartmentID=' + CAST(@аddressID AS VARCHAR(12)) + ' set to isValid=0 successfully';	
					
			DELETE FROM [inst_basic].[InstitutionPhone]
				WHERE [inst_basic].[InstitutionPhone].DepartmentID = @аddressID;
			PRINT 'INFO: InstitutionDepartment phones DELETED successfully from [inst_basic].[InstitutionPhone] with DepartmentID=' + CAST(@аddressID AS VARCHAR(12));
			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END

	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @аddressID AS instdepid;
END /* *************** End of OperationType = 3 -> DELETE [InstitutionDepartment] *************** */ 

ELSE
	THROW 99002, '[institutionDepartmentCUD] Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'InstitutionDepartment', @аddressID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'InstitutionDepartment', @аddressID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [institutionDepartmentCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
