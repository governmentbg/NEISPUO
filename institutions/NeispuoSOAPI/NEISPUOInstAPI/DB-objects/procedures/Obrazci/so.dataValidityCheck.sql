USE [neispuo]
GO
/****** Object:  StoredProcedure [so].[dataValidityCheck]    Script Date: 1.3.2022 г. 10:07:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [so].[dataValidityCheck] 
-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS
BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
PRINT 'INFO: PROCEDURE [so].[dataValidityCheck] started! Start initializing parameters...';

------------- define context param: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int			'$.instid'
)

DECLARE @schoolYear int;
SELECT @schoolYear = [inst_year].[getSchoolYearByInstID](@instID)
	
-- ---------- define Additional params --------------
DECLARE @hasResult int; -- true/false, като ако е true значи има данни във ValidityCheckResult

PRINT 'INFO: PROCEDURE [so].[dataValidityCheck] initial data gathered. Context is: InstitutionID=' + CAST(@instID AS VARCHAR(12));
/* ############################## Start Validity Check procedure ############################## */
IF (@instID IS NOT NULL)
BEGIN
	PRINT 'INFO: [so].[dataValidityCheck] Will call doValidityCheck procedure for InstitutionID=' + CAST(@instID AS VARCHAR(12)) + ' AND schoolYear=' + CAST(@schoolYear AS VARCHAR(12));

	EXEC @hasResult = [so_2022].[doValidityCheck] @instID, @schoolYear

	PRINT 'INFO: [so].[dataValidityCheck] COMPLETED successfully for InstitutionID=' + CAST(@instID AS VARCHAR(12)) + ' AND schoolYear=' + CAST(@schoolYear AS VARCHAR(12));
	/* return result to procedure caller */
	SELECT @hasResult AS hasResult;
END /* End Validity Check procedure *************** */
ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or missing required MASTER obejects params! Sent params are: ' + 
		'instID=' + ISNULL(CAST(@instID AS VARCHAR(12)),'');
	THROW 99000, @ERR_MSG, 1;
END

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'ValidityCheck', NULL, @schoolYear;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [so].[dataValidityCheck] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
