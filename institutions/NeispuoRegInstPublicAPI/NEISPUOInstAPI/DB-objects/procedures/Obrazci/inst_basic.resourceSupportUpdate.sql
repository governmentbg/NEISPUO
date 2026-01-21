USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[resourceSupportUpdate]    Script Date: 8/1/2022 12:46:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[resourceSupportUpdate] 
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
);
-- ---------- define context param: institutionID --------------
DECLARE @schoolYear smallint;
SELECT @schoolYear = schoolYear FROM OPENJSON(@_json)
WITH (   
		schoolYear smallint			'$.schoolYear'
);
-- ---------- define context param: personid --------------
DECLARE @personid int;
SELECT @personid = personid FROM OPENJSON(@_json)
WITH (   
		personid int				'$.personid'
);

-- ---------- define context param: resourceSupportID --------------
DECLARE @resourceSupportID int;
SELECT @resourceSupportID = resourceSupportID FROM OPENJSON(@_json)
WITH (   
		resourceSupportID int		'$.resourceSupportID'
);

-- ---------- define Detail object 1: [ResourceSupportSpecialist] ID and string params ---
DECLARE @resourceSupportSpecialistCreatedVar TABLE  
(  
	id INT
);
DECLARE @resourceSupportSpecialistCreatedString NVARCHAR(4000);

DECLARE @resourceSupportSpecialistUpdatedVar TABLE  
(  
	id INT
);
DECLARE @resourceSupportSpecialistUpdatedString NVARCHAR(4000);

DECLARE @resourceSupportSpecialistDeletedString NVARCHAR(4000);

	--if the school used external data provider --- then it cannot write to the database
	DECLARE @isExtDataProvider bit;
	SELECT @isExtDataProvider = IIF(COUNT(SOExtProviderID)>0,1,0)
	FROM [core].[InstitutionConfData] 
	WHERE InstitutionID = @instID 
		AND SchoolYear = @schoolYear

	IF @isExtDataProvider = 1
	BEGIN	
		DECLARE @operationResultType int, @messageCode int;
		SET @operationResultType = 2;
		SET @messageCode = 1023;
		SELECT @operationResultType AS OperationResultType, @messageCode AS MessageCode, @instID AS instID;

		COMMIT TRANSACTION
		RETURN
	END


PRINT 'INFO: PROCEDURE resourceSupportUpdate started! Initial data gathered. InstitutionID=' + CAST(@instID AS VARCHAR(12)) + ' personid=' + CAST(@personid AS VARCHAR(12)) + ' resourceSupportID=' + CAST(@resourceSupportID AS VARCHAR(12));

/* ############################## Start OperationType = 2 -> UPDATE [ResourceSupportSpecialist] ############################## */
IF ((@OperationType = 2) AND (@personid IS NOT NULL) AND (@resourceSupportID IS NOT NULL))
BEGIN

