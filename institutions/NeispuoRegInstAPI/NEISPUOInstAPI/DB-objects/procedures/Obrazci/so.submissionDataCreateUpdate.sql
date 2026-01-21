USE [neispuo]
GO
/****** Object:  StoredProcedure [so].[submissionDataCreateUpdate]    Script Date: 1.3.2022 г. 10:10:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [so].[submissionDataCreateUpdate] 
-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS
BEGIN

	/************** Setup initial params ******************/
------------- define context param: institutionID --------------
DECLARE @instID int;
SELECT @instID = institutionID FROM OPENJSON(@_json)
WITH (   
		institutionID int			'$.instid'
)
------------- define context param: currDate --------------
DECLARE @currDate DATETIME;
SET @currDate = GETDATE();

------------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = CAST (schoolYear AS int) FROM OPENJSON(@_json)
WITH (   
		schoolYear int			'$.schoolYear'
)

------------- define context param: period --------------
DECLARE @period int;
SELECT @period = CAST (submissionPeriod AS int) FROM OPENJSON(@_json)
WITH (   
		submissionPeriod int			'$.period'
)

------------- define context param: submissionDataID --------------
DECLARE @submissionDataID int;
SELECT @submissionDataID = submissionDataID FROM OPENJSON(@_json)
WITH (   
		submissionDataID int			'$.submissionDataID'
)

------------- define context param: submissionStatusID --------------
DECLARE @submissionStatusID int;
SELECT @submissionStatusID = submissionStatusID FROM OPENJSON(@_json)
WITH (   
		submissionStatusID int			'$.submissionStatusID'
)

-- ---------- define context param: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define context param: specialInstitution --------------
DECLARE @specialInstType int;
SELECT @specialInstType = specialInstType FROM OPENJSON(@_json)
WITH (   
		specialInstType int			'$.specialInstType'
)

-- ---------- Define Details object 1: [so].[SubmissionDetail] ID and string params ---
DECLARE @SubmissionDetailVar TABLE  
(  
	id INT
);
DECLARE @SubmissionDetailString NVARCHAR(4000);

-- ---------- Define Details object 2: SubmissionCertificate ID and string params ---
DECLARE @SubmissionCertificateVar TABLE  
(  
	id INT
);
DECLARE @SubmissionCertificateString NVARCHAR(4000);

-- ---------- define addtional param: signer --------------
DECLARE @signer nvarchar(max);
SELECT @signer = signer FROM OPENJSON(@_json)
WITH (   
		signer nvarchar(max)			'$.signer'
)


PRINT 'INFO: PROCEDURE submissionDataCreateUpdate started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW [SubmissionData] *************** */
IF ((@OperationType = 1) AND (@submissionDataID IS NULL))
BEGIN

	/* ************ OperationType = 1 -> Update if exist certificate for this period *************** */
	UPDATE [so].[SubmissionData]
	SET 
			 [IsFinal]=0
			,[SubmissionStatusID]=13
			 
	WHERE
		[so].[SubmissionData].[InstitutionID] = @instID 
		AND [so].[SubmissionData].[SubmissionStatusID]=12 
		AND [so].[SubmissionData].[SchoolYear] = @schoolYear
		AND [so].[SubmissionData].[Period] = @period;
	
	/* ************ OperationType = 1 -> Create new submission for this period *************** */

	PRINT 'INFO: OperationType: 1 - CREATE NEW [SubmissionData]...';
	INSERT INTO [so].[SubmissionData]
	(
	   [InstitutionID]
	  ,[Comment]
      ,[SchoolYear]
      ,[Period]
      ,[IsLocked]
      ,[IsLast]
	  ,[SubmissionStatusID]
	  ,[ModifiedOn]
	  ,[IsFinal]

	)
	SELECT *,@schoolYear,@period,1,1,0,@currDate,0
	FROM OPENJSON(@_json)
	WITH (
		instid int						'$.instid' 	
		,comment int					'$.comment' 

	);

	-- GET INSERTED SubmissionDataID (autoincrement)
	SELECT @submissionDataID = SCOPE_IDENTITY();
	PRINT 'INFO: [so].[SubmissionData] created with id=' + CAST(@submissionDataID AS VARCHAR(12));

	SELECT @submissionDataID AS submissionDataID; -- return back to the caller i.e. front-end

