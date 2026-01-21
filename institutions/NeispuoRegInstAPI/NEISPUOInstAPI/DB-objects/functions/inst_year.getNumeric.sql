USE [neispuo]
GO

/****** Object:  UserDefinedFunction [inst_year].[getNumeric]    Script Date: 7.4.2022 г. 11:39:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Venci Mateev	
-- Create date: 7.4.2022 г. 10:16:57
-- Description: Use the following function to extract digits from string
-- =============================================
CREATE OR ALTER FUNCTION [inst_year].[getNumeric] (@strAlphaNumeric VARCHAR(256))
 RETURNS VARCHAR(256)
AS
BEGIN
DECLARE @intAlpha INT
	SET @intAlpha = PATINDEX('%[^0-9]%', @strAlphaNumeric)
BEGIN
	WHILE @intAlpha > 0
		BEGIN
			SET @strAlphaNumeric = STUFF(@strAlphaNumeric, @intAlpha, 1, '' )
			SET @intAlpha = PATINDEX('%[^0-9]%', @strAlphaNumeric )
		END
	END
RETURN ISNULL(@strAlphaNumeric,0)
END
GO

