USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[studentUpdate]    Script Date: 8/1/2022 12:48:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[studentUpdate] 
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
-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear smallint;
SELECT @schoolYear = schoolYear FROM OPENJSON(@_json)
WITH (   
		schoolYear smallint			'$.schoolYear'
)
-- ---------- define Master object param: personid --------------
DECLARE @personID int;
SELECT @personID = personID FROM OPENJSON(@_json)
WITH (   
		personID int			'$.personid'
)

-- ---------- define Detail object: [student].[InternationalProtection] ---
DECLARE @internationalProtectionCreatedVar TABLE  
(  
	id INT
);
DECLARE @internationalProtectionCreatedString NVARCHAR(4000);

DECLARE @internationalProtectionUpdatedVar TABLE  
(  
	id INT
);
DECLARE @internationalProtectionUpdatedString NVARCHAR(4000);

DECLARE @internationalProtectionDeletedString NVARCHAR(4000);

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


PRINT 'INFO: PROCEDURE studentUpdate started! Initial data gathered. personID=' + CAST(@personID AS VARCHAR(12));

/* ############################## Start OperationType = 2 -> UPDATE Student data ############################## */
IF ((@OperationType = 2) AND (@instID IS NOT NULL) AND (@personID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Detiled object: [student].[InternationalProtection] with personID=' + CAST(@personID AS VARCHAR(12));

	/* *********************** CREATE newly added [student].[InternationalProtection] objects in collection ******************************** */ 
	INSERT INTO [student].[InternationalProtection]
	(
	   [DocNumber]
	  ,[DocDate]
	  ,[ValidFrom]
	  ,[ValidTo]
	  ,[PersonId]
	)
	OUTPUT INSERTED.Id INTO @internationalProtectionCreatedVar
	SELECT *, @personID
	FROM OPENJSON(@_json, '$.internationalProtectionCreated')
	WITH (
		 DocNumber nvarchar(50)		'$.docNumber'
	    ,DocDate date				'$.docDate'
	    ,ValidFrom date				'$.validFrom'
	    ,ValidTo date				'$.validTo'
	);
	SET @internationalProtectionCreatedString = (SELECT STRING_AGG(id, ',') FROM @internationalProtectionCreatedVar);
	PRINT 'INFO: [CurriculumStudent] >> CurriculumListA INSERTED with ids=' + @internationalProtectionCreatedString;

	/* *********************** UPDATE existing/changed [InternationalProtection] objects in collection ******************************** */ 
	UPDATE [student].[InternationalProtection]
	SET
	   [DocNumber] = JIP.DocNumber
	  ,[DocDate] = JIP.DocDate
	  ,[ValidFrom] = JIP.ValidFrom
	  ,[ValidTo] = JIP.ValidTo
	  ,[PersonId] = @personID
	OUTPUT INSERTED.Id INTO @internationalProtectionUpdatedVar
	FROM OPENJSON(@_json, '$.internationalProtectionUpdated')
	WITH (   
		 id int						'$.id'
		,DocNumber nvarchar(50)		'$.docNumber'
	    ,DocDate date				'$.docDate'
	    ,ValidFrom date				'$.validFrom'
	    ,ValidTo date				'$.validTo'
		) JIP
	WHERE
		[student].[InternationalProtection].Id = JIP.id;
	SET @internationalProtectionUpdatedString = (SELECT STRING_AGG(id, ',') FROM @internationalProtectionUpdatedVar);
	PRINT 'INFO: [InternationalProtection] UPDATED with InternationalProtectionIDs=' + @internationalProtectionUpdatedString;

	/* *********************** DELETE removed [InternationalProtection] objects in collection **************************** */
	SELECT @internationalProtectionDeletedString = (SELECT STRING_AGG(internationalProtectionDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.internationalProtectionDeleted')
		WITH (   
			internationalProtectionDeletedIDs int			'$.id'
		));

	DELETE FROM [student].[InternationalProtection]
		WHERE [student].[InternationalProtection].Id IN (			
			SELECT internationalProtectionDeletedIDs FROM OPENJSON(@_json, '$.internationalProtectionDeleted')
				WITH (   
					internationalProtectionDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [InternationalProtection] DELETED with InternationalProtectionIDs=' + CAST(@internationalProtectionDeletedString AS VARCHAR(12)); 


	SELECT @instID AS instID, @personID AS personID, 
		@internationalProtectionCreatedString AS internationalProtectionCreatedStringIDs, @internationalProtectionUpdatedString AS internationalProtectionUpdatedStringIDs, @internationalProtectionDeletedString AS internationalProtectionDeletedStringIDs; -- return back to the caller i.e. front-end
 
 END  /* End OperationType = 2 -> UPDATE Student data *************** */
/* ############################################################ */
ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', instID=' + ISNULL(CAST(@instID AS VARCHAR(12)),'') +
		', personID=' + ISNULL(CAST(@personID AS VARCHAR(12)),'');
	THROW 99000, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'InternationalProtection', NULL

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	INSERT INTO [inst_basic].[DB_Errors]
	VALUES
		(SUSER_SNAME(),
		ERROR_NUMBER(),
		ERROR_STATE(),
		ERROR_SEVERITY(),
		ERROR_LINE(),
		ERROR_PROCEDURE(),
		@OperationType,
		ERROR_MESSAGE(),
		GETDATE(),
		NULL
		);
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [studentUpdate] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
