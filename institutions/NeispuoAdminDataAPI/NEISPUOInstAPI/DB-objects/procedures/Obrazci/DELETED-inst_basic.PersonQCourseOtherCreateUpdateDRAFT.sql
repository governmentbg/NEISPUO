USE [neispuo]
GO

/****** Object:  StoredProcedure [inst_basic].[PersonQCourseOtherCreateUpdateDRAFT]    Script Date: 6.10.2021 г. 10:14:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [inst_basic].[PersonQCourseOtherCreateUpdateDRAFT] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS
BEGIN

/************** Setup initial params ******************/
-- ---------- define context param: personid --------------
DECLARE @personid int;
SELECT @personid = personID FROM OPENJSON(@_json)
WITH (   
		personid int			'$.personid'
)

-- ---------- define Master object: [InstitutionPublicCouncil] ID param --------------
DECLARE @personQCourseID int;
SELECT @personQCourseID = personQCourseID FROM OPENJSON(@_json)
WITH (   
			personQCourseID int			'$.personQCourseID'
)


PRINT 'INFO: PROCEDURE PersonQCourseCreateUpdateDRAFT started! Initial data gathered. PersonID=' + CAST(@personid AS VARCHAR(12));

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
				,[DocumentType] 
			)
	SELECT @personid,1, *
	FROM OPENJSON(@_json)
	WITH (   

			qCourseYearOther int
			,qCourseTypeIDOther int
			,qCourseTypeNotesOther nvarchar(255)
			,qCourseBudgetSourceTypeIDOther int
			,internalCoursePriceOther float
			,universityOther int
			,universityNotesOther nvarchar(1024)
			,qCourseTopicOther nvarchar(255)
			,qCourseDurationTypeIDOther int
			,documentTypeOther nvarchar(255)

		); 

	-- GET INSERTED @PersonQCourse (autoincrement)
	SELECT @personQCourseID = SCOPE_IDENTITY();
	PRINT 'INFO: [PersonQCourse] created with id=' + CAST(@personQCourseID AS VARCHAR(12));
		
END  /* End of OperationType = 1 -> CREATE NEW *************** */

/* ############################## Start OperationType = 2 -> UPDATE Institution publicCouncil ############################## */
IF ((@OperationType = 2) AND (@personid IS NOT NULL) AND (@personQCourseID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 2 -> Will UPDATE Master object: [PersonQCourse] with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));
	UPDATE top (1) [inst_basic].[PersonQCourse]
	SET 
				[QCourseYear]=JPCS.qCourseYearOther  
				,[QCourseTypeID]=JPCS.qCourseTypeIDOther 
				,[QCourseTypeNotes]=JPCS.qCourseTypeNotesOther
				,[QCourseBudgetSourceTypeID]=JPCS.qCourseBudgetSourceTypeIDOther
				,[InternalCoursePrice]=JPCS.internalCoursePriceOther 
				,[UniversityID]=JPCS.universityOther 
				,[UniversityNotes]=JPCS.universityNotesOther
				,[QCourseTopic]=JPCS.qCourseTopicOther 
				,[QCourseDurationTypeID]=JPCS.qCourseDurationTypeIDOther 
				,[DocumentType]=JPCS.documentTypeOther
				
	FROM OPENJSON(@_json)
	WITH (   
			qCourseYearOther int
			,qCourseTypeIDOther int
			,qCourseTypeNotesOther nvarchar(255)
			,qCourseBudgetSourceTypeIDOther int
			,internalCoursePriceOther float
			,universityOther int
			,universityNotesOther nvarchar(1024)
			,qCourseTopicOther nvarchar(255)
			,qCourseDurationTypeIDOther int
			,documentTypeOther nvarchar(255)

		) JPCS				 
	WHERE
		[inst_basic].[PersonQCourse].[PersonQCourseID] = @personQCourseID;
	PRINT 'INFO: [PersonQCourse] UPDATED successfully!';


END  /* End OperationType = 2 -> UPDATE *************** */

/* ############################## Start OperationType = 3 -> DELETE PersonQCourse ############################## */
ELSE IF ((@OperationType = 3) AND (@personid IS NOT NULL) AND (@personQCourseID IS NOT NULL))
BEGIN
	PRINT 'INFO: OperationType = 3 -> DELETE [PersonQCourse] with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));

		DELETE FROM [inst_basic].[PersonQCourse]
			WHERE [inst_basic].[PersonQCourse].[PersonQCourseID] = @personQCourseID;
		PRINT 'INFO: [PersonQCourse] DELETED with PersonQCourseID=' + CAST(@personQCourseID AS VARCHAR(12));


	SELECT @personQCourseID AS id;
END /* End OperationType = 3 -> DELETE *************** */

END /* ************ END OF PROCEDURE ************ */
GO


