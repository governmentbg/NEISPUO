USE [neispuo]
GO

/****** Object:  UserDefinedFunction [inst_year].[getAlphabets]    Script Date: 7.4.2022 г. 11:38:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Venci Mateev	
-- Create date: 7.4.2022 г. 10:16:57
-- Description: Use the following function to extract alphabets from string
-- =============================================
CREATE OR ALTER FUNCTION [inst_year].[getAlphabets]
               ( @strAlphaNumeric VARCHAR(256))
RETURNS VARCHAR(256)
AS
BEGIN
	DECLARE @intAlpha INT
	SET @intAlpha = PATINDEX('%[^a-zA-Zа-яА-Я]%', @strAlphaNumeric)
	BEGIN
		WHILE @intAlpha > 0
		BEGIN
			SET @strAlphaNumeric = STUFF(@strAlphaNumeric, @intAlpha, 1, '' )
			SET @intAlpha = PATINDEX('%[^a-zA-Zа-яА-Я]%', @strAlphaNumeric )
		END
	END
	RETURN ISNULL(@strAlphaNumeric,0)
END
GO

