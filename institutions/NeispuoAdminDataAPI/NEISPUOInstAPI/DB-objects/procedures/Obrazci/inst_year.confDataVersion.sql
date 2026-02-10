USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_year].[confDataVersion]    Script Date: 1.3.2022 г. 9:43:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [inst_year].[confDataVersion]

		-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int

AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/

DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int			'$.instid'
)

PRINT 'INFO: PROCEDURE instConfData started! Initial data gathered. confDataID=' + CAST(@instID AS VARCHAR(12));

/* ############################## Start OperationType = 2 -> UPDATE Conf data ############################## */
IF ((@OperationType = 2) AND (@instID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE object: [core].[InstitutionConfData] with @instID=' + CAST(@instID AS VARCHAR(12));

	DECLARE @SOVersion smallint; 
	DECLARE @CBVersion smallint;
	
	SET @SOVersion = (SELECT SOVersion FROM [core].[InstitutionConfData] where InstitutionID = @instID);
	SET @CBVersion = (SELECT CBVersion FROM [core].[InstitutionConfData] where InstitutionID = @instID);

	UPDATE top (1) [core].[InstitutionConfData]
	SET 
			 [SOVersion]=@SOVersion + 1
			,[CBVersion]=@CBVersion + 1			 
	WHERE
		[core].[InstitutionConfData].InstitutionID = @instID;
END
ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or missing required MASTER obejects params! Sent params are: ' + 
		'instID=' + ISNULL(CAST(@instID AS VARCHAR(12)),'');
	THROW 99000, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'InstitutionConfData', @instID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'InstitutionConfData', @instID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [core].[InstitutionConfData] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH




/* ************ END OF PROCEDURE ************ */


