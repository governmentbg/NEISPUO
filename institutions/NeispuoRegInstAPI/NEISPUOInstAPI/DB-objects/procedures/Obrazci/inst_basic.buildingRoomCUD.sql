USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[buildingRoomCUD]    Script Date: 5/19/2022 11:11:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[buildingRoomCUD] 
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

-- ---------- define Master object: [buildingRoomID] ID param --------------
DECLARE @buildingRoomID int;
SELECT @buildingRoomID = buildingRoomID FROM OPENJSON(@_json)
WITH (   
		buildingRoomID int			'$.buildingRoomID'
);

-- ---------- define Detail object 1: [buildingRoomEquipmentBasic] ID and string params ---
DECLARE @buildingRoomEquipmentBasicCreatedVar TABLE  
(  
	id INT
);
DECLARE @buildingRoomEquipmentBasicCreatedString NVARCHAR(4000);

DECLARE @buildingRoomEquipmentBasicUpdatedVar TABLE  
(  
	id INT
);
DECLARE @buildingRoomEquipmentBasicUpdatedString NVARCHAR(4000);

DECLARE @buildingRoomEquipmentBasicDeletedString NVARCHAR(4000);

-- ---------- define Detail object 1: [buildingRoomEquipmentSpecial] ID and string params ---
DECLARE @buildingRoomEquipmentSpecialCreatedVar TABLE  
(  
	id INT
);
DECLARE @buildingRoomEquipmentSpecialCreatedString NVARCHAR(4000);

DECLARE @buildingRoomEquipmentSpecialUpdatedVar TABLE  
(  
	id INT
);
DECLARE @buildingRoomEquipmentSpecialUpdatedString NVARCHAR(4000);

DECLARE @buildingRoomEquipmentSpecialDeletedString NVARCHAR(4000);

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
DECLARE @hasRefDataCountBuildingRoomEquipment int;
SET @hasRefDataCountBuildingRoomEquipment = (SELECT COUNT(1) FROM [inst_basic].[BuildingRoomEquipment] B WHERE B.BuildingRoomID = @buildingRoomID);


