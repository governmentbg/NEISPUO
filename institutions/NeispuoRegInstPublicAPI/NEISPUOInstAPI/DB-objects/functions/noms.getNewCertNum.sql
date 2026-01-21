USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getNewCertNum]    Script Date: 8.10.2021 г. 3:35:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER FUNCTION [noms].[getNewCertNum] 
(
)
RETURNS int
AS
BEGIN
	DECLARE @newCertNum int;
	SELECT @newCertNum = (SELECT MAX(CertificateNo)+1 FROM reginst_basic.RICertificate);
	RETURN (@newCertNum);
END;
GO

