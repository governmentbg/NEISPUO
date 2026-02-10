USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[distanceLearningConditionCUD]    Script Date: 1.3.2022 г. 8:37:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[distanceLearningConditionCUD] 
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

-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define Master object: [DistanceLearningCondition] ID param --------------
DECLARE @distanceLearningConditionID int;
SELECT @distanceLearningConditionID = distanceLearningConditionID FROM OPENJSON(@_json)
WITH (   
		distanceLearningConditionID int				'$.distanceLearningConditionID'
);


PRINT 'INFO: PROCEDURE distanceLearningConditionCUD started! Initial data gathered. distanceLearningConditionID=' + CAST(@distanceLearningConditionID AS VARCHAR(12));

/* ############################## Start OperationType = 2 -> UPDATE [distanceLearningCondition] ############################## */
IF ((@OperationType = 2) AND (@distanceLearningConditionID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [distanceLearningConditionID] with distanceLearningConditionID=' + CAST(@distanceLearningConditionID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[DistanceLearningCondition]
	SET
	   [CompliedDegree] = JDLC.compliedDegree
	   , [SysUserID] = @sysuserid
	FROM OPENJSON(@_json)
	WITH (
	   compliedDegree int
	) JDLC 
	WHERE 
		[inst_basic].[DistanceLearningCondition].DistanceLearningConditionID = @distanceLearningConditionID;
	PRINT 'INFO: [DistanceLearningCondition] UPDATED successfully!';

	SELECT @distanceLearningConditionID AS distanceLearningConditionID; -- return back to the caller i.e. front-end

END  /* End OperationType = 2 -> UPDATE DistanceLearningCondition *************** */

ELSE
	THROW 99103, '[DistanceLearningCondition] Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'DistanceLearningCondition', @distanceLearningConditionID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'DistanceLearningCondition', @distanceLearningConditionID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [DistanceLearningCondition] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
