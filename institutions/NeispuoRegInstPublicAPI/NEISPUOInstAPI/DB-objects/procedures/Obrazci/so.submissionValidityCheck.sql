USE [neispuo]
GO
/****** Object:  StoredProcedure [so_2022].[submissionValidityCheck]    Script Date: 1.3.2022 г. 10:23:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [so_2022].[submissionValidityCheck] 
	-- Add the parameters for the stored procedure here
	@submissionDataID int
AS

BEGIN TRY

/************** Setup initial params ******************/
PRINT 'INFO: PROCEDURE [so_2022].[submissionValidityCheck] started! Start initializing parameters...';

-- ---------- define Main params --------------
DECLARE @instID int;
SELECT @instID = (SELECT [InstitutionID] FROM [so].[SubmissionData] WHERE [SubmissionDataID] = @submissionDataID);

DECLARE @schoolYear int;
SELECT @schoolYear = [inst_year].[getSchoolYearByInstID](@instID)

DECLARE @hasResult int; -- true/false, като ако е true значи има данни във ValidityCheckResult с вид грешка = 2

PRINT 'INFO: PROCEDURE [so_2022].[submissionValidityCheck] initial data gathered. Context params are: InstitutionID=' + CAST(@instID AS VARCHAR(12)) + 
			'; SchoolYear=' + CAST(@schoolYear AS VARCHAR(12));


-- ############################## DO necessary checks before to start Loading data in SO  ############################## 
PRINT 'INFO: Check if there is existing [SubmissionData] record with correct status: 0 (New submission) for SubmissionDataID=' + CAST(@submissionDataID AS VARCHAR(12));
DECLARE @hasSubmissionDataCorrectRecord int;
SELECT @hasSubmissionDataCorrectRecord = (SELECT COUNT(SubmissionDataID) FROM [so].[SubmissionData] 
	WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID 
	  AND [so].[SubmissionData].SubmissionStatusID = 0 
	  AND [so].[SubmissionData].[IsLast]=1);

/* TEMPORARY COMMENT FOR TEST */
IF (@hasSubmissionDataCorrectRecord <> 1)
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = '[submissionValidityCheck] ERROR! Illegal main param submissionDataID! Status has to be 0 (New submission) AND for the LAST record for this submission with submissionDataID=' + CAST(@submissionDataID AS VARCHAR(12));
	THROW 99000, @ERR_MSG, 1;
END
/* TEMPORARY COMMENT FOR TEST */

-- ############################## SET correct status before validating data in SO_2022  ######################
BEGIN TRANSACTION
UPDATE [so].[SubmissionData] 
	SET [so].[SubmissionData].SubmissionStatusID = 1 
	WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID; 

-- Commit transaction to persist status before to start actual check
COMMIT TRANSACTION

BEGIN TRANSACTION

EXEC @hasResult = [so_2022].[doValidityCheck] @instID, @schoolYear

PRINT 'PROCEDURE [so].[submissionValidityCheck] recived hasResult=' + CAST(@hasResult AS VARCHAR(12));

-- ############################## SET correct status if Validity Check is SUCCESSFUL! ##############################
IF (@hasResult = 0)
BEGIN
	UPDATE [so].[SubmissionData] SET [so].[SubmissionData].SubmissionStatusID = 2 -- Проверка на данните (преминала успешно) 
		WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID; 
	PRINT 'INFO: PROCEDURE [so].[submissionValidityCheck] finished WITHOUT validity errors. Submission status updated in [SubmissionData] to 2 - Проверка на данните (преминала успешно)!'
END
ELSE
BEGIN
	UPDATE [so].[SubmissionData] SET [so].[SubmissionData].SubmissionStatusID = 98 -- Проверка на данните (неуспешна) 
		WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID; 
	PRINT 'INFO: PROCEDURE [so].[submissionValidityCheck] finished WITH validity errors. Submission status updated in [SubmissionData] to 98 - Проверка на данните (неуспешна)!'
END;

-- Insert [SubmissionDetail] data
INSERT INTO [so].[SubmissionDetail]
           ([SubmissionDataID]
           ,[SysUserID]
           ,[UserTypeID]
           ,[EventTypeID]
           ,[Comment]
           ,[SignerName]
           ,[EventDate])
     VALUES
           (@submissionDataID 
		   ,1
           ,0
           ,0 -- валидиране
           ,'SUCCESSFUL'
           ,null
           ,SYSDATETIME())

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	UPDATE [so].[SubmissionData] 
		SET [so].[SubmissionData].SubmissionStatusID = 99 
		WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID; 

	INSERT INTO [so].[SubmissionDetail]
			   ([SubmissionDataID]
			   ,[SysUserID]
			   ,[UserTypeID]
			   ,[EventTypeID]
			   ,[Comment]
			   ,[SignerName]
			   ,[EventDate])
		 VALUES
			   (@submissionDataID 
			   ,1
			   ,0
			   ,0 -- валидиране
			   ,'ERROR'
			   ,null
			   ,SYSDATETIME())

	EXEC [logs].logError NULL, NULL, 103, 'SubmissionValidityCheck', @submissionDataID, @schoolYear;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [so_2022].[submissionValidityCheck] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
