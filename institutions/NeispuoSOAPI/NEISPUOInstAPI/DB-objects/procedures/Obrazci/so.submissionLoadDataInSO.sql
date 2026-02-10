USE [neispuo]
GO
/****** Object:  StoredProcedure [so].[submissionLoadDataInSO]    Script Date: 1.3.2022 г. 10:18:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [so].[submissionLoadDataInSO] 
	-- Add the parameters for the stored procedure here
	@submissionDataID int
AS
BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
PRINT 'INFO: PROCEDURE [so].[submissionLoadDataInSO] started! Start initializing parameters...';

PRINT 'INFO: Check if there is existing [SubmissionData] record with correct status: 10 (Started Loading data in SO) for SubmissionDataID=' + CAST(@submissionDataID AS VARCHAR(12));
DECLARE @hasSubmissionDataCorrectRecord int;
SELECT @hasSubmissionDataCorrectRecord = (SELECT COUNT(SubmissionDataID) FROM [so].[SubmissionData] 
	WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID 
	  AND [so].[SubmissionData].SubmissionStatusID = 9 
	  AND [so].[SubmissionData].[IsLast]=1);

IF (@hasSubmissionDataCorrectRecord <> 1)
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal main param submissionDataID! Either status is not 9 (Started Loading data in SO) OR it is not Last record for this submission for submissionDataID=' + CAST(@submissionDataID AS VARCHAR(12));
	THROW 99000, @ERR_MSG, 1;
END

UPDATE [so].[SubmissionData] 
	SET [so].[SubmissionData].SubmissionStatusID = 10 
	WHERE [so].[SubmissionData].SubmissionDataID = @submissionDataID; 

-- ---------- define Main params --------------
DECLARE @instID int;
SELECT @instID = (SELECT [InstitutionID] FROM [so].[SubmissionData] WHERE [SubmissionDataID] = @submissionDataID);

DECLARE @submissionYear int;
SELECT @submissionYear = (SELECT [SchoolYear] FROM [so].[SubmissionData] WHERE [SubmissionDataID] = @submissionDataID);

DECLARE @period int;
SELECT @period = (SELECT [Period] FROM [so].[SubmissionData] WHERE [SubmissionDataID] = @submissionDataID);

PRINT 'INFO: PROCEDURE [so].[submissionLoadDataInSO] initial data gathered. Context params are: InstitutionID=' + CAST(@instID AS VARCHAR(12)) + 
	'; SchoolYear=' + CAST(@submissionYear AS VARCHAR(12)) +  '; Period=' + CAST(@period AS VARCHAR(12));


-- ############################## Start Loading data in SO  ############################## 

DECLARE @schoolYear int;
EXEC @schoolYear = [inst_year].[getSchoolYearByInstID] @instID

EXEC [so_2022].[submissionDeleteDataInSO] @instID, @submissionYear, @period

EXEC [so_2022].[submissionLoadInstitutionDataInSO] @instID, @schoolYear, @submissionYear, @period

EXEC [so_2022].[submissionLoadBuildingDataInSO] @instID, @submissionYear, @period

EXEC [so_2022].[submissionLoadPersonDataInSO] @instID, @schoolYear, @submissionYear, @period

EXEC [so_2022].[submissionLoadRelativeDataInSO] @instID, @schoolYear, @submissionYear, @period

EXEC [so_2022].[submissionLoadSpecialNeedsDataInSO] @instID, @schoolYear, @submissionYear, @period

EXEC [so_2022].[submissionLoadClassDataInSO] @instID, @schoolYear, @submissionYear, @period

EXEC [so_2022].[submissionLoadStaffDataInSO] @instID, @submissionYear, @period

EXEC [so_2022].[submissionLoadCurriculumDataInSO] @instID, @schoolYear, @submissionYear, @period

-- SET correct status if Loading data in SO is SUCCESSFUL!
-- !!!!!!!!!!!! TEMPORARY COMMENT DURING DEV !!!!!!!!!!!!
UPDATE [so].[SubmissionData] 
	SET [so].[SubmissionData].SubmissionStatusID = 11 
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
           ,3
           ,'SUCCESSFUL'
           ,null
           ,SYSDATETIME())

PRINT 'INFO: PROCEDURE [so].[submissionLoadDataInSO] finished successfully with status update in [SubmissionData] to 11 (Данните са обобщени)!'

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
			   ,3
			   ,'ERROR'
			   ,null
			   ,SYSDATETIME())

	EXEC [logs].logError NULL, NULL, 103, 'SubmissionData', @submissionDataID, @schoolYear;

	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [so].[submissionLoadDataInSO] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
