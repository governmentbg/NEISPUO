USE [neispuo]
GO
/****** Object:  StoredProcedure [logs].[logEvent]    Script Date: 1.3.2022 г. 10:00:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [logs].[logEvent] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int,
	@ModuleId int,
	@ObjectName NVARCHAR(50),
	@ObjectId int,
	@SchoolYear int

AS
BEGIN TRY

/************** Setup initial params ******************/

DECLARE @action NVARCHAR(50);
IF (@OperationType = 1)
	SET @action = 'INSERT';
ELSE IF  (@OperationType = 2)
	SET @action = 'UPDATE';
ELSE IF  (@OperationType = 3)
	SET @action = 'DELETE';
ELSE
	SET @action = 'UNKNOWN';

DECLARE @instID int;
SELECT @instID = instID FROM OPENJSON(@_json)
WITH (instID int '$.instid')

DECLARE @personid int;
SELECT @personid = personID FROM OPENJSON(@_json)
WITH (personid int '$.personid')

DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (sysuserid int '$.sysuserid')

DECLARE @sysroleid int;
SELECT @sysroleid = sysroleid FROM OPENJSON(@_json)
WITH (sysroleid int '$.sysroleid')

DECLARE @username NVARCHAR(1000);
SELECT @username = username FROM OPENJSON(@_json)
WITH (username NVARCHAR(1000) '$.audit.username')

DECLARE @firstName NVARCHAR(1000);
SELECT @firstName = firstName FROM OPENJSON(@_json)
WITH (firstName NVARCHAR(1000) '$.audit.firstName')

DECLARE @middleName NVARCHAR(1000);
SELECT @middleName = middleName FROM OPENJSON(@_json)
WITH (middleName NVARCHAR(1000) '$.audit.middleName')

DECLARE @lastName NVARCHAR(1000);
SELECT @lastName = lastName FROM OPENJSON(@_json)
WITH (lastName NVARCHAR(1000) '$.audit.lastName')

DECLARE @sessionId NVARCHAR(1000);
SELECT @sessionId = sessionId FROM OPENJSON(@_json)
WITH (sessionId NVARCHAR(1000) '$.audit.sessionId')

DECLARE @remoteIpAddress NVARCHAR(50);
SELECT @remoteIpAddress = remoteIpAddress FROM OPENJSON(@_json, '$.audit')
WITH (remoteIpAddress NVARCHAR(50) '$.remoteIpAddress')
-- TEMPORARY HACK
IF (@remoteIpAddress IS NULL) SET @remoteIpAddress = '0.0.0.0';

DECLARE @userAgent NVARCHAR(MAX);
SELECT @userAgent = userAgent FROM OPENJSON(@_json, '$.audit')
WITH (userAgent NVARCHAR(MAX) '$.userAgent')
-- TEMPORARY HACK
IF (@userAgent IS NULL) SET @userAgent = 'N/A';

INSERT INTO [logs].[Audit]
           ([AuditCorrelationId]
           ,[AuditModuleId]
           ,[SysUserId]
           ,[SysRoleId]
           ,[Username]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[LoginSessionId]
           ,[RemoteIpAddress]
           ,[UserAgent]
           ,[DateUtc]
           ,[SchoolYear]
           ,[InstId]
           ,[PersonId]
           ,[ObjectName]
           ,[ObjectId]
           ,[Action]
           ,[Data])
     VALUES
           (null
           ,@ModuleId
           ,@sysuserid
           ,@sysroleid
           ,@username
           ,@firstName
           ,@middleName
           ,@lastName
           ,@sessionId
           ,@remoteIpAddress
           ,@userAgent
           ,(SELECT CURRENT_TIMESTAMP)
           ,@SchoolYear
           ,@instID
           ,@personid
           ,@ObjectName
           ,@ObjectId
           ,@action
           ,@_json);

END TRY

BEGIN CATCH

	EXEC [logs].logError @_json, @OperationType, 103, 'Audit', NULL, NULL;
 
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [logEvent] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */
