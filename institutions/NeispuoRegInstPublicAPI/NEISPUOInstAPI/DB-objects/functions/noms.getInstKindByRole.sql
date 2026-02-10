USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getInstKindByRole]    Script Date: 8.10.2021 г. 3:32:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER FUNCTION [noms].[getInstKindByRole] 
(
	@instKindID INT
)
RETURNS int
AS
BEGIN
	DECLARE @roleID int;

	
	IF (@instKindID IN (3, 4, 7, 11, 12, 13, 14)) SET @roleID=1 ELSE
	SET @roleID = 2

	RETURN (@roleID);
END;
GO

