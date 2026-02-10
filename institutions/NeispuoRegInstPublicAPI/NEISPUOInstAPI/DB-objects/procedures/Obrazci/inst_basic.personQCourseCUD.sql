USE [neispuo]
GO
/****** Object:  StoredProcedure [inst_basic].[personQCourseCUD]    Script Date: 8/1/2022 12:54:52 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [inst_basic].[personQCourseCUD] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
-- ---------- define context param: personid --------------
DECLARE @personid int;
SELECT @personid = personID FROM OPENJSON(@_json)
WITH (   
		personid int			'$.personid'
)

-- ---------- define context param2: sysuserid --------------
DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (   
		sysuserid int			'$.sysuserid'
)

-- ---------- define Master object: [personQCourse] ID param --------------
DECLARE @personQCourseID int;
SELECT @personQCourseID = personQCourseID FROM OPENJSON(@_json)
WITH (   
			personQCourseID int			'$.personQCourseDataID'
)

-- ---------- define context param: instid --------------
DECLARE @instid int;
SELECT @instid = instid FROM OPENJSON(@_json)
WITH (   
		instid int			'$.instid'
)
-- ---------- define context param: schoolYear --------------
DECLARE @schoolYear int;
SELECT @schoolYear = inst_year.getSchoolYearByInstID(@instID);


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


PRINT 'INFO: PROCEDURE personQCourseCUD started! Initial data gathered. PersonID=' + CAST(@personid AS VARCHAR(12));

/* ************ OperationType = 1 -> CREATE NEW PersonQCourse *************** */
IF ((@OperationType = 1) AND (@personid IS NOT NULL) AND (@personQCourseID IS NULL))
BEGIN
	PRINT 'INFO: OperationType: 1 - CREATE NEW [PersonQCourse]...';
	INSERT INTO [inst_basic].[PersonQCourse]
           (
				[PersonID]
				,[QCourseActTypeID]
				,[QCourseYear]
				,[QCourseTypeID]
				,[QCourseTypeNotes]
				,[QCourseBudgetSourceTypeID]
				,[InternalCoursePrice]
				,[UniversityID] 
				,[UniversityNotes] 
				,[QCourseTopic] 
				,[QCourseDurationTypeID] 
				,[QCourseDurationCredits] 
				,[QCourseDurationHours]
				,[DocumentType] 
				,[SysUserID]
			)
	SELECT @personid,*, @sysuserid
	FROM OPENJSON(@_json)
	WITH (   
	 		 qCourseActTypeID int
			,qCourseYear int		
			,qCourseTypeID int
			,qCourseTypeNotes nvarchar(255)
			,qCourseBudgetSourceTypeID int
			,internalCoursePrice float
			,universityID int
			,universityNotes nvarchar(1024)
			,qCourseTopic nvarchar(255)
			,qCourseDurationTypeID int
			,qCourseDurationCredits int
			,qCourseDurationHours decimal(6,2)
			,documentType nvarchar(255)
		); 

	-- GET INSERTED @PersonQCourse (autoincrement)
	SELECT @personQCourseID = SCOPE_IDENTITY();
	PRINT 'INFO: [PersonQCourse] created with id=' + CAST(@personQCourseID AS VARCHAR(12));
		
END  /* End of OperationType = 1 -> CREATE NEW *************** */

/* ############################## Start OperationType = 2 -> UPDATE personQCourse ############################## */
ELSE IF ((@OperationType = 2) AND (@personid IS NOT NULL) AND (@personQCourseID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [PersonQCourse] with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[PersonQCourse]
	SET 
			 [QCourseYear]=JPCS.qCourseYear 
			,[QCourseTypeID]=JPCS.qCourseTypeID 
			,[QCourseTypeNotes]=JPCS.qCourseTypeNotes
			,[QCourseBudgetSourceTypeID]=JPCS.qCourseBudgetSourceTypeID
			,[InternalCoursePrice]=JPCS.internalCoursePrice
			,[UniversityID]=JPCS.universityID 
			,[UniversityNotes]=JPCS.universityNotes
			,[QCourseTopic]=JPCS.qCourseTopic 
			,[QCourseDurationTypeID]=JPCS.qCourseDurationTypeID 
			,[QCourseDurationCredits]=JPCS.qCourseDurationCredits 
			,[QCourseDurationHours]=JPCS.qCourseDurationHours
			,[DocumentType]=JPCS.documentType	
			,[SysUserID]=@sysuserid
	FROM OPENJSON(@_json)
	WITH (   
			 qCourseYear int
			,qCourseTypeID int
			,qCourseTypeNotes nvarchar(255)
			,qCourseBudgetSourceTypeID int
			,internalCoursePrice float
			,universityID int
			,universityNotes nvarchar(1024)
			,qCourseTopic nvarchar(255)
			,qCourseDurationTypeID int
			,qCourseDurationCredits int
			,qCourseDurationHours decimal(6,2)
			,documentType nvarchar(255)
		) JPCS				 
	WHERE
		[inst_basic].[PersonQCourse].[PersonQCourseID] = @personQCourseID;
	PRINT 'INFO: [PersonQCourse] UPDATED successfully!';

END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 3 -> DELETE PersonQCourse ############################## */
ELSE IF ((@OperationType = 3) AND (@personQCourseID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [PersonQCourse] with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));

		DELETE FROM [inst_basic].[PersonQCourse]
			WHERE [inst_basic].[PersonQCourse].[PersonQCourseID] = @personQCourseID;
		PRINT 'INFO: [PersonQCourse] DELETED with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));
		
	SELECT @personQCourseID AS id;
END /* End OperationType = 3 -> DELETE *************** */

ELSE
BEGIN
	DECLARE @ERR_MSG NVARCHAR(4000);
	SELECT @ERR_MSG = 'Illegal CONTEXT or MASTER Obejects params! Sent params are: ' + 
		'OperationType=' + ISNULL(CAST(@OperationType AS VARCHAR(12)),'') + 
		', personid=' + ISNULL(CAST(@personid AS VARCHAR(12)),'') +
		', personQCourseID=' + ISNULL(CAST(@personQCourseID AS VARCHAR(12)),'');
	THROW 99006, @ERR_MSG, 1;
END

EXEC [logs].logEvent @_json, @OperationType, 103, 'PersonQCourse', @personQCourseID, NULL;

COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'PersonQCourse', @personQCourseID, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [personQCourseCUD] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
