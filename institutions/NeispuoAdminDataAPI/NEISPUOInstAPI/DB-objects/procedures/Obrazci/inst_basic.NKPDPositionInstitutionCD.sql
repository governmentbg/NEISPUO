USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[NKPDPositionInstitutionCD]    Script Date: 8/5/2022 8:25:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [inst_basic].[NKPDPositionInstitutionCD] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int --1 - add, 3 - delete
AS 
BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
-- ---------- define context param 1: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int	'$.instid'
);

-- ---------- define context param 2: NKPDPositionId --------------
DECLARE @NKPDPositionId int;
SELECT @NKPDPositionId = NKPDPositionId FROM OPENJSON(@_json)
WITH (   
		NKPDPositionId int	'$.NKPDPositionIdChoice'
);
-- ---------- define context param 3: CustomVarValueID --------------
DECLARE @customVarValueID int;
SELECT @customVarValueID = customVarValueID FROM OPENJSON(@_json)
WITH (   
		customVarValueID int	'$.customVarValueID'
);
-- ---------- define context param 4: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID);

--------------start operation type 1 - add new occupation---------------
IF ((@OperationType = 1) AND (@instID IS NOT NULL) AND (@customVarValueID IS NULL) )
	BEGIN
			INSERT INTO [inst_nom].[CustomVarValue] 
				(
						[InstitutionID]
						,[CustomVarID]
						,[CustomVarValue]
						,[IsValid]
				)

				SELECT @instid, 2, *, 1
				FROM OPENJSON(@_json)
				WITH 
				(   
						NKPDPositionIdChoice int
				); 
				-- GET INSERTED @customVarValueID (autoincrement)
				SELECT @customVarValueID = SCOPE_IDENTITY(); 
				PRINT 'INFO: [customVarValue] created with id=' + CAST(@customVarValueID AS VARCHAR(12));
				SELECT @customVarValueID AS customVarValueID; -- return back to the caller i.e. front-end

	END					

-------------start operation type 3 - delete occupation---------------
ELSE IF @OperationType = 3 --AND @CheckOccExist IS NOT NULL 
	BEGIN
		DELETE [inst_nom].[CustomVarValue] 
		WHERE InstitutionID = @instID AND CustomVarValueID = @CustomVarValueID;

		COMMIT TRANSACTION
		RETURN
	END
ELSE 
	BEGIN
		ROLLBACK TRANSACTION
		PRINT 'INFO: TRANSACTION has been ROLLBACKED FOR OperationType = 3';
		RETURN
	END
COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED IN CATCH!';

	EXEC [logs].logError @_json, @OperationType, 103, 'NKPDPositionInstitutionCD', @NKPDPositionId, @schoolYear;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [NKPDPositionInstitutionCD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */