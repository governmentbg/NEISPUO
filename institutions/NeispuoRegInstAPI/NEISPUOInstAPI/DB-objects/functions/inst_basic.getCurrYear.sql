USE [neispuo]
GO

/****** Object:  UserDefinedFunction [inst_basic].[getCurrYear]    Script Date: 8.10.2021 г. 3:30:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [inst_basic].[getCurrYear]
(
	-- Add the parameters for the function here

)
RETURNS int
AS
BEGIN
	DECLARE @Year int;

	IF MONTH(GETDATE()) < 9
	BEGIN
	SET @Year = YEAR(GETDATE()) - 1
	END 
	ELSE
	BEGIN
	SET @Year = YEAR(GETDATE())
	END
	RETURN @Year
END
GO

