ELSE IF ((@OperationType = 2) OR (@OperationType = 3))   -- 2/3 - UPDATE / COMPLETE
BEGIN
	PRINT 'INFO: OperationType: 2/3 - UPDATE / COMPLETE.'
	DECLARE @riProcIDupdate int;
	SELECT @riProcIDupdate = riProcedureID FROM OPENJSON(@_json)
	WITH (   
			   riProcedureID int			'$.riProcedureID'
	)
	PRINT 'DEBUG: @riProcIDupdate = ' + CAST(@riProcIDupdate AS VARCHAR(12));

	DECLARE @riInstIDupdate int;
	SELECT @riInstIDupdate = riInstitutionID FROM OPENJSON(@_json)
	WITH (   
			   riInstitutionID int			'$.riInstitutionID'
	)
	DECLARE @instIDupdate int;
	SELECT @instIDupdate = institutionID FROM OPENJSON(@_json)
	WITH (   
			   institutionID int			'$.institutionID'
	)

	DECLARE @riProcedureIDForDeactivate int;
	DECLARE @newProcStatus bit;
	IF (@OperationType = 3)  -- IF complete
		BEGIN
			SELECT @riProcedureIDForDeactivate = (SELECT RIProcedureID from reginst_basic.RIProcedure where InstitutionID = @instIDupdate and IsActive = 1);
			SELECT @newProcStatus = 1;
		END
	ELSE 
		SELECT @newProcStatus = 0;

	IF (@riProcIDupdate > 0 AND @riInstIDupdate > 0)
	BEGIN 
		UPDATE top (1) [reginst_basic].[RIProcedure]
		SET
			[ProcedureTypeID] = JP.procedureTypeID,
			[ProcedureDate] = JP.procedureDate,
			[YearDue] = JP.procedureYearDue,
			[StatusTypeID] = JP.procedureStatusTypeID,
			[TransformTypeID] = JP.procedureTransformTypeID,
			[TransformDetails] = JP.procedureTransformDetails,
			[IsActive] = @newProcStatus
		FROM OPENJSON(@_json)
		WITH (   
			procedureTypeID int,  
			procedureDate date,
			procedureYearDue int,
			procedureStatusTypeID int,
			procedureTransformTypeID int,
			procedureTransformDetails nvarchar(4000)) JP
		WHERE
			[reginst_basic].[RIProcedure].RIprocedureID = @riProcIDupdate;

		/* Deactive old procedure 
		IF ((@OperationType = 3) AND (@riProcedureIDForDeactivate <> null))
			UPDATE top (1) [reginst_basic].[RIProcedure]
			SET [IsActive] = 0
			WHERE (RIProcedureID = @riProcedureIDForDeactivate) AND (IsActive=1); */

		UPDATE top (1) [reginst_basic].[RIInstitution]
		SET
			[Bulstat] = JI.bulstat,
			[Name] = JI.name,
			[Abbreviation] = JI.abbreviation,
			[BaseSchoolTypeID] = JI.baseSchoolTypeID,
			[DetailedSchoolTypeID] = JI.detailedSchoolTypeID,
			[FinancialSchoolTypeID] = JI.financialSchoolTypeID,
			[BudgetingSchoolTypeID] = JI.budgetingSchoolTypeID,
			[TRCountryID] = JI.settlementCountry,
			[TRTownID] = JI.settlementTown,
			[TRLocalAreaID] = JI.settlementLocalArea,
			[TRAddress] = JI.settlementAddress,
			[TRPostCode] = JI.settlementPostCode,
			[ReligInstDetails] = JI.religInstDetails,
			[HeadFirstName] = JI.headFirstName,
			[HeadMiddleName] = JI.headMiddleName,
			[HeadLastName] = JI.headLastName,
			[IsNational] = JI.isNational,
			[PersonnelCount] = JI.personnelCount,
			[AuthProgram] = JI.authProgram,
			[IsDataDue] = JI.isDataDue
		FROM OPENJSON(@_json)
		WITH (   
			bulstat nvarchar(13),
			name nvarchar(1024),
			abbreviation nvarchar(255),
			baseSchoolTypeID int,
			detailedSchoolTypeID int,
			financialSchoolTypeID int,
			budgetingSchoolTypeID int,
			settlementCountry int,
			settlementTown int,
			settlementLocalArea int,
			settlementAddress nvarchar(255),
			settlementPostCode int,
			religInstDetails nvarchar(max),
			headFirstName nvarchar(255),
			headMiddleName nvarchar(255),
			headLastName nvarchar(255),
			isNational bit,
			personnelCount int,
			authProgram nvarchar(max),
			isDataDue bit) JI
		 WHERE
			[reginst_basic].[RIInstitution].RIinstitutionID = @riInstIDupdate;
	END
	
	SELECT @riProcIDupdate AS riProcIDupdate, @riInstIDupdate AS riInstIDupdate, @instIDupdate AS instIDupdate, @riProcedureIDForDeactivate AS riProcedureIDForDeactivate, @newProcStatus AS newProcStatus;

END
