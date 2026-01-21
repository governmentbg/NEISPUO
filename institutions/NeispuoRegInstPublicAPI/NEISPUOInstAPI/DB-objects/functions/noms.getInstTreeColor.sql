USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getInstTreeColor]    Script Date: 8.10.2021 г. 3:33:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER FUNCTION [noms].[getInstTreeColor] 
(
	@isRIActive INT,
	@transformTypeID INT,
	@procedureStatus int,
	@maxProcedureID int,
	@procedureID int,
	@maxTransformTypeID INT
)
RETURNS varchar(10)
AS
BEGIN
	DECLARE @instTreeColor varchar(10);
	
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=1) AND (@transformTypeID IN (4))) SET @instTreeColor = '#90EE90' ELSE
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=1)) SET @instTreeColor = '#008000' ELSE
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=0)) SET @instTreeColor = '#FF0000' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=1) SET @instTreeColor = '#808080' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=2) SET @instTreeColor = '#FFA500' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=3) SET @instTreeColor = '	#FFFF00' ELSE

	SET @instTreeColor = '#008000'

	RETURN (@instTreeColor);
END;
GO