/* ############################## Start OperationType = 2 -> UPDATE ResourceSupportSpecialist ############################## */
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Detail object: [ResourceSupportSpecialist] with resourceSupportID=' + CAST(@resourceSupportID AS VARCHAR(12));

		/* *********************** START INSERT INTO [ResourceSupportSpecialist] ********************************** */ 
	INSERT INTO [student].[ResourceSupportSpecialist]
	(
		[ResourceSupportId],
		[Name],
		[OrganizationType],
		[OrganizationName],
		[SpecialistType],
		[ResourceSupportSpecialistTypeId],
		[WorkPlaceID]
	)
	OUTPUT INSERTED.Id INTO @resourceSupportSpecialistCreatedVar
	SELECT @resourceSupportID, *
	FROM OPENJSON(@_json, '$.resourceSupportSpecialistCreated')
	WITH (
		 Name nvarchar(200)					 '$.name'
		,OrganizationType nvarchar(200)		 '$.organizationType'
		,OrganizationName nvarchar(200)		 '$.organizationName'
		,SpecialistType nvarchar(200)		 '$.specialistType'
		,ResourceSupportSpecialistTypeId int '$.resourceSupportSpecialistTypeId'
		,WorkPlaceID int					 '$.workPlaceID'
	);
	SET @resourceSupportSpecialistCreatedString = (SELECT STRING_AGG(id, ',') FROM @resourceSupportSpecialistCreatedVar);
	PRINT 'INFO: [ResourceSupportSpecialist] INSERTED with ids=' + @resourceSupportSpecialistCreatedString;
	/* *********************** END INSERT INTO [ResourceSupportSpecialist] ************************************ */
	
	/* *********************** UPDATE existing/changed [ResourceSupportSpecialist] objects in collection ******************************** */ 
	UPDATE [student].[ResourceSupportSpecialist]
	SET
		[Name]=RS.Name,
		[OrganizationType]=RS.OrganizationType,
		[OrganizationName]=RS.OrganizationName,
		[SpecialistType]=RS.SpecialistType,
		[ResourceSupportSpecialistTypeId]=RS.ResourceSupportSpecialistTypeId,
		[WorkPlaceID]=RS.WorkPlaceID
	
	OUTPUT INSERTED.Id INTO @resourceSupportSpecialistUpdatedVar
	FROM OPENJSON(@_json, '$.resourceSupportSpecialistUpdated')
	WITH (
		id	int								 '$.id'
		,Name nvarchar(200)			         '$.name'
		,OrganizationType nvarchar(200)		 '$.organizationType'
		,OrganizationName nvarchar(200)		 '$.organizationName'
		,SpecialistType nvarchar(200)		 '$.specialistType'
		,ResourceSupportSpecialistTypeId int '$.resourceSupportSpecialistTypeId'
		,WorkPlaceID int					 '$.workPlaceID'
		) RS
	WHERE
		[student].[ResourceSupportSpecialist].Id = RS.Id;
	SET @resourceSupportSpecialistUpdatedString = (SELECT STRING_AGG(id, ',') FROM @resourceSupportSpecialistUpdatedVar);
	PRINT 'INFO: [student].[ResourceSupportSpecialist] UPDATED with Ids=' + @resourceSupportSpecialistUpdatedString;

	
	/* *********************** DELETE removed [InstitutionInnovation] objects in collection **************************** */
	SELECT @resourceSupportSpecialistDeletedString = (SELECT STRING_AGG(resourceSupportSpecialistDeletedIDs, ',')  
		FROM OPENJSON(@_json, '$.resourceSupportSpecialistDeleted')
		WITH (   
			resourceSupportSpecialistDeletedIDs int			'$.id'
		));

	DELETE FROM [student].[ResourceSupportSpecialist]
		WHERE [student].[ResourceSupportSpecialist].Id IN (			
			SELECT resourceSupportSpecialistDeletedIDs FROM OPENJSON(@_json, '$.resourceSupportSpecialistDeleted')
				WITH (   
					resourceSupportSpecialistDeletedIDs int			'$.id'
				)
			);
	PRINT 'INFO: [student].[ResourceSupportSpecialist] DELETED with Id=' + CAST(@resourceSupportSpecialistDeletedString AS VARCHAR(12)); 


	SELECT @instID AS instID, @personid AS personid, @resourceSupportID AS resourceSupportID, 
	@resourceSupportSpecialistCreatedString AS resourceSupportSpecialisCreatedIDs,
	@resourceSupportSpecialistUpdatedString AS resourceSupportSpecialistUpdatedIDs,
	@resourceSupportSpecialistDeletedString AS rsourceSupportSpecialistDeletedIDs; -- return back to the caller i.e. front-end


END  /* End OperationType = 2 -> UPDATE resourceSupportSpecialist *************** */

ELSE
	THROW 99102, '[ResourceSupport] Illegal CONTEXT or MASTER Obejects params! Please contact DEV/support team.', 1;

EXEC [logs].logEvent @_json, @OperationType, 103, 'ResourceSupport', @resourceSupportID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'ResourceSupport', @resourceSupportID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [resourceSupportUpdate] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
