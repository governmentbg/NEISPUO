USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[buildingCUD]    Script Date: 5/19/2022 11:11:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[buildingCUD] 
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

-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SET @schoolYear = [inst_year].[getSchoolYearByInstID](@instID)

-- ---------- define context param: аddressID --------------
DECLARE @аddressID int;
SELECT @аddressID = departmentAddrID FROM OPENJSON(@_json)
WITH (   
		departmentAddrID int		'$.address'
)

-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)


-- ---------- define Master object: [Building] ID param --------------
DECLARE @buildingID int;
SELECT @buildingID = buildingID FROM OPENJSON(@_json)
WITH (   
		buildingID int				'$.buildingID'
);

-- ---------- define Detail object 1: [BuildingModernizationDegree] ID and string params ---
DECLARE @buildingModernizationDegreeCreatedVar TABLE  
(  
	id INT
);
DECLARE @buildingModernizationDegreeCreatedString NVARCHAR(4000);

DECLARE @buildingModernizationDegreeUpdatedVar TABLE  
(  
	id INT
);
DECLARE @buildingModernizationDegreeUpdatedString NVARCHAR(4000);

DECLARE @buildingModernizationDegreeDeletedString NVARCHAR(4000);

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
DECLARE @hasRefDataCountBuildingRoom int;
SET @hasRefDataCountBuildingRoom = (SELECT COUNT(1) FROM [inst_basic].[BuildingRoom] B WHERE B.BuildingID = @buildingID);

