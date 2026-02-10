USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[personOKSCUD]    Script Date: 8/1/2022 12:54:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[personOKSCUD]
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
-- ---------- define context param: personid --------------
DECLARE @personid int;
SELECT @personid = personID FROM OPENJSON(@_json)
WITH (
		personid int			'$.personid'
)

-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define Master object: [PersonOKSID] ID param --------------
DECLARE @personOKSID int;
SELECT @personOKSID = personOKSID FROM OPENJSON(@_json)
WITH (
		personOKSID int			'$.personOKSID'
)

-- ---------- Define Details object 1: PersonOKSSubjectGroup ID and string params ---
DECLARE @PersonOKSSubjectGroupVar TABLE  
(  
	id INT
);
DECLARE @PersonOKSSubjectGroupString NVARCHAR(4000);

-- ---------- define Detail object 1: [PersonOKSSubjectGroupUpdate] ID and string params ---
DECLARE @personOKSSubjectGroupCreatedVar TABLE  
(  
	id INT
);
DECLARE @personOKSSubjectGroupCreatedString NVARCHAR(4000);

DECLARE @personOKSSubjectGroupUpdatedVar TABLE  
(  
	id INT
);
DECLARE @personOKSSubjectGroupUpdatedString NVARCHAR(4000);

DECLARE @personOKSSubjectGroupDeletedString NVARCHAR(4000);

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
DECLARE @hasRefDataStaffPositionMainPersonOKS int;
SET @hasRefDataStaffPositionMainPersonOKS = (SELECT COUNT(StaffPositionID) FROM [inst_basic].[StaffPosition]
	WHERE [inst_basic].[StaffPosition].MainPersonOKSID = @personOKSID);

-- ---------- define context param: instid --------------
DECLARE @instid int;
SELECT @instid = instid FROM OPENJSON(@_json)
WITH (   
		instid int			'$.instid'
)
-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID);


	--if the school used external data provider --- then it cannot write to the database
	DECLARE @isExtDataProvider bit;
	SELECT @isExtDataProvider = IIF(COUNT(SOExtProviderID)>0,1,0)
	FROM [core].[InstitutionConfData] 
	WHERE InstitutionID = @instID 
		AND SchoolYear = @schoolYear

	IF @isExtDataProvider = 1
	BEGIN	
		SET @operationResultType = 2;
		SET @messageCode = 1023;
		SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @instID AS instID; 

		COMMIT TRANSACTION
		RETURN
	END



