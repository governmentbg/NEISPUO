USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[personFLSkillCUD]    Script Date: 8/1/2022 12:54:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[personFLSkillCUD] 
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

-- ---------- define Master object: [personFLSkill] ID param --------------
DECLARE @personFLSkillID int;
SELECT @personFLSkillID = personFLSkillID FROM OPENJSON(@_json)
WITH (   
			personFLSkillID int			'$.personFLSkillID'
)
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
		DECLARE @operationResultType int,  @messageCode int;
		SET @operationResultType = 2;
		SET @messageCode = 1023;
		SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @instID AS instID; 

		COMMIT TRANSACTION
		RETURN
	END


PRINT 'INFO: PROCEDURE personFLSkillCUD started! Initial data gathered. PersonID=' + CAST(@personid AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW PersonFLSkill *************** */
IF ((@OperationType = 1) AND (@personid IS NOT NULL) AND (@personFLSkillID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [PersonFLSkill]...';
	INSERT INTO [inst_basic].[PersonFLSkill]
           (
				[PersonID]
				,[FLID]
				,[FLLevelID] 
				,[Notes] 
				,[SysUserID]
			)
	SELECT @personid, *, @sysuserid
	FROM OPENJSON(@_json)
	WITH (   
			flID int
			,flLevelID int
			,notes nvarchar(1024)		
		); 

	-- GET INSERTED @PersonFLSkill (autoincrement)
	SELECT @personFLSkillID = SCOPE_IDENTITY();
	PRINT 'INFO: [PersonFLSkill] created with id=' + CAST(@personFLSkillID AS VARCHAR(12));
		
END  /* End of OperationType = 1 -> CREATE NEW *************** */

/* ############################## Start OperationType = 2 -> UPDATE Institution publicCouncil ############################## */
ELSE IF ((@OperationType = 2) AND (@personFLSkillID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [PersonFLSkill] with PersonFLSkillID=' + CAST(@personFLSkillID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[PersonFLSkill]
	SET 
			 [FLID]=JPCS.flID
			,[FLLevelID]=JPCS.fLLevelID 
			,[Notes]=JPCS.notes
			,[SysUserID]=@sysuserid
				
	FROM OPENJSON(@_json)
	WITH (   
			 flID int
			,flLevelID int
			,notes nvarchar(1024)
		) JPCS				 
	WHERE
		[inst_basic].[PersonFLSkill].[PersonFLSkillID] = @personFLSkillID;
	PRINT 'INFO: [PersonFLSkill] UPDATED successfully!';
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 3 -> DELETE PersonFLSkill ############################## */
ELSE IF ((@OperationType = 3) AND (@personFLSkillID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [PersonFLSkill] with PersonFLSkillID=' + CAST(@personFLSkillID AS VARCHAR(12));

		DELETE FROM [inst_basic].[PersonFLSkill]
			WHERE [inst_basic].[PersonFLSkill].[PersonFLSkillID] = @personFLSkillID;
		PRINT 'INFO: [PersonFLSkill] DELETED with PersonFLSkillID=' + CAST(@personFLSkillID AS VARCHAR(12));
		
	SELECT @personFLSkillID AS id;
END /* End OperationType = 3 -> DELETE *************** */

ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', personid=' + ISNULL(CAST(@personid AS VARCHAR(12)),'') +
		', personFLSkillID=' + ISNULL(CAST(@personFLSkillID AS VARCHAR(12)),'');
	THROW 99007, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'PersonFLSkill', @personFLSkillID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'PersonFLSkill', @personFLSkillID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [personFLSkillCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */