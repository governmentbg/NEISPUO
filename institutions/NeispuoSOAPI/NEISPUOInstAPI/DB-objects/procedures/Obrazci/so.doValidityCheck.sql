USE [neispuo]
GO
/****** Object:  StoredProcedure [so_2022].[doValidityCheck]    Script Date: 8.2.2022 г. 14:09:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [so_2022].[doValidityCheck]
	-- Add the parameters for the stored procedure here
	@instID int,
	@schoolYear int
AS

/************** Setup initial params ******************/
PRINT 'INFO: PROCEDURE [so_2022].[submissionValidityCheck] started! Start initializing parameters...';
-- ---------- define Additional params --------------
DECLARE @hasResult int; -- true/false, като ако е true значи има данни във ValidityCheckResult
SET @hasResult = 0;

PRINT 'INFO: PROCEDURE [doValidityCheck] initial data gathered. Context is: InstitutionID=' + CAST(@instID AS VARCHAR(12)) + '; schoolYear=' + CAST(@schoolYear AS VARCHAR(12));

/* ############################## Start Validity Check procedure ############################## */

-- DECLARE ValidityCheck params
DECLARE @validityCheckID INT, @validityCheckSQL NVARCHAR(4000), @validityCheckType INT, 
		@hasList BIT, @isSchoolYearRelated BIT, @isValid BIT;

-- DECLARE ValidityCheckDetails params
DECLARE @validityCheckDetailID int, @validityCheckDetailMessage NVARCHAR(4000), 
        @instTypeID int, @detailedSchoolTypeID int, 
		@descriptionDetails NVARCHAR(max), @isValidDetails bit;

-- DECLARE Loop params
DECLARE @Counter INT, @MaxId INT;

-- validity resultSet params 
DECLARE @validityResultSetVar table (InfoText varchar(1000));
DECLARE @validityResultSetString NVARCHAR(4000);
DECLARE @validityResultSetCount int;

PRINT 'INFO: PROCEDURE [doValidityCheck] Initialize Loop params....'
-- Initialize Loop params
SELECT @Counter = min(VCD.ValidityCheckDetailID), @MaxId = max(VCD.ValidityCheckDetailID) 
  FROM [so_2022].[ValidityCheckDetails] VCD;

-- DELETE OLD [ValidityCheckResults]
DELETE FROM [so_2022].[ValidityCheckResults] 
		WHERE [ValidityCheckResults].InstitutionID = @instID;

