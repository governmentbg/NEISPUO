USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getNewInstID]    Script Date: 8.10.2021 г. 3:36:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER FUNCTION [noms].[getNewInstID] 
(
	@Region INT
)
RETURNS int
AS
BEGIN
	DECLARE @newInstID int;
	DECLARE @startcode int;
	DECLARE @endcode int;

	IF @Region = 3 -- Varna/Burgas swap
	BEGIN
	SET @Region = 4
	END
	ELSE IF @Region = 4
	BEGIN
	SET @Region = 3
	END

	SET @startcode = @Region * 100000;
	SET @endcode = (@Region * 100000) +  + 99999;

	SELECT  TOP 1
			@newInstID = InstitutionID + 1
	FROM    reginst_basic.RIInstitution SD1
	WHERE   NOT EXISTS
			(
			SELECT  NULL
			FROM   reginst_basic.RIInstitution SD2 
			WHERE   SD2.InstitutionID = SD1.InstitutionID + 1
			)
	AND InstitutionID BETWEEN @startcode AND @endcode
	ORDER BY
			InstitutionID

	RETURN (@newInstID);
END;
GO