END  /* End of OperationType = 1 -> CREATE NEW SubmissionData *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData ############################## */
					
ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=2))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	

	EXEC [so_2022].[submissionValidityCheck] @submissionDataID

	SET @submissionStatusID = (SELECT [SubmissionStatusID] FROM [so].[SubmissionData] WHERE [so].[SubmissionData].[SubmissionDataID]=@submissionDataID) ; -- return back to the caller i.e. front-end
	SELECT @submissionStatusID AS submissionStatusID;

	IF @submissionStatusID=98
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [IsLocked]=0
			,[IsLast]=0

			 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;


END  /* End OperationType = 2 -> UPDATE *************** */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=3))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

		/* *********************** START INSERT INTO SubmissionDetail -> AccountantSign************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 1, @currDate, @signer, @sysuserid
	FROM OPENJSON(@_json, '$.accountantSignDataCreated')
	WITH (   
		accountantSignDataType int	'$.accountantSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */

	SELECT @SubmissionDetailString AS submissionDetailIDs;
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> DirectorSign ############################## */
ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=4))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

		/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 1, @currDate, @signer, @sysuserid
	FROM OPENJSON(@_json, '$.directorSignDataCreated')
	WITH (   
		directorSignDataType int	'$.directorSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */

	SELECT @SubmissionDetailString AS submissionDetailIDs;

	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> DirectorReturnForCorrectionAfterValidityCheck ############################## */
ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=5))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 2, @currDate, @sysuserid
	FROM OPENJSON(@_json, '$.validationDataUpdated')
	WITH (   
		validationSignDataType int	'$.validationSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> DirectorReturnForCorrectionAfterAccountantSign ############################## */
ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=5))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 2, @currDate, @sysuserid
	FROM OPENJSON(@_json, '$.directorSignDataCreated')
	WITH (   
		directorSignDataType int	'$.directorSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> BudgetingInstitutionSign ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=6) AND (@specialInstType=1))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

		/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[Comment]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 1, @currDate, @signer, @sysuserid
	FROM OPENJSON(@_json, '$.budgetingInstitutionSignDataCreated')
	WITH (   
		budgetingInstitutionDataType int	'$.budgetingInstitutionDataType'
		,budgetingInstitutionComment int	'$.budgetingInstitutionComment'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */

	SELECT @SubmissionDetailString AS submissionDetailIDs;
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> BudgetingInstitutionSignWithComments ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=7) AND (@specialInstType=1))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				,[Comment]
				,[UserTypeID]
				
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, 1, @currDate, @signer, @sysuserid, *
	FROM OPENJSON(@_json, '$.budgetingInstitutionSignDataCreated')
	WITH (   
		budgetingInstitutionComment nvarchar(max)	'$.budgetingInstitutionComment',
		budgetingInstitutionDataType int	'$.budgetingInstitutionDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> BudgetingInstitutionReturnForCorrection ############################## */
ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=8)  AND (@specialInstType=1))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';

	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				,[Comment]
				,[UserTypeID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, 2, @currDate, @sysuserid, *
	FROM OPENJSON(@_json, '$.budgetingInstitutionSignDataCreated')
	WITH (   
		budgetingInstitutionComment nvarchar(max)	'$.budgetingInstitutionComment',
		budgetingInstitutionDataType int	'$.budgetingInstitutionDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
	
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> ConfirmationDirectorForNonSpecialInstitution ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=9) AND (@specialInstType=0))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';
	
	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 1, @currDate, @signer, @sysuserid
	FROM OPENJSON(@_json, '$.directorConfirmationSignDataCreated')
	WITH (   
		directorConfirmationSignDataType int	'$.directorConfirmationSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */


	EXEC [so].[submissionLoadDataInSO] @submissionDataID 
		
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> ConfirmationDirectorForSpecialInstitution ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=9) AND (@specialInstType=1))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';
	
	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SignerName]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 1, @currDate, @signer, @sysuserid
	FROM OPENJSON(@_json, '$.directorConfirmationAfterBISignDataCreated')
	WITH (   
		directorConfirmationAfterBISignDataType int	'$.directorConfirmationAfterBISignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */


	EXEC	[so].[submissionLoadDataInSO] @submissionDataID 

		
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> ReturnForCorrectionForNonSpecialInstitution ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=13) AND (@specialInstType=0))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';
	
	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 2, @currDate, @sysuserid
	FROM OPENJSON(@_json, '$.directorConfirmationSignDataCreated')
	WITH (   
		directorConfirmationSignDataType int	'$.directorConfirmationSignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
		
END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> ReturnForCorrectionForSpecialInstitution ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=13) AND (@specialInstType=1))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int
		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';
	
	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				)
	OUTPUT INSERTED.SubmissionDetailID INTO @SubmissionDetailVar
	SELECT @submissionDataID, *, 2, @currDate, @sysuserid
	FROM OPENJSON(@_json, '$.directorConfirmationAfterBISignDataCreated')
	WITH (   
		directorConfirmationAfterBISignDataType int	'$.directorConfirmationAfterBISignDataType'
		); 
	
	SET @SubmissionDetailString = (SELECT STRING_AGG(id, ',') FROM @SubmissionDetailVar);
	PRINT 'INFO: [SubmissionDetail] created with ids=' + @SubmissionDetailString;
	/* *********************** END INSERT INTO [SubmissionDetail] ************************************ */
		
