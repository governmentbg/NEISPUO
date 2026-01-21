use neispuo;
GO
CREATE SCHEMA azure_temp;
-- neispuo.azure_temp.Classes definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Classes;

CREATE TABLE neispuo.azure_temp.Classes (
	RowID int IDENTITY(1,1) NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Title varchar(255) NULL,
	ClassCode varchar(255) NULL,
	OrgID varchar(255) NULL,
	TermID int NULL,
	TermName varchar(255) NULL,
	TermStartDate datetime2(7) NULL,
	TermEndDate datetime2(7) NULL,
	InProcessing int DEFAULT 0 NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	ClassID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	CONSTRAINT PK__Classes__CB1927A07908B1CE PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Classes_History definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Classes_History;

CREATE TABLE neispuo.azure_temp.Classes_History (
	RowID int NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Title varchar(255) NULL,
	ClassCode varchar(255) NULL,
	OrgID varchar(255) NULL,
	TermID int NULL,
	TermName varchar(255) NULL,
	TermStartDate datetime2(7) NULL,
	TermEndDate datetime2(7) NULL,
	InProcessing int DEFAULT 0 NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	ClassID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	CONSTRAINT PK__Classes___CB1927A058D07992 PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Enrollments definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Enrollments;

CREATE TABLE neispuo.azure_temp.Enrollments (
	RowID int IDENTITY(1,1) NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	UserID varchar(100) NOT NULL,
	ClassID varchar(100) NOT NULL,
	OrganizationID varchar(100) NOT NULL,
	InProcessing int DEFAULT 0 NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	Status int DEFAULT 0 NOT NULL,
	CONSTRAINT PK__Enrollme__FD27BBEA3A5E1624 PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Enrollments_History definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Enrollments_History;

CREATE TABLE neispuo.azure_temp.Enrollments_History (
	RowID int NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	UserID varchar(255) NOT NULL,
	ClassID varchar(255) NOT NULL,
	OrganizationID varchar(255) NOT NULL,
	InProcessing int DEFAULT 0 NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	Status int DEFAULT 0 NOT NULL,
	CONSTRAINT PK__Enrollme__FD27BBEAD599348E PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Organizations definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Organizations;

CREATE TABLE neispuo.azure_temp.Organizations (
	RowID int IDENTITY(1,1) NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Name varchar(255) NULL,
	Description varchar(255) NULL,
	PrincipalId varchar(255) NULL,
	PrincipalName varchar(255) NULL,
	PrincipalEmail varchar(255) NULL,
	HighestGrade int NULL,
	LowestGrade int NULL,
	Phone varchar(255) NULL,
	City varchar(255) NULL,
	Area varchar(255) NULL,
	Country varchar(255) NULL,
	PostalCode varchar(255) NULL,
	Street varchar(255) NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	OrganizationID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	InProcessing int DEFAULT 0 NULL,
	Username varchar(100) NULL,
	CONSTRAINT PK__Organiza__CADB0B724B8EB191 PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Organizations_History definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Organizations_History;

CREATE TABLE neispuo.azure_temp.Organizations_History (
	RowID int NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Name varchar(255) NULL,
	Description varchar(255) NULL,
	PrincipalId varchar(255) NULL,
	PrincipalName varchar(255) NULL,
	PrincipalEmail varchar(255) NULL,
	HighestGrade int NULL,
	LowestGrade int NULL,
	Phone varchar(255) NULL,
	City varchar(255) NULL,
	Area varchar(255) NULL,
	Country varchar(255) NULL,
	PostalCode varchar(255) NULL,
	Street varchar(255) NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	OrganizationID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	InProcessing int DEFAULT 0 NULL,
	Username varchar(100) NULL,
	CONSTRAINT PK__Organiza__CADB0B72B04082B8 PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Users definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Users;

CREATE TABLE neispuo.azure_temp.Users (
	RowID int IDENTITY(1,1) NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Identifier varchar(255) NULL,
	FirstName varchar(255) NULL,
	MiddleName varchar(255) NULL,
	Surname varchar(255) NULL,
	Password varchar(255) NULL,
	Email varchar(255) NULL,
	Phone varchar(255) NULL,
	Grade varchar(255) NULL,
	SchoolId varchar(255) NULL,
	BirthDate datetime2(7) NULL,
	UserRole varchar(255) NULL,
	AccountEnabled int NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	UserID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	Username varchar(100) NULL,
	InProcessing int DEFAULT 0 NULL,
	CONSTRAINT PK__Users__1788CCAC998A9D4E PRIMARY KEY (RowID)
);


-- neispuo.azure_temp.Users_History definition

-- Drop table

-- DROP TABLE neispuo.azure_temp.Users_History;

CREATE TABLE neispuo.azure_temp.Users_History (
	RowID int NOT NULL,
	WorkflowType varchar(255) NOT NULL,
	Identifier varchar(255) NULL,
	FirstName varchar(255) NULL,
	MiddleName varchar(255) NULL,
	Surname varchar(255) NULL,
	Password varchar(255) NULL,
	Email varchar(255) NULL,
	Phone varchar(255) NULL,
	Grade varchar(255) NULL,
	SchoolId varchar(255) NULL,
	BirthDate varchar(255) NULL,
	UserRole varchar(255) NULL,
	AccountEnabled int NULL,
	ErrorMessage varchar(255) NULL,
	CreatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	UpdatedOn datetime2(7) DEFAULT getdate() NOT NULL,
	GUID varchar(255) NULL,
	RetryAttempts int DEFAULT 0 NULL,
	UserID varchar(100) NULL,
	Status int DEFAULT 0 NOT NULL,
	Username varchar(100) NULL,
	InProcessing int DEFAULT 0 NULL,
	CONSTRAINT PK__Users_Hi__1788CCACFDD4FDAD PRIMARY KEY (RowID)
);


CREATE PROCEDURE  [azure_temp].[ARCHIVE_CLASSES] AS
BEGIN
	INSERT
		INTO
		"azure_temp"."Classes_History"
	SELECT
		*
	FROM
		"azure_temp"."Classes";
	TRUNCATE TABLE "azure_temp"."Classes"
END;
GO

CREATE PROCEDURE  [azure_temp].[ARCHIVE_ENROLLMENTS] AS
BEGIN
	INSERT
		INTO
		"azure_temp"."Enrollments_History"
	SELECT
		*
	FROM
		"azure_temp"."Enrollments";
	TRUNCATE TABLE "azure_temp"."Enrollments"
END;
GO

CREATE PROCEDURE  [azure_temp].[ARCHIVE_ORGANIZATIONS] AS
BEGIN
	INSERT
		INTO
		"azure_temp"."Organizations_History"
	SELECT
		*
	FROM
		"azure_temp"."Organizations";
	TRUNCATE TABLE "azure_temp"."Organizations"
END;
GO

CREATE PROCEDURE  [azure_temp].[ARCHIVE_USERS] AS
BEGIN
	INSERT
		INTO
		"azure_temp"."Users_History"
	SELECT
		*
	FROM
		"azure_temp"."Users";
	TRUNCATE TABLE "azure_temp"."Users"
END;
GO

CREATE PROCEDURE azure_temp.CREATE_AZURE_INSTITUTION_USER  
(
	  @InstitutionID int,
	  @FirstName VARCHAR(100),
	  @MiddleName VARCHAR(100),
      @LastName VARCHAR(100),
      @UserName VARCHAR(100),
      @IsAzureUser int,
      @IsAzureSynced int,
	  @JobID int, 
	  @JobStatus int, 
	  @InProcessing int
)  
AS
BEGIN
	DECLARE @ResultValue int
        SET @ResultValue = 0;
		BEGIN TRY
	        BEGIN TRANSACTION
	        DECLARE @PersonID int;
	        DECLARE @tableThatHoldsInsertedPersonID TABLE(PersonID int);
	        DECLARE @SysUserID int;
	        DECLARE @tableThatHoldsInsertedSysUserID TABLE(SysUserID int);
                UPDATE
                	azure_temp.Organizations
	            SET
	                UpdatedOn = GETDATE(),
	                Status = @JobStatus,
	                InProcessing = @InProcessing
	            WHERE
	                RowID = @JobID
	            INSERT 
	                INTO
	                core.Person (
	                    firstName,
	                    middleName,
	                    lastName
	                )
	            OUTPUT inserted.PersonID into @tableThatHoldsInsertedPersonID
	            VALUES (
				  @FirstName,
				  @MiddleName,
			      @LastName
	            );        
	            SELECT @PersonID = PersonID from @tableThatHoldsInsertedPersonID     
	            INSERT INTO
	                core.SysUser (
	                    Username,
	                    PersonID,
	                    IsAzureUser,
	                    IsAzureSynced
	                )
	            OUTPUT inserted.SysUserID into @tableThatHoldsInsertedSysUserID
	            VALUES (
	                @UserName,
	                @PersonID,
	                @IsAzureUser,
	                @IsAzureSynced
	            );
	            SELECT @SysUserID = SysUserID from @tableThatHoldsInsertedSysUserID;
	           INSERT INTO
	                core.SysUserSysRole (
		                SysUserID,
						SysRoleID,
						InstitutionID
	                )
	            VALUES (
	                @SysUserID,
	                0,
					@InstitutionID
	            );
	            SET @ResultValue = 1;
	        COMMIT;
	    END TRY
	    BEGIN CATCH
	       ROLLBACK;
	       UPDATE
                	azure_temp.Organizations
	            SET
	                UpdatedOn = GETDATE(),
	                InProcessing = 0
	            WHERE
	                RowID = @JobID;
			RETURN @ResultValue; 
	    END CATCH
	RETURN @ResultValue; 
END;
GO
       
CREATE PROCEDURE azure_temp.CREATE_AZURE_USER  
(
      @PersonID int,
      @UserName VARCHAR(100),
      @IsAzureUser int,
      @IsAzureSynced int,
	  @JobID int,
	  @JobStatus int,
	  @InProcessing int
)  
AS
BEGIN
	DECLARE @ResultValue int
        SET @ResultValue = 0;
		BEGIN TRY
	        BEGIN TRANSACTION
	        	UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                Status = @JobStatus,
	                InProcessing = @InProcessing
	            WHERE
	                RowID = @JobID;
	            INSERT INTO
	                core.SysUser (
	                    Username,
	                    PersonID,
	                    IsAzureUser,
	                    IsAzureSynced
	                )
	            VALUES (
	                @UserName,
	                @PersonID,
	                @IsAzureUser,
	                @IsAzureSynced
	            );
	            SET @ResultValue = 1;
	        COMMIT;
	    END TRY
	    BEGIN CATCH
	       ROLLBACK;
	       UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                InProcessing = 0
	            WHERE
	                RowID = @JobID;
			RETURN @ResultValue; 
	    END CATCH
	RETURN @ResultValue; 
END;
GO
       
CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_STUDENT
(
      @PersonID int,
      @IsAzureSynced int,
	  @JobID int,
	  @JobStatus int,
	  @InProcessing int
)  
AS
BEGIN
		BEGIN TRY
	        BEGIN TRANSACTION
	        	UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                Status = @JobStatus,
	                InProcessing = @InProcessing
	            WHERE
	                RowID = @JobID;
	            UPDATE core.SysUser SET
	                    IsAzureSynced = @IsAzureSynced
	            WHERE
	                PersonID = CAST(@PersonID AS int);
	        COMMIT;
	    END TRY
	    BEGIN CATCH
	       ROLLBACK;
	       UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                InProcessing = 0
	            WHERE
	                RowID = @JobID;
	    END CATCH
END;
CREATE PROCEDURE azure_temp.UPDATE_AZURE_USER_TEACHER
(
      @PersonID int,
      @IsAzureSynced int,
	  @JobID int,
	  @JobStatus int,
	  @InProcessing int
)  
AS
BEGIN
		BEGIN TRY
	        BEGIN TRANSACTION
	        	UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                Status = @JobStatus,
	                InProcessing = @InProcessing
	            WHERE
	                RowID = @JobID;
	            UPDATE core.SysUser SET
	                    IsAzureSynced = @IsAzureSynced
	            WHERE
	                PersonID = CAST(@PersonID AS int);
	        COMMIT;
	    END TRY
	    BEGIN CATCH
	       ROLLBACK;
	       UPDATE
                	azure_temp.Users
	            SET
	                UpdatedOn = GETDATE(),
	                InProcessing = 0
	            WHERE
	                RowID = @JobID;
	    END CATCH
END;


GO
