USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getInstType]    Script Date: 8.10.2021 г. 3:34:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER FUNCTION [noms].[getInstType] 
(
    @baseSchoolTypeID INT
)
RETURNS INT
AS
BEGIN
	DECLARE @instType INT;

	IF (@baseSchoolTypeID IN (9, 15)) SET @instType = 2 ELSE  -- "ДЕТСКИ ГРАДИНИ" (isValid = 0, 1)
	IF (@baseSchoolTypeID IN (16, 8)) SET @instType = 3 ELSE -- "ЦПЛР" 
	IF (@baseSchoolTypeID IN (13)) SET @instType = 4 ELSE  -- ЦСОП (център за специална образователна подкрепа)
	IF (@baseSchoolTypeID IN (17,4)) SET @instType = 5 ELSE -- СОЗ (специализирано обслужващо звено)
	SET @instType = 1 -- УЧИЛИЩА (всички други)

	RETURN (@instType);
END;
GO

