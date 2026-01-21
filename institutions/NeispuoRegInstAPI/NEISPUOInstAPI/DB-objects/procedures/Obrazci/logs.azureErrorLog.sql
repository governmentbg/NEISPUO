USE [neispuo]
GO
/****** Object:  StoredProcedure [logs].[azureErrorLog]    Script Date: 1.3.2022 г. 9:53:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [logs].[azureErrorLog] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int

AS

BEGIN TRY
BEGIN TRANSACTION

/************** Setup initial params ******************/
DECLARE @instID int;
SELECT @instID = instID FROM OPENJSON(@_json)
WITH (instID int '$.instid')

DECLARE @sysuserid int;
SELECT @sysuserid = sysuserid FROM OPENJSON(@_json)
WITH (sysuserid int '$.sysuserid')

DECLARE @error_number int;
SELECT @error_number = error_num FROM OPENJSON(@_json)
WITH (error_num int '$.error_number')

DECLARE @error_message VARCHAR(1000);
SELECT @error_message = error_msg FROM OPENJSON(@_json)
WITH (error_msg VARCHAR(1000) '$.error_message')

DECLARE @error_procedure VARCHAR(100);
SELECT @error_procedure = error_proc FROM OPENJSON(@_json)
WITH (error_proc VARCHAR(100) '$.error_procedure')

DECLARE @operationTypeAzureCall INT;
SELECT @operationTypeAzureCall = operationTypeAzureCall FROM OPENJSON(@_json)
WITH (operationTypeAzureCall int '$.operationType')

DECLARE @payload VARCHAR(MAX);
SELECT @payload = payload FROM OPENJSON(@_json)
WITH (payload VARCHAR(MAX) '$.payload')


/************** Insert data in DB ******************/

INSERT INTO [logs].[DB_Errors]
           ([ErrorDateTime]
           ,[UserName]
           ,[ModuleId]
           ,[ErrorProcedure]
           ,[OperationType]
           ,[ForceOperation]
           ,[ErrorMessage]
           ,[SchoolYear]
           ,[InstId]
           ,[PersonId]
           ,[ObjectName]
           ,[ObjectId]
           ,[SysUserId]
           ,[SysRoleId]
           ,[LoginSessionId]
           ,[RemoteIpAddress]
           ,[UserAgent]
           ,[ErrorNumber]
           ,[ErrorState]
           ,[ErrorSeverity]
           ,[ErrorLine]
           ,[Data])
     VALUES
           (GETDATE(),
            'AzureCall', -- [UserName]
            103, -- [ModuleId]
            @error_procedure,
            @operationTypeAzureCall,
            NULL, -- [ForceOperation]
            @error_message,
            NULL, -- [SchoolYear]
            @instID,
            NULL, -- [PersonId]
            'AzureCall',
            NULL, -- [ObjectId]
            @sysuserid,
            NULL, -- [SysRoleId]
            NULL, -- [LoginSessionId]
            NULL, -- [RemoteIpAddress]
            NULL, -- [UserAgent]
            @error_number,
            NULL, -- [ErrorState]
            NULL, -- [ErrorSeverity]
            NULL, -- [ErrorLine]
            @payload)
			
COMMIT TRANSACTION
END TRY

BEGIN CATCH

	ROLLBACK TRANSACTION
	PRINT 'INFO: TRANSACTION has been ROLLBACKED!';

	EXEC [logs].logError @_json, @OperationType, 103, 'AzureCall', NULL, NULL;
  
	DECLARE @ErrorMessage NVARCHAR(4000);  
	DECLARE @ErrorSeverity INT;  
	DECLARE @ErrorState INT;  
  
	SELECT   
		@ErrorMessage = ERROR_MESSAGE(),  
		@ErrorSeverity = ERROR_SEVERITY(),  
		@ErrorState = ERROR_STATE(); 

	PRINT 'INFO: [azureErrorLog] failed with error! XACT_STATE()=' + CAST(XACT_STATE() AS VARCHAR(12));

	RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
/* ************ END OF PROCEDURE ************ */