PRINT 'INFO: PROCEDURE [personOKSCUD] started! Initial data gathered. PersonID=' + CAST(@personid AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW PersonOKS *************** */
IF ((@OperationType = 1) AND (@personid IS NOT NULL) AND (@personOKSID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [PersonOKS]...';
	INSERT INTO [inst_basic].[PersonOKS]
           (
			 [PersonID]
			 ,[EducationGradeTypeID]
			 ,[SpecialityOrdTypeID]
			 ,[UniversityID]
			 ,[UniversityNotes]
			 ,[CertifcateNo]
			 ,[YearOfGraduation]
			 ,[Speciality]
			 ,[AcquiredPK]
			 ,[IsPKTeacher]
			 ,[SysUserID]
			)
	SELECT @personid, *, @sysuserid
	FROM OPENJSON(@_json)
	WITH (
			educationGradeTypeID int
			,specialityOrdTypeID int
			,universityID int
			,universityNotes nvarchar(1024)
			,certifcateNo nvarchar(50)
			,yearOfGraduation int
			,speciality nvarchar(1024)
			,acquiredPK nvarchar(1024)
			,isPKTeacher bit

		);

	-- GET INSERTED @PersonOKS (autoincrement)
	SELECT @personOKSID = SCOPE_IDENTITY();
	PRINT 'INFO: [PersonOKS] created with id=' + CAST(@personOKSID AS VARCHAR(12));

	INSERT INTO [inst_basic].[PersonOKSSubjectGroup]
				([PersonOKSID]
				,[SubjectGroupID]
				,[SysUserID])
	OUTPUT INSERTED.PersonOKSSubjectGroupID INTO @PersonOKSSubjectGroupVar
	SELECT @personOKSID, *, @sysuserid
	FROM OPENJSON(@_json, '$.personOKSSubjectGroupData')
	WITH (   
		 SubjectGroupID int			'$.personOKSSubjectGroup'
		); 
	
	SET @PersonOKSSubjectGroupString = (SELECT STRING_AGG(id, ',') FROM @PersonOKSSubjectGroupVar);
	PRINT 'INFO: [PersonOKSSubjectGroup] created with ids=' + @PersonOKSSubjectGroupString;
	/* *********************** END INSERT INTO [PersonOKSSubjectGroup] ************************************ */

	SELECT @personOKSID AS personOKSID, @PersonOKSSubjectGroupString AS PersonOKSSubjectGroupIDs ; -- return back to the caller i.e. front-end


END /* End of OperationType = 1 -> CREATE NEW *************** */

/* ############################## Start OperationType = 2 -> UPDATE PersonOKS ############################## */
ELSE IF ((@OperationType = 2) AND (@personOKSID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [PersonOKS] with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[PersonOKS]
	SET
			 [EducationGradeTypeID]=JPCS.educationGradeTypeID
			 ,[SpecialityOrdTypeID]=JPCS.specialityOrdTypeID
			 ,[UniversityID]=JPCS.universityID
			 ,[UniversityNotes]=JPCS.universityNotes
			 ,[CertifcateNo]=JPCS.certifcateNo
			 ,[YearOfGraduation]=JPCS.yearOfGraduation
			 ,[Speciality]=JPCS.speciality
			 ,[AcquiredPK]=JPCS.acquiredPK
			 ,[IsPKTeacher]=JPCS.isPKTeacher
			 ,[SysUserID] = @sysuserid

	FROM OPENJSON(@_json)
	WITH (
			educationGradeTypeID int
			,specialityOrdTypeID int
			,universityID int
			,universityNotes nvarchar(1024)
			,certifcateNo nvarchar(50)
			,yearOfGraduation int
			,speciality nvarchar(1024)
			,acquiredPK nvarchar(1024)
			,isPKTeacher bit

		) JPCS
	WHERE
		[inst_basic].[PersonOKS].[PersonOKSID] = @personOKSID;
	PRINT 'INFO: [PersonOKS] UPDATED successfully!';

		/* *********************** START INSERT INTO [PersonOKSSubjectGroup] ************************************ */ 
	INSERT INTO [inst_basic].[PersonOKSSubjectGroup]
				(
				[PersonOKSID]
				,[SubjectGroupID]
				,[SysUserID]
				)
	OUTPUT INSERTED.PersonOKSSubjectGroupID INTO @personOKSSubjectGroupCreatedVar
	SELECT @personOKSID, *, @sysuserid
	FROM OPENJSON(@_json, '$.personOKSSubjectGroupDataCreated')
	WITH (   
			SubjectGroupID int			'$.personOKSSubjectGroup'
		); 
	
	SET @personOKSSubjectGroupCreatedString = (SELECT STRING_AGG(id, ',') FROM @personOKSSubjectGroupCreatedVar);
	PRINT 'INFO: [PersonOKSSubjectGroup] created with ids=' + @personOKSSubjectGroupCreatedString;

	/* *********************** UPDATE existing/changed [PersonOKSSubjectGroup] objects in collection ******************************** */ 
	UPDATE [inst_basic].[PersonOKSSubjectGroup]
	SET
		[SubjectGroupID]=SG.SubjectGroupID
		,[SysUserID]=@sysuserid
	
	OUTPUT INSERTED.PersonOKSSubjectGroupID INTO @personOKSSubjectGroupUpdatedVar
	FROM OPENJSON(@_json, '$.personOKSSubjectGroupDataUpdated')
	WITH (
		id	int							'$.id'
		,SubjectGroupID int			'$.personOKSSubjectGroup'
		) SG
	WHERE
		[inst_basic].[PersonOKSSubjectGroup].PersonOKSSubjectGroupID = SG.id;
	SET @personOKSSubjectGroupUpdatedString = (SELECT STRING_AGG(id, ',') FROM @personOKSSubjectGroupUpdatedVar);
	PRINT 'INFO: [inst_basic].[PersonOKSSubjectGroup] UPDATED with Ids=' + @personOKSSubjectGroupUpdatedString;

	/* *********************** DELETE removed [PersonOKSSubjectGroup] objects in collection **************************** */
	SELECT @personOKSSubjectGroupDeletedString = (SELECT STRING_AGG(personOKSSubjectGroupDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.personOKSSubjectGroupDataDeleted')
		WITH (   
			personOKSSubjectGroupDeletedIDs int			'$.id'
		));

	DELETE FROM [inst_basic].[PersonOKSSubjectGroup]
		WHERE [inst_basic].[PersonOKSSubjectGroup].PersonOKSSubjectGroupID IN (			
			SELECT personOKSSubjectGroupDeletedIDs FROM OPENJSON(@_json, '$.personOKSSubjectGroupDeleted')
				WITH (   
					personOKSSubjectGroupDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [inst_basic].[PersonOKSSubjectGroup] DELETED with Id=' + CAST(@personOKSSubjectGroupDeletedString AS VARCHAR(12)); 

	SELECT @personOKSID AS personOKSID
	,@personOKSSubjectGroupCreatedString AS personOKSSubjectGroupCreatedIDs
	,@personOKSSubjectGroupUpdatedString AS personOKSSubjectGroupUpdatedIDs
	,@personOKSSubjectGroupDeletedString AS personOKSSubjectGroupDeletedIDs; -- return back to the caller i.e. front-end


	/* *********************** END INSERT INTO [PersonOKSSubjectGroup] ************************************ */
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 3 -> DELETE PersonOKS ############################## */
ELSE IF ((@OperationType = 3) AND (@personOKSID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [PersonOKS] with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12));

	/* FIRST, check for referenced data */
	PRINT 'INFO: CHECK for referenced data found in [inst_basic].[StaffPosition].MainPersonOKSID: ' + CAST(@hasRefDataCount AS VARCHAR(12)) + ' records';

	SET @hasRefDataCount = @hasRefDataStaffPositionMainPersonOKS;

	IF (@hasRefDataCount = 0) -- there is NO referenced data, will delete master and detailed (children) records
	BEGIN
		PRINT 'INFO: There are no references to master record in [inst_basic].[StaffPosition].MainPersonOKSID with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12)) + ' Master and detailed (children) records will be deleted...';
		-- Delete master and detailed records
		DELETE FROM [inst_basic].[PersonOKSSubjectGroup]
			WHERE [inst_basic].[PersonOKSSubjectGroup].PersonOKSID = @personOKSID;
		PRINT 'INFO: [PersonOKSSubjectGroup] DELETED with PersonOKSSubjectGroupID=' + CAST(@personOKSID AS VARCHAR(12));

		DELETE FROM [inst_basic].[PersonOKS]
			WHERE [inst_basic].[PersonOKS].[PersonOKSID] = @personOKSID;
		PRINT 'INFO: [PersonOKS] DELETED with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12));

		SET @operationResultType = 1;
		SET @messageCode = 1;
	END
	ELSE IF (@hasRefDataCount > 0) -- there is referenced data, will ask for confirmation with specific question for referral data
	BEGIN
		PRINT 'INFO: There are references to master record in [inst_basic].[StaffPosition].MainPersonOKSID with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12));
		IF (@forceOperation = 0) -- need to ask for operation confirmation IF there is referenced data
		BEGIN
			PRINT 'INFO: @forceOperation = 0 => Will ask User for operation confirmation with specific message. Do nothing now!';
			SET @operationResultType = 0;
			SET @messageCode = 1004;

			SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personOKSID AS personOKSID; -- return back to the caller i.e. front-end
		
			COMMIT TRANSACTION

			RETURN -- exit procedure
		END
		ELSE IF (@forceOperation = 1) -- confirmation is done, do specific operations based on ref. data
		BEGIN
			PRINT 'INFO: @forceOperation = 1 => Will set [inst_basic].[StaffPosition].MainPersonOKSID to NULL for personOKSID=' + CAST(@personOKSID AS VARCHAR(12));
			UPDATE [inst_basic].[StaffPosition]
				SET [inst_basic].[StaffPosition].MainPersonOKSID = NULL
				WHERE [inst_basic].[StaffPosition].MainPersonOKSID = @personOKSID;
			PRINT 'INFO: [StaffPosition].MainPersonOKSID for personOKSID=' + CAST(@personOKSID AS VARCHAR(12)) + ' set to NULL successfully';	
					
			-- Delete master and detailed records
			DELETE FROM [inst_basic].[PersonOKSSubjectGroup]
				WHERE [inst_basic].[PersonOKSSubjectGroup].PersonOKSID = @personOKSID;
			PRINT 'INFO: [PersonOKSSubjectGroup] DELETED with PersonOKSSubjectGroupID=' + CAST(@personOKSID AS VARCHAR(12));

			DELETE FROM [inst_basic].[PersonOKS]
				WHERE [inst_basic].[PersonOKS].[PersonOKSID] = @personOKSID;
			PRINT 'INFO: [PersonOKS] DELETED with PersonOKSID=' + CAST(@personOKSID AS VARCHAR(12));

			SET @operationResultType = 1;
			SET @messageCode = 1;
		END
	END

	SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @personOKSID AS personOKSID;

END /**************** End OperationType = 3 -> DELETE ****************/

ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', personid=' + ISNULL(CAST(@personid AS VARCHAR(12)),'') +
		', personOKSID=' + ISNULL(CAST(@personOKSID AS VARCHAR(12)),'');
	THROW 99003, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'PersonOKS', @personOKSID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'PersonOKS', @personOKSID, NULL;

	DECLARE @ErrorMessage NVARCHAR(4000);
	DECLARE @ErrorSeverity INT;
	DECLARE @ErrorState INT;

	SELECT
		@ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE();

	PRINT 'INFO: [personOKSCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
