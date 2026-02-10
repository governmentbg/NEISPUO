USE [neispuo]
GO
/****** Object:  StoredProcedure [reginst_basic].[documentSave]    Script Date: 8.5.2022 г. 13:59:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [reginst_basic].[documentSave] 
	-- Add the parameters for the stored procedure here
	@_json NVARCHAR(MAX),
	@OperationType int
AS
BEGIN

IF (@OperationType < 1)
BEGIN
	PRINT 'ERROR: Invalid OperationType.'
	RETURN(-1)
END

DECLARE @riProcID int;
SELECT @riProcID = riProcedureID FROM OPENJSON(@_json)
WITH (   
			riProcedureID int			'$.procID'
)
IF (@riProcID < 1)
BEGIN
	PRINT 'ERROR: You must specify valid procedure ID.'
	RETURN(-2)
END

/* ************************ insert new Certificate data ************************ */
IF (@OperationType = 1)  -- 1 - insert new Certificate data (prepare for generate document)
BEGIN
	INSERT INTO reginst_basic.RICertificate
			   ([RIprocedureID]
			   ,[CertificateNo]
			   ,[CertificateDate])
		 VALUES
			   (@riProcID,
				noms.getNewCertNum(),
			    GETDATE());
END

/* ************************ save GenerateCertificateFile ID ************************ */
IF (@OperationType = 5)  -- 5 - save GenerateCertificateFile ID
BEGIN
	/* PRINT 'INFO: save GenerateCertificateFile will be executed with procID=' + @riProcID + ' and blobID=' +  @docDocumentID */
	DECLARE @genDocumentID int;
	SELECT @genDocumentID = docBlobID FROM OPENJSON(@_json)
	WITH (   
				docBlobID int			'$.blobID'
	)
	IF (@genDocumentID < 1)
	BEGIN
		PRINT 'ERROR: You must specify valid document (Blob) ID.'
		RETURN(-3)
	END

	UPDATE top (1) [reginst_basic].[RICertificate]
	SET
		[GenerateCertificateFile] = @genDocumentID
	WHERE
		[reginst_basic].[RICertificate].RIprocedureID = @riProcID;
END

/* ************************ save CertificateFileScaned ID ************************ */
ELSE IF (@OperationType = 6)  -- 6 - save CertificateFileScaned ID
BEGIN
	/* PRINT 'INFO: save CertificateFileScaned will be executed with procID=' + @riProcID + ' and blobID=' +  @docDocumentID */
	DECLARE @scanDocumentID int;
	SELECT @scanDocumentID = docBlobID FROM OPENJSON(@_json)
	WITH (   
				docBlobID int			'$.blobID'
	)
	IF (@scanDocumentID < 1)
	BEGIN
		PRINT 'ERROR: You must specify valid document (Blob) ID.'
		RETURN(-4)
	END

	UPDATE top (1) [reginst_basic].[RICertificate]
	SET
		[CertificateFileScaned] = @scanDocumentID
	WHERE
		[reginst_basic].[RICertificate].RIprocedureID = @riProcID;
END

END
