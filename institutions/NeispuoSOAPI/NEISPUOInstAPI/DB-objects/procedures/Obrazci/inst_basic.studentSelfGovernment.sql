USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[studentSelfGovernmentCUD]    Script Date: 8/1/2022 12:47:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




ALTER PROCEDURE [inst_basic].[studentSelfGovernmentCUD] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
-- ---------- define context param 1: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int	'$.instid'
)
-- ---------- define Master object: [SelfGovernment] ID param --------------
DECLARE @selfGovernmentID int;
SELECT @selfGovernmentID = selfGovernmentID FROM OPENJSON(@_json)
WITH (
		selfGovernmentID int			'$.Id'
)
-- ---------- define context param1: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define context param2: personid --------------
DECLARE @personid int;
SELECT @personid = personid FROM OPENJSON(@_json)
WITH (   
		personid int			'$.personId'
)

-- ---------- define context param2: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID)

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


PRINT 'INFO: PROCEDURE studentSelfGovernmentCD started!';

/* ************ OperationType = 1 -> CREATE NEW StudentSelfGovernment *************** */
IF ((@OperationType = 1) AND (@instID IS NOT NULL) AND (@selfGovernmentID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [StudentsSelfGovernment]...';
	INSERT INTO [student].[SelfGovernment]
           (
			[SchoolYear]
			,[ParticipationId]
			,[PersonId]
            ,[PositionId]
			,[AdditionalInformation]
			,[InstitutionId]
			,[CreatedBySysUserID]
			,[CreateDate])
	SELECT @schoolYear, 3, *, @instID, @sysuserid,GETDATE()
	FROM OPENJSON(@_json)
	WITH (   
			personId int
			,positionId int
			,additionalInformation nvarchar(1024)
		); 

	-- GET INSERTED @selfGovernmentID (autoincrement)
	SELECT @selfGovernmentID = SCOPE_IDENTITY();
	PRINT 'INFO: [SelfGovernment] created with id=' + CAST(@selfGovernmentID AS VARCHAR(12));
	
	UPDATE top (1) [student].[Student]
	SET 
	    [MobilePhone]=JIPC.mobilePhone
		,[Email]=JIPC.email

	FROM OPENJSON(@_json)
	WITH (   
			 mobilePhone nvarchar(100)
			,email nvarchar(100)
		) JIPC				 
	WHERE
		[student].[Student].PersonID = @personid;

	PRINT 'INFO: [Student] UPDATED successfully!';
	

	SELECT @selfGovernmentID AS Id;

END  /* End of OperationType = 1 -> CREATE NEW StudentSelfGovernment *************** */

/* ############################## Start OperationType = 2 -> UPDATE StudentSelfGovernment ############################## */
ELSE IF ((@OperationType = 2) AND (@instid IS NOT NULL) AND (@selfGovernmentID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [StudentSelfGovernment] with ID=' + CAST(@selfGovernmentID AS VARCHAR(12));
	UPDATE top (1) [student].[SelfGovernment]
	SET 
	    [PositionId]=JIPC.positionId
		,[AdditionalInformation]=JIPC.additionalInformation
		,[ModifiedBySysUserId]=@sysuserid
		,[ModifyDate]=GETDATE()
	
	FROM OPENJSON(@_json)
	WITH (   
			 positionId int
			,additionalInformation nvarchar(1024)
		) JIPC				 
	WHERE
		[student].[SelfGovernment].Id = @selfGovernmentID;

	PRINT 'INFO: [SelfGovernment] UPDATED successfully!';

	UPDATE top (1) [student].[Student]
	SET 
	    [MobilePhone]=JIPC.mobilePhone
		,[Email]=JIPC.email

	FROM OPENJSON(@_json)
	WITH (   
			 mobilePhone nvarchar(100)
			,email nvarchar(100)
		) JIPC				 
	WHERE
		[student].[Student].PersonID = @personid;

	PRINT 'INFO: [Student] UPDATED successfully!';

END  /* End OperationType = 2 -> UPDATE SelfGovernment *************** */

/* ############################## Start OperationType = 3 -> DELETE SelfGovernmentID ############################## */
ELSE IF ((@OperationType = 3) AND (@instID IS NOT NULL) AND (@selfGovernmentID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [student].[StudentSelfGovernment] with Id=' + CAST(@selfGovernmentID AS VARCHAR(12));

		DELETE FROM [student].[SelfGovernment]
			WHERE [student].[SelfGovernment].Id = @selfGovernmentID;
		PRINT 'INFO: [StudentSelfGovernment] DELETED with InstitutionPublicCouncilID=' + CAST(@selfGovernmentID AS VARCHAR(12));
		
	SELECT @selfGovernmentID AS id;
END /* End OperationType = 3 -> DELETE [StudentSelfGovernment] *************** */

ELSE
	THROW 99003, 'Procedure [studentSelfGovernmentCD] - Error! Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'StudentSelfGovernement', @selfGovernmentID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'StudentSelfGovernement', @selfGovernmentID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [StudentSelfGovernment] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