END  /* End OperationType = 2 -> UPDATE *************** */


/* ############################## Start OperationType = 2 -> UPDATE SubmissionData -> GenerateCertificate ############################## */

ELSE IF ((@OperationType = 2) AND (@submissionDataID IS NOT NULL) AND (@submissionStatusID=12))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [SubmissionData] with SubmissionDataD=' + CAST(@submissionDataID AS VARCHAR(12));
	UPDATE top (1) [so].[SubmissionData]
	SET 
			 [SubmissionStatusID]=JPCS.submissionStatusID
			,[ModifiedOn]=@currDate
			,[IsLocked]=0
			,[IsLast]=0
			,[IsFinal]=1
				
	FROM OPENJSON(@_json)
	WITH (   
			 submissionStatusID int

		) JPCS				 
	WHERE
		[so].[SubmissionData].[SubmissionDataID] = @submissionDataID;
	PRINT 'INFO: [SubmissionData] UPDATED successfully!';
	
	/* *********************** START INSERT INTO SubmissionCertificate ************************************ */ 
	INSERT INTO [so].[SubmissionCertificate]
				(
				[SubmissionDataID]
				,[GenerateCertificateFile]

				)
	OUTPUT INSERTED.SubmissionCertificateID INTO @SubmissionCertificateVar
	SELECT @submissionDataID, *
	FROM OPENJSON(@_json, '$.generateCertificateDataCreated')
	WITH (   
		blobID int	'$.blobID'
		); 
	
	SET @SubmissionCertificateString = (SELECT STRING_AGG(id, ',') FROM @SubmissionCertificateVar);
	PRINT 'INFO: [SubmissionCertificate] created with ids=' + @SubmissionCertificateString;

	/* *********************** START INSERT INTO SubmissionDetail ************************************ */ 
	INSERT INTO [so].[SubmissionDetail]
				(
				[SubmissionDataID]
				,[UserTypeID]
				,[EventTypeID]
				,[EventDate]
				,[SysUserID]
				)
	VALUES (@submissionDataID,0,4,@currDate,@sysuserid)
		
END  /* End OperationType = 2 -> UPDATE *************** */

EXEC [logs].logEvent @_json, @OperationType, 103, 'SubmissionData', @submissionDataID, @schoolYear;

END