PRINT 'INFO: PROCEDURE buildingCUD started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12)) + ' addressID=' + CAST(@аddressID AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW [Building] *************** */
IF ((@OperationType = 1) AND (@аddressID IS NOT NULL) AND (@buildingID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [Building]...';
	INSERT INTO [inst_basic].[Building]
	(
	   [InstitutionDepartmentID]
	  ,[InstitutionID]
	  ,[Name]
	  ,[BuildingTypeID]
	  ,[ExpectedStudentCount]
	  ,[StructureCount]
	  ,[SchoolShiftTypeID]
	  ,[CadastralCode]
	  ,[InitialOwnershipDoc]
	  ,[LatestOwnershipDoc]
	  ,[LatestRepairYear]
	  ,[Latitude]
	  ,[Longitude]
	  ,[Note]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	SELECT @аddressID, *, @sysuserid, @schoolYear
	FROM OPENJSON(@_json)
	WITH (
		instid int						'$.instid' 	
	   ,name nvarchar(255)				'$.name' 
	   ,buildingTypeID int				'$.buildingTypeID' 
	   ,expectedStudentCount int		'$.expectedStudentCount' 
	   ,structureCount int				'$.structureCount' 
	   ,schoolShiftTypeID int			'$.schoolShiftTypeID' 
	   ,cadastralCode nvarchar(50)		'$.cadastralCode' 
	   ,initialOwnershipDoc int			'$.initialOwnershipDoc' 
	   ,latestOwnershipDoc int			'$.latestOwnershipDoc' 
	   ,latestRepairYear int			'$.latestRepairYear'
	   ,latitude float					'$.latitude' 
	   ,longitude float					'$.longitude' 
	   ,note nvarchar(4000)				'$.note' 
	);


	-- GET INSERTED BuildingID (autoincrement)
	SELECT @buildingID = SCOPE_IDENTITY();
	PRINT 'INFO: [Building] created with id=' + CAST(@buildingID AS VARCHAR(12));

	/* *********************** START INSERT INTO [BuildingModernizationDegree] ************************************ */ 
	INSERT INTO [inst_basic].[BuildingModernizationDegree]
	(
	   [BuildingID]
	  ,[ModernizationDegreeID]
	  ,[Count]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingModernizationDegreeID INTO @buildingModernizationDegreeCreatedVar
	SELECT @buildingID, *, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingModernizationDegree')
	WITH (
		modernizationDegreeID int		'$.modernizationDegreeID'
		,countModernizationDegree int	'$.countModernizationDegree'
	);
	SET @buildingModernizationDegreeCreatedString = (SELECT STRING_AGG(id, ',') FROM @buildingModernizationDegreeCreatedVar);
	PRINT 'INFO: [BuildingModernizationDegree] INSERTED with ids=' + @buildingModernizationDegreeCreatedString;
	/* *********************** END INSERT INTO [BuildingModernizationDegree] ************************************ */

	SELECT @аddressID AS аddress, @buildingID AS buildingID, @buildingModernizationDegreeCreatedString AS buildingModernizationDegreeCreatedIDs; -- return back to the caller i.e. front-end

END  /* End of OperationType = 1 -> CREATE NEW BuildingModernizationDegree *************** */

/* ############################## Start OperationType = 2 -> UPDATE [Building] ############################## */
ELSE IF ((@OperationType = 2) AND (@аddressID IS NOT NULL) AND (@buildingID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [Building] with BuildingID=' + CAST(@buildingID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[Building]
	SET
	   [Name] = JB.name
	  ,[BuildingTypeID] = JB.buildingTypeID
	  ,[ExpectedStudentCount] = JB.expectedStudentCount
	  ,[StructureCount] = JB.structureCount
	  ,[SchoolShiftTypeID] = JB.schoolShiftTypeID
	  ,[CadastralCode] = JB.cadastralCode
	  ,[InitialOwnershipDoc] = JB.initialOwnershipDoc
	  ,[LatestOwnershipDoc] = JB.latestOwnershipDoc
	  ,[LatestRepairYear] = JB.latestRepairYear
	  ,[Latitude] = JB.latitude
	  ,[Longitude] = JB.longitude
	  ,[Note] = JB.note
	  ,[SysUserID]=@sysuserid
	FROM OPENJSON(@_json)
	WITH (
	    name nvarchar(255)
	   ,buildingTypeID int
	   ,expectedStudentCount int
	   ,structureCount int
	   ,schoolShiftTypeID int
	   ,cadastralCode nvarchar(50)
	   ,initialOwnershipDoc int
	   ,latestOwnershipDoc int
	   ,latestRepairYear int
	   ,latitude float
	   ,longitude float
	   ,note nvarchar(4000)
	) JB 
	WHERE 
		[inst_basic].[Building].BuildingID = @buildingID;
	PRINT 'INFO: [Building] UPDATED successfully!';

/* ############################## Start OperationType = 2 -> UPDATE BuildingModernizationDegree ############################## */
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Detail object: [BuildingModernizationDegree] with BuildingID=' + CAST(@buildingID AS VARCHAR(12));

	/* *********************** START INSERT INTO [BuildingModernizationDegree] ************************************ */ 
	INSERT INTO [inst_basic].[BuildingModernizationDegree]
	(
	   [BuildingID]
	  ,[ModernizationDegreeID]
	  ,[Count]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingModernizationDegreeID INTO @BuildingModernizationDegreeCreatedVar
	SELECT @buildingID, *, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingModernizationDegreeCreated')
	WITH (
		modernizationDegreeID int		'$.modernizationDegreeID'
		,countModernizationDegree int	'$.countModernizationDegree'
	);
	SET @BuildingModernizationDegreeCreatedString = (SELECT STRING_AGG(id, ',') FROM @BuildingModernizationDegreeCreatedVar);
	PRINT 'INFO: [BuildingModernizationDegree] INSERTED with ids=' + @BuildingModernizationDegreeCreatedString;
	/* *********************** END INSERT INTO [BuildingModernizationDegree] ************************************ */

	/* *********************** UPDATE existing/changed [BuildingModernizationDegree] objects in collection ******************************** */ 
	UPDATE [inst_basic].[BuildingModernizationDegree]
	SET
		[ModernizationDegreeID]=MD.modernizationDegreeID
		,[Count]=MD.countModernizationDegree
		,[SysUserID]=@sysuserid
	
	OUTPUT INSERTED.BuildingModernizationDegreeID INTO @buildingModernizationDegreeUpdatedVar
	FROM OPENJSON(@_json, '$.buildingModernizationDegreeUpdated')
	WITH (
		id	int							'$.id'
		,modernizationDegreeID int		'$.modernizationDegreeID'
		,countModernizationDegree int	'$.countModernizationDegree'
		) MD
	WHERE
		[inst_basic].[BuildingModernizationDegree].BuildingModernizationDegreeID = MD.id;
	SET @buildingModernizationDegreeUpdatedString = (SELECT STRING_AGG(id, ',') FROM @buildingModernizationDegreeUpdatedVar);
	PRINT 'INFO: [inst_basic].[BuildingModernizationDegree] UPDATED with Ids=' + @buildingModernizationDegreeUpdatedString;
	
	
	/* *********************** DELETE removed [BuildingModernizationDegree] objects in collection **************************** */
	SELECT @buildingModernizationDegreeDeletedString = (SELECT STRING_AGG(buildingModernizationDegreeDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.buildingModernizationDegreeDeleted')
		WITH (   
			buildingModernizationDegreeDeletedIDs int			'$.id'
		));

	DELETE FROM [inst_basic].[BuildingModernizationDegree]
		WHERE [inst_basic].[BuildingModernizationDegree].BuildingModernizationDegreeID IN (			
			SELECT buildingModernizationDegreeDeletedIDs FROM OPENJSON(@_json, '$.buildingModernizationDegreeDeleted')
				WITH (   
					buildingModernizationDegreeDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [inst_basic].[BuildingModernizationDegree] DELETED with Id=' + CAST(@buildingModernizationDegreeDeletedString AS VARCHAR(12)); 

	SELECT @аddressID AS аddress, @buildingID AS buildingID, 
	@buildingModernizationDegreeCreatedString AS buildingModernizationDegreeCreatedIDs
	,@buildingModernizationDegreeUpdatedString AS buildingModernizationDegreeUpdatedIDs
	,@buildingModernizationDegreeDeletedString AS buildingModernizationDegreeDeletedDs; -- return back to the caller i.e. front-end
END  /* End OperationType = 2 -> UPDATE Building *************** */

/* ############################## Start OperationType = 3 -> DELETE [Building] ############################## */
ELSE IF ((@OperationType = 3) AND (@buildingID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [Building] with BuildingID=' + CAST(@buildingID AS VARCHAR(12));

	/* FIRST, check for referenced data */
	PRINT 'INFO: Will calculate referenced data records in: [inst_basic].[BuildingRoom] with BuildingID=' + CAST(@buildingID AS VARCHAR(12));
		
	SET @hasRefDataCount = @hasRefDataCountBuildingRoom;

	PRINT 'INFO: CHECK for referenced data found in [inst_basic].[BuildingRoom] in total: ' + CAST(@hasRefDataCount AS VARCHAR(12)) + ' records';

	IF (@hasRefDataCount = 0) -- there is NO referenced data, will delete master and detailed (children) records
	BEGIN
		PRINT 'INFO: There are no references to master record with BuildingID=' + CAST(@buildingID AS VARCHAR(12)) + ' Master and detailed (children) records will be deleted...';
		-- Delete master and detailed records
		DELETE BMD FROM [inst_basic].[BuildingModernizationDegree] BMD
			WHERE BMD.BuildingID = @buildingID;
		PRINT 'INFO: [BuildingModernizationDegree] DELETED with buildingID=' + CAST(@buildingID AS VARCHAR(12));

		DELETE B FROM [inst_basic].[Building] B
			WHERE B.BuildingID = @buildingID;
		PRINT 'INFO: [Building] DELETED with BuildingID=' + CAST(@buildingID AS VARCHAR(12));

		SET @operationResultType = 1;
		SET @messageCode = 1;
	END
	ELSE IF (@hasRefDataCount > 0) -- there is referenced data, will ask for confirmation with specific question for referral data
	BEGIN
		PRINT 'INFO: There are references to master record with buildingID=' + CAST(@buildingID AS VARCHAR(12));
		IF (@forceOperation = 0) -- need to ask for operation confirmation IF there is referenced data
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with specific message. Do nothing now!';
			SET @operationResultType = 0;
			SET @messageCode = 1016;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @buildingID AS buildingID;
		
			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done, do specific operations based on ref. data
		BEGIN
			PRINT 'INFO: @forceOperation = 1 => Will DELETE [BuildingRoomEquipment], [BuildingRoom], [BuildingModernizationDegree], [Building] with buildingID=' + CAST(@buildingID AS VARCHAR(12));
			
			DELETE BRE FROM [inst_basic].[BuildingRoomEquipment] BRE 
				JOIN [inst_basic].[BuildingRoom] BR ON BRE.BuildingRoomID = BR.BuildingRoomID 
				WHERE BR.BuildingID = @buildingID;
			PRINT 'INFO: [BuildingRoomEquipment] DELETED for buildingID=' + CAST(@buildingID AS VARCHAR(12));

			DELETE BR FROM [inst_basic].[BuildingRoom] BR 
				WHERE BR.BuildingID = @buildingID;
			PRINT 'INFO: [BuildingRoom] DELETED for buildingID=' + CAST(@buildingID AS VARCHAR(12));

			DELETE BMD FROM [inst_basic].[BuildingModernizationDegree] BMD
				WHERE BMD.BuildingID = @buildingID;
			PRINT 'INFO: [BuildingModernizationDegree] DELETED with buildingID=' + CAST(@buildingID AS VARCHAR(12));

			DELETE B FROM [inst_basic].[Building] B
				WHERE B.BuildingID = @buildingID;
			PRINT 'INFO: [Building] DELETED with BuildingID=' + CAST(@buildingID AS VARCHAR(12));

			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END

	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @buildingID AS buildingID;
END /* End OperationType = 3 -> DELETE [Building] *************** */
ELSE
	THROW 99100, '[Building] Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'Building', @buildingID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'Building', @buildingID, NULL;
  
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [buildingCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