WHILE(@Counter IS NOT NULL AND @Counter <= @MaxId)
BEGIN
	PRINT 'DEBUG: [doValidityCheck] loop started. Current interation is for counter=' + CAST(@Counter AS VARCHAR(12));
	-- Initialize ValidityCheck params
	SET @validityCheckDetailID = NULL;
	SELECT @validityCheckID = VC.ValidityCheckID, @validityCheckSQL = VC.ValidityCheckSQL, @validityCheckType = VC.ValidityCheckType, 
		   @hasList = VC.HasList, @isValid = VC.IsValid, @isSchoolYearRelated = VC.IsSchoolYearRelated, 
		   @validityCheckDetailID = VCD.ValidityCheckDetailID, @instTypeID = VCD.InstTypeID, 
		   @detailedSchoolTypeID = VCD.DetailedSchoolTypeID, @validityCheckDetailMessage = VCD.Message, 
		   @descriptionDetails = VCD.Description, @isValidDetails = VCD.IsValid
		FROM [so_2022].[ValidityCheckDetails] VCD 
		JOIN [so_2022].[ValidityCheck] VC
		ON VCD.ValidityCheckID = VC.ValidityCheckID
		WHERE VCD.ValidityCheckDetailID = @Counter;

	IF ((@validityCheckDetailID IS NOT NULL) AND (@isValid = 1) AND (@isValidDetails = 1))
	BEGIN
		PRINT 'DEBUG: [doValidityCheck] loop >> Valid validity check record found for ValidityCheckDetailID=' + CAST(@validityCheckDetailID AS VARCHAR(12));
		PRINT 'DEBUG: [doValidityCheck] loop >> Original validityCheckSQL=' + ISNULL(CAST(@validityCheckSQL AS NVARCHAR(4000)),'');
		PRINT 'DEBUG: [doValidityCheck] loop >> Other Validity check params are: ' + 
			'validityCheckType=' + ISNULL(CAST(@validityCheckType AS VARCHAR(12)),'') +
			', hasList=' + ISNULL(CAST(@hasList AS VARCHAR(12)),'') +
			', isSchoolYearRelated=' + ISNULL(CAST(@isSchoolYearRelated AS VARCHAR(12)),'') +
			', isValid=' + ISNULL(CAST(@isValid AS VARCHAR(12)),'') +
			', instTypeID=' + ISNULL(CAST(@instTypeID AS VARCHAR(12)),'') +
			', detailedSchoolTypeID=' + ISNULL(CAST(@detailedSchoolTypeID AS VARCHAR(12)),'') +
			', validityCheckDetailMessage=' + ISNULL(CAST(@validityCheckDetailMessage AS VARCHAR(12)),'') +
			', descriptionDetails=' + ISNULL(CAST(@descriptionDetails AS VARCHAR(12)),'') +
			', isValidDetails=' + ISNULL(CAST(@isValidDetails AS VARCHAR(12)),'');

		-- extend original SQL check with dynamic params
		DECLARE @validityCheckSQLExtended NVARCHAR(4000);
		SET @validityCheckSQLExtended = @validityCheckSQL + 
				' AND T.InstitutionID = ' + CAST(@instID AS VARCHAR(12)) + 
				' AND NOT T.InstitutionID IN (SELECT W.InstitutionID 
	                                            FROM so_2022.ValidityCheckWhitelist W
                                               WHERE W.IsValid = 1
											     AND W.ValidityCheckID = ' + CAST(@validityCheckID AS VARCHAR(12)) + ')';
				-- ' AND D.InstType = ' + ISNULL(CAST(@instTypeID AS  VARCHAR(12)), 'D.InstType');

		IF (@isSchoolYearRelated = 1) 
			SET @validityCheckSQLExtended = @validityCheckSQLExtended + ' AND T.SchoolYear =' + CAST(@schoolYear AS VARCHAR(12)); 
				
		IF (@instTypeID IS NOT NULL) 
			SET @validityCheckSQLExtended = @validityCheckSQLExtended + ' AND D.InstType = ' + CAST(@instTypeID AS  VARCHAR(12));
		
		IF (@detailedSchoolTypeID IS NOT NULL) 
			SET @validityCheckSQLExtended = @validityCheckSQLExtended + ' AND I.DetailedSchoolTypeID = ' + CAST(@detailedSchoolTypeID AS  VARCHAR(12));
		
		PRINT 'DEBUG: [doValidityCheck] loop >> Extended validityCheckSQL is:' + ISNULL(CAST(@validityCheckSQLExtended AS NVARCHAR(4000)),'---');

		-- Do the magic :-)
		DELETE @validityResultSetVar;
		INSERT INTO @validityResultSetVar ([InfoText]) exec (@validityCheckSQLExtended);

		IF (@hasList = 0) 
		BEGIN
			SET @validityResultSetCount = CAST((SELECT (InfoText) FROM @validityResultSetVar) AS INT);
			PRINT 'DEBUG: [doValidityCheck] loop >> validityResultSetCount is:' + CAST(@validityResultSetCount AS VARCHAR(12));
			SET @validityResultSetString = '';			
		END
		ELSE
		BEGIN
			SET @validityResultSetCount = (SELECT COUNT(InfoText) FROM @validityResultSetVar);
			PRINT 'DEBUG: [doValidityCheck] loop >> validityResultCount is:' + ISNULL(CAST(@validityResultSetCount AS VARCHAR(12)),'');
			SET @validityResultSetString = (SELECT STRING_AGG(InfoText, ',<br/>') FROM @validityResultSetVar);
			PRINT 'DEBUG: [doValidityCheck] loop >> validityResultSet is:' + ISNULL(CAST(@validityResultSetString AS NVARCHAR(4000)),'');
		END
	
		IF (@validityResultSetCount > 0) 
		BEGIN
			PRINT 'DEBUG: [doValidityCheck] loop >> Found validityResultSet count=' + ISNULL(CAST(@validityResultSetCount AS VARCHAR(12)),'');
			-- Insert [SubmissionDetail] data
			INSERT INTO [so_2022].[ValidityCheckResults]
					   ([InstitutionID]
					   ,[ValidityCheckDetailID]
					   ,[Message]
					   ,[Result]
					   ,[CreatedOn])
				 VALUES
					   (@instID 
					   ,@validityCheckDetailID
					   ,@validityCheckDetailMessage
					   ,@validityResultSetString
					   ,SYSDATETIME())
			PRINT 'DEBUG: [doValidityCheck] loop >> Inserted record in [ValidityCheckResults] with ValidityCheckDetailID=' + ISNULL(CAST(@validityCheckDetailID AS VARCHAR(12)),'');

			IF (@validityCheckType = 2) SET @hasResult = @hasResult + 1;
		END		
		PRINT 'DEBUG: [doValidityCheck] loop >> Completed ValidityCheck iteration with ValidityCheckDetailID=' + ISNULL(CAST(@validityCheckDetailID AS VARCHAR(12)),'');
	END

	SET @Counter = @Counter  + 1;
END

PRINT 'DEBUG: [doValidityCheck] COMPLETED! Found results count='+ ISNULL(CAST(@hasResult AS VARCHAR(12)),'');
IF (@hasResult > 0)
	SET @hasResult = 1;

PRINT 'DEBUG: [doValidityCheck] Returned result is hasResult='+ ISNULL(CAST(@hasResult AS VARCHAR(12)),'');
RETURN @hasResult;
/* ************ END OF PROCEDURE ************ */
