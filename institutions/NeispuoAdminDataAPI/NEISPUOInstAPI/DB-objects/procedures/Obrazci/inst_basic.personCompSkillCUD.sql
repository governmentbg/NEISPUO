USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[personCompSkillCUD]    Script Date: 8/1/2022 12:53:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[personCompSkillCUD] 
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
-- ---------- define context param: instid --------------
DECLARE @instid int;
SELECT @instid = instid FROM OPENJSON(@_json)
WITH (   
		instid int			'$.instid'
)

-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define Master object: [InstitutionPublicCouncil] ID param --------------
DECLARE @personCompSkillID int;
SELECT @personCompSkillID = personCompSkillID FROM OPENJSON(@_json)
WITH (   
			personCompSkillID int			'$.personCompSkillID'
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

PRINT 'INFO: PROCEDURE personCompSkillCUD started! Initial data gathered. PersonID=' + CAST(@personid AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW PersonCompSkill *************** */
IF ((@OperationType = 1) AND (@personid IS NOT NULL) AND (@personCompSkillID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [PersonCompSkill]...';
	INSERT INTO [inst_basic].[PersonCompSkill]
           (
				[PersonID]
				,[CompSkillID]
				,[CompSkillLevelID] 
				,[Notes] 
				,[SysUserID]
			)
	SELECT @personid, *, @sysuserid
	FROM OPENJSON(@_json)
	WITH (   
			compSkillID int
			,compSkillLevelID int
			,notes nvarchar(1024)
		
		); 

	-- GET INSERTED @PersonCompSkill (autoincrement)
	SELECT @personCompSkillID = SCOPE_IDENTITY();
	PRINT 'INFO: [PersonCompSkill] created with id=' + CAST(@personCompSkillID AS VARCHAR(12));
		
END  /* End of OperationType = 1 -> CREATE NEW *************** */

/* ############################## Start OperationType = 2 -> UPDATE Institution publicCouncil ############################## */
ELSE IF ((@OperationType = 2) AND (@personCompSkillID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [PersonCompSkill] with PersonCompSkillID=' + CAST(@personCompSkillID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[PersonCompSkill]
	SET 
			 [CompSkillID]=JPCS.compSkillID
			,[CompSkillLevelID]=JPCS.compSkillLevelID 
			,[Notes]=JPCS.notes
			,[SysUserID]=@sysuserid
				
	FROM OPENJSON(@_json)
	WITH (   
			compSkillID int
			,compSkillLevelID int
			,notes nvarchar(1024)
		) JPCS				 
	WHERE
		[inst_basic].[PersonCompSkill].[PersonCompSkillID] = @personCompSkillID;
	PRINT 'INFO: [PersonCompSkill] UPDATED successfully!';


END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 3 -> DELETE PersonCompSkill ############################## */
ELSE IF ((@OperationType = 3) AND (@personCompSkillID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [PersonCompSkill] with PersonCompSkillID=' + CAST(@personCompSkillID AS VARCHAR(12));

		DELETE FROM [inst_basic].[PersonCompSkill]
			WHERE [inst_basic].[PersonCompSkill].[PersonCompSkillID] = @personCompSkillID;
		PRINT 'INFO: [PersonCompSkill] DELETED with PersonCompSkillID=' + CAST(@personCompSkillID AS VARCHAR(12));
		
	SELECT @personCompSkillID AS id;
END /* End OperationType = 3 -> DELETE *************** */

ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', personid=' + ISNULL(CAST(@personid AS VARCHAR(12)),'') +
		', personCompSkillID=' + ISNULL(CAST(@personCompSkillID AS VARCHAR(12)),'');
	THROW 99004, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'PersonCompSkill', @personCompSkillID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'PersonCompSkill', @personCompSkillID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [personCompSkillCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