PRINT 'INFO: PROCEDURE buildingRoomCUD started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12)) + ' addressID=' + CAST(@аddressID AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW [BuildingRoom] *************** */
IF ((@OperationType = 1) AND (@аddressID IS NOT NULL) AND (@buildingRoomID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [BuildingRoom]...';
	INSERT INTO [inst_basic].[BuildingRoom]
	(
	   [InstitutionID]
	  ,[BuildingID]
	  ,[BuildingAreaTypeID]
	  ,[BuildingRoomTypeID]
	  ,[Name]
	  ,[Note]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	SELECT *, @sysuserid, @schoolYear
	FROM OPENJSON(@_json)
	WITH (
	    InstitutionID int                       '$.instid'
	   ,BuildingID int                          '$.buildingID'
	   ,BuildingAreaTypeID smallint             '$.buildingAreaTypeID'
	   ,BuildingRoomTypeID int                  '$.buildingRoomTypeID'
	   ,Name nvarchar(255)                      '$.name'
	   ,Note nvarchar(4000)                     '$.note'
	);
	
	-- GET INSERTED BuildingRoomID (autoincrement)
	SELECT @buildingRoomID = SCOPE_IDENTITY();
	PRINT 'INFO: [BuildingRoomID] created with id=' + CAST(@buildingRoomID AS VARCHAR(12));

	/* *********************** START INSERT INTO [BuildingRoomEquipmentBasic] ********************************** */ 
	INSERT INTO [inst_basic].[BuildingRoomEquipment]
	(
	   [BuildingRoomID]
	  ,[EquipmentTypeID]
	  ,[Note]
	  ,[IsSpecial]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @buildingRoomEquipmentBasicCreatedVar
	SELECT @buildingRoomID, *, 0, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentBasic')
	WITH (
		 EquipmentTypeID int				'$.equipmentTypeID'
		,Note nvarchar(4000)				'$.note'
	);
	SET @buildingRoomEquipmentBasicCreatedString = (SELECT STRING_AGG(id, ',') FROM @buildingRoomEquipmentBasicCreatedVar);
	PRINT 'INFO: [BuildingRoomEquipmentBasic] INSERTED with ids=' + @buildingRoomEquipmentBasicCreatedString;
	/* *********************** END INSERT INTO [BuildingRoomEquipmentBasic] ************************************ */

	/* *********************** START INSERT INTO [BuildingRoomEquipmentSpecial] ********************************** */ 
	INSERT INTO [inst_basic].[BuildingRoomEquipment]
	(
	   [BuildingRoomID]
	  ,[EquipmentTypeID]
	  ,[Note]
	  ,[IsSpecial]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @BuildingRoomEquipmentSpecialCreatedVar
	SELECT @buildingRoomID, *, 1, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentSpecial')
	WITH (
		 EquipmentTypeID int				'$.equipmentTypeID'
		,Note nvarchar(4000)				'$.note'
	);
	SET @BuildingRoomEquipmentSpecialCreatedString = (SELECT STRING_AGG(id, ',') FROM @BuildingRoomEquipmentSpecialCreatedVar);
	PRINT 'INFO: [BuildingRoomEquipmentSpecial] INSERTED with ids=' + @BuildingRoomEquipmentSpecialCreatedString;
	/* *********************** END INSERT INTO [BuildingRoomEquipmentSpecial] ************************************ */

	SELECT @аddressID AS аddress, @buildingRoomID AS buildingRoomID, @BuildingRoomEquipmentBasicCreatedString AS buildingRoomEquipmentBasicIDs, @BuildingRoomEquipmentSpecialCreatedString AS buildingRoomEquipmentSpecialIDs; -- return back to the caller i.e. front-end

END  /* End of OperationType = 1 -> CREATE NEW BuildingRoom *************** */

/* ############################## Start OperationType = 2 -> UPDATE [BuildingRoom] ############################## */
ELSE IF ((@OperationType = 2) AND (@аddressID IS NOT NULL) AND (@buildingRoomID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [BuildingRoom] with buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[BuildingRoom]
	SET
		 [BuildingID] = JBR.buildingID
		,[BuildingAreaTypeID] = JBR.buildingAreaTypeID
		,[BuildingRoomTypeID] = JBR.buildingRoomTypeID
		,[Name] = JBR.name
		,[Note] = JBR.note
		,[SysUserID]=@sysuserid
	FROM OPENJSON(@_json)
	WITH (
		 buildingID int
		,buildingAreaTypeID smallint
		,buildingRoomTypeID int
		,name nvarchar(255)
		,note nvarchar(4000)
	) JBR 
	WHERE 
		[inst_basic].[BuildingRoom].BuildingRoomID = @buildingRoomID;
	PRINT 'INFO: [BuildingRoom] UPDATED successfully!';

/* ############################## Start OperationType = 2 -> UPDATE BuildingRoomEquipmentBasic ############################## */
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Detail object: [BuildingRoomEquipmentBasic] with BuildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));

	/* *********************** START INSERT INTO [BuildingRoomEquipmentBasic] ********************************** */ 
	INSERT INTO [inst_basic].[BuildingRoomEquipment]
	(
	   [BuildingRoomID]
	  ,[EquipmentTypeID]
	  ,[Note]
	  ,[IsSpecial]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @buildingRoomEquipmentBasicCreatedVar
	SELECT @buildingRoomID, *, 0, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentBasicCreated')
	WITH (
		 EquipmentTypeID int				'$.equipmentTypeID'
		,Note nvarchar(4000)				'$.note'
	);
	SET @BuildingRoomEquipmentBasicCreatedString = (SELECT STRING_AGG(id, ',') FROM @buildingRoomEquipmentBasicCreatedVar);
	PRINT 'INFO: [BuildingRoomEquipmentBasic] INSERTED with ids=' + @buildingRoomEquipmentBasicCreatedString;
	/* *********************** END INSERT INTO [BuildingRoomEquipmentBasic] ************************************ */
	
	/* *********************** UPDATE existing/changed [BuildingRoomEquipmentBasic] objects in collection ******************************** */ 
	UPDATE [inst_basic].[BuildingRoomEquipment]
	SET
		 [EquipmentTypeID]=MD.EquipmentTypeID
		,[Note]=MD.Note
	    ,[SysUserID]=@sysuserid
	
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @buildingRoomEquipmentBasicUpdatedVar
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentBasicUpdated')
	WITH (
		id	int							'$.id'
		,EquipmentTypeID int				'$.equipmentTypeID'
		,Note nvarchar(4000)				'$.note'
		) MD
	WHERE
		[inst_basic].[BuildingRoomEquipment].BuildingRoomEquipmentID = MD.id;
	SET @buildingRoomEquipmentBasicUpdatedString = (SELECT STRING_AGG(id, ',') FROM @buildingRoomEquipmentBasicUpdatedVar);
	PRINT 'INFO: [inst_basic].[BuildingRoomEquipment] UPDATED with Ids=' + @buildingRoomEquipmentBasicUpdatedString;
	
	/* *********************** DELETE removed [BuildingRoomEquipmentBasic] objects in collection **************************** */
	SELECT @buildingRoomEquipmentBasicDeletedString = (SELECT STRING_AGG(buildingRoomEquipmentBasicDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.buildingRoomEquipmentBasicDeleted')
		WITH (   
			buildingRoomEquipmentBasicDeletedIDs int			'$.id'
		));

	DELETE FROM [inst_basic].[BuildingRoomEquipment]
		WHERE [inst_basic].[BuildingRoomEquipment].BuildingRoomEquipmentID IN (			
			SELECT buildingRoomEquipmentBasicDeletedIDs FROM OPENJSON(@_json, '$.buildingRoomEquipmentBasicDeleted')
				WITH (   
					buildingRoomEquipmentBasicDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [inst_basic].[BuildingRoomEquipment] DELETED with Id=' + CAST(@buildingRoomEquipmentBasicDeletedString AS VARCHAR(12)); 
	
	   
	/* *********************** START INSERT INTO [BuildingRoomEquipmentSpecial] ******************************** */ 
	INSERT INTO [inst_basic].[BuildingRoomEquipment]
	(
	   [BuildingRoomID]
	  ,[EquipmentTypeID]
	  ,[Note]
	  ,[IsSpecial]
	  ,[SysUserID]
	  ,[SchoolYear]
	)
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @BuildingRoomEquipmentSpecialCreatedVar
	SELECT @buildingRoomID, *, 1, @sysuserid, @schoolYear
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentSpecialCreated')
	WITH (
		 EquipmentTypeID int				'$.equipmentTypeID'
		,Note nvarchar(4000)				'$.note'
	);
	SET @buildingRoomEquipmentSpecialCreatedString = (SELECT STRING_AGG(id, ',') FROM @buildingRoomEquipmentSpecialCreatedVar);
	PRINT 'INFO: [BuildingRoomEquipmentSpecial] INSERTED with ids=' + @buildingRoomEquipmentSpecialCreatedString;
	/* *********************** END INSERT INTO [BuildingRoomEquipmentSpecial] ************************************ */
	
	/* *********************** UPDATE existing/changed [BuildingRoomEquipmentSpecial] objects in collection ******************************** */ 
	UPDATE [inst_basic].[BuildingRoomEquipment]
	SET
		 [EquipmentTypeID]=MD.EquipmentTypeID
		,[Note]=MD.Note
	    ,[SysUserID]=@sysuserid	
	OUTPUT INSERTED.BuildingRoomEquipmentID INTO @buildingRoomEquipmentSpecialUpdatedVar
	FROM OPENJSON(@_json, '$.buildingRoomEquipmentSpecialUpdated')
	WITH (
		id	int							'$.id'
		,EquipmentTypeID int			'$.equipmentTypeID'
		,Note nvarchar(4000)			'$.note'
		) MD
	WHERE
		[inst_basic].[BuildingRoomEquipment].BuildingRoomEquipmentID = MD.id;
	SET @buildingRoomEquipmentSpecialUpdatedString = (SELECT STRING_AGG(id, ',') FROM @buildingRoomEquipmentSpecialUpdatedVar);
	PRINT 'INFO: [inst_basic].[BuildingRoomEquipment] UPDATED with Ids=' + @buildingRoomEquipmentSpecialUpdatedString;
	
	/* *********************** DELETE removed [BuildingRoomEquipmentSpecial] objects in collection **************************** */
	SELECT @buildingRoomEquipmentSpecialDeletedString = (SELECT STRING_AGG(buildingRoomEquipmentSpecialDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.buildingRoomEquipmentSpecialDeleted')
		WITH (   
			buildingRoomEquipmentSpecialDeletedIDs int			'$.id'
		));

	DELETE FROM [inst_basic].[BuildingRoomEquipment]
		WHERE [inst_basic].[BuildingRoomEquipment].BuildingRoomEquipmentID IN (			
			SELECT buildingRoomEquipmentSpecialDeletedIDs FROM OPENJSON(@_json, '$.buildingRoomEquipmentSpecialDeleted')
				WITH (   
					buildingRoomEquipmentSpecialDeletedIDs int		'$.id'
				)
			);
	PRINT 'INFO: [inst_basic].[BuildingRoomEquipment] DELETED with Id=' + CAST(@buildingRoomEquipmentSpecialDeletedString AS VARCHAR(12)); 


	SELECT @аddressID AS аddress, @buildingRoomID AS buildingRoomID
	,@buildingRoomEquipmentBasicCreatedString AS buildingRoomEquipmentBasicCreatedIDs
	,@BuildingRoomEquipmentSpecialCreatedString AS buildingRoomEquipmentSpecialCreatedIDs
	,@buildingRoomEquipmentBasicUpdatedString AS buildingRoomEquipmentBasicUpdatedIDs
	,@BuildingRoomEquipmentSpecialUpdatedString AS buildingRoomEquipmentSpecialUpdatedIDs
	,@buildingRoomEquipmentBasicDeletedString AS buildingRoomEquipmentBasicDeletedIDs
	,@BuildingRoomEquipmentSpecialDeletedString AS buildingRoomEquipmentSpecialDeletedIDs; -- return back to the caller i.e. front-end

END  /* End OperationType = 2 -> UPDATE BuildingRoom *************** */

/* ############################## Start OperationType = 3 -> DELETE [BuildingRoom] ############################## */
ELSE IF ((@OperationType = 3) AND (@buildingRoomID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [BuildingRoom] with BuildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));
	
	/* FIRST, check for referenced data */
	PRINT 'INFO: Will calculate referenced data records to: [inst_basic].[BuildingRoomEquipment] with buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));
		
	SET @hasRefDataCount = @hasRefDataCountBuildingRoomEquipment;

	PRINT 'INFO: CHECK for referenced data found in [inst_basic].[BuildingRoomEquipment] in total: ' + CAST(@hasRefDataCount AS VARCHAR(12)) + ' records';

	IF (@hasRefDataCount = 0) -- there is NO referenced data, will delete master and detailed (children) records
	BEGIN
		PRINT 'INFO: There are no references to master record with buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12)) + ' Master and detailed (children) records will be deleted...';
		-- Delete master and detailed records
		DELETE BR FROM [inst_basic].[BuildingRoom] BR 
			WHERE BR.BuildingRoomID = @buildingRoomID;
		PRINT 'INFO: [BuildingRoom] DELETED for buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));

		SET @operationResultType = 1;
		SET @messageCode = 1;
	END
	ELSE IF (@hasRefDataCount > 0) -- there is referenced data, will ask for confirmation with specific question for referral data
	BEGIN
		PRINT 'INFO: There are references to master record with buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));
		IF (@forceOperation = 0) -- need to ask for operation confirmation IF there is referenced data
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with specific message. Do nothing now!';
			SET @operationResultType = 0;
			SET @messageCode = 1017;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @buildingRoomID AS buildingRoomID;

			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done, do specific operations based on ref. data
		BEGIN
			PRINT 'INFO: @forceOperation = 1 => Will DELETE [BuildingRoomEquipment], [BuildingRoom] with buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));
			
			DELETE BRE FROM [inst_basic].[BuildingRoomEquipment] BRE 
				WHERE BRE.BuildingRoomID = @buildingRoomID;
			PRINT 'INFO: [BuildingRoomEquipment] DELETED for buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));

			DELETE BR FROM [inst_basic].[BuildingRoom] BR 
				WHERE BR.BuildingRoomID = @buildingRoomID;
			PRINT 'INFO: [BuildingRoom] DELETED for buildingRoomID=' + CAST(@buildingRoomID AS VARCHAR(12));

			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END


	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @buildingRoomID AS buildingRoomID;
END /* End OperationType = 3 -> DELETE [Building] *************** */
ELSE
	THROW 99102, '[BuildingRoom] Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'BuildingRoom', @buildingRoomID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'BuildingRoom', @buildingRoomID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [buildingRoomCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
