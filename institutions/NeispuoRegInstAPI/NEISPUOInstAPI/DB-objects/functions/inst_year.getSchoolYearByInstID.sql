USE [neispuo]
GO

/****** Object:  UserDefinedFunction [inst_year].[getSchoolYearByInstID]    Script Date: 20.10.2021 г. 8:13:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER FUNCTION [inst_year].[getSchoolYearByInstID]  
(
	@instID INT
)
RETURNS int
AS
BEGIN
	DECLARE @schoolYear int

	SELECT @schoolYear=SchoolYear FROM core.InstitutionConfData where InstitutionID=@instID

	RETURN @schoolYear
END;
GO

