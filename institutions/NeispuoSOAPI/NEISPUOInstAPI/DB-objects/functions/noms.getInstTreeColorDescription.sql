USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getInstTreeColorDescription]    Script Date: 8.10.2021 г. 3:34:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER FUNCTION [noms].[getInstTreeColorDescription] 
(
	@isRIActive INT,
	@transformTypeID INT,
	@procedureStatus int,
	@maxProcedureID int,
	@procedureID int,
	@maxTransformTypeID INT
)
RETURNS varchar(255)
AS
BEGIN
	DECLARE @instTreeColorDescription varchar(255);
	
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=1) AND (@transformTypeID IN (4))) SET @instTreeColorDescription = 'Активна институция (временно не провежда учебен процес)' ELSE
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=1)) SET @instTreeColorDescription = 'Активна институция (няма отворени процедури)' ELSE
	IF ((@maxProcedureID=@procedureID) AND (@isRIActive=0)) SET @instTreeColorDescription = 'Закрита институция' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=1) SET @instTreeColorDescription = 'Активна институция (нова открита процедура)' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=2) SET @instTreeColorDescription = 'Активна институция (В ход, очаква допълнително решение от МОН/ община)' ELSE
	IF (@maxProcedureID<>@procedureID) AND (@procedureStatus=3) SET @instTreeColorDescription = 'Активна институция (В процес на обжалване от институцията)' ELSE

	SET @instTreeColorDescription = ''

	RETURN (@instTreeColorDescription);
END;
GO

