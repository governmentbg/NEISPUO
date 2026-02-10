USE [neispuo]
GO

/****** Object:  UserDefinedFunction [noms].[getInstKind]    Script Date: 8.10.2021 г. 3:31:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER FUNCTION [noms].[getInstKind] 
(
	@detailedSchoolTypeID INT,
	@financialSchoolTypeID INT,
	@region int
)
RETURNS int
AS
BEGIN
	DECLARE @instKind int;

	
	IF (@detailedSchoolTypeID IN (1,2,3,4,5,6,7,8,10,21,22,31,32,33,34,50,51,52,53,54,55,56,57,58,71,112,113,114,121,122,123,124,125,126,131,132,133,134,135,136,141)) AND (@financialSchoolTypeID IN (1,4,5)) AND (@region<>29) SET @instKind = 1 ELSE --publicSchool
	IF (@detailedSchoolTypeID IN (1,2,3,4,5,6,7,8,10,21,22,31,32,33,34,50,51,52,53,54,55,56,57,58,71,112,113,114,121,122,123,124,125,126,131,132,133,134,135,136,141)) AND (@financialSchoolTypeID IN (1,4,5)) AND (@region=29) SET @instKind = 14 ELSE --bgSchoolAbroad
	IF (@detailedSchoolTypeID IN (173)) SET @instKind = 11 ELSE --sozOther
	IF (@detailedSchoolTypeID IN (9,111)) SET @instKind = 4 ELSE --spiritualSchool
	IF (@detailedSchoolTypeID IN (181)) SET @instKind = 13 ELSE --internContractSchool
	IF (@detailedSchoolTypeID IN (40,41,42,43,44,45,81,82,161,163,164,165,166)) SET @instKind = 8 ELSE --cplr
	IF (@detailedSchoolTypeID IN (171,172, 165)) SET @instKind = 12 ELSE --sozNDDDLC
	IF (@detailedSchoolTypeID IN (1,2,3,4,5,6,7,8,10,21,22,31,32,33,34,50,51,52,53,54,55,56,57,58,71,112,113,114,121,122,123,124,125,126,131,132,133,134,135,136,141)) AND (@financialSchoolTypeID IN (2)) SET @instKind = 2 ELSE --municipalitySchool
	IF (@detailedSchoolTypeID IN (1,2,3,4,5,6,7,8,10,21,22,31,32,33,34,50,51,52,53,54,55,56,57,58,71,112,113,114,121,122,123,124,125,126,131,132,133,134,135,136,141)) AND (@financialSchoolTypeID IN (3)) SET @instKind = 3 ELSE --privateSchool
	IF (@detailedSchoolTypeID IN (91,92,93,94,151)) AND (@financialSchoolTypeID IN (1,4,5)) SET @instKind = 5 ELSE --publicKindergarden
	IF (@detailedSchoolTypeID IN (91,92,93,94,151)) AND (@financialSchoolTypeID IN (2)) SET @instKind = 6 ELSE --municipalityKindergarden
	IF (@detailedSchoolTypeID IN (91,92,93,94,151)) AND (@financialSchoolTypeID IN (3)) SET @instKind = 7 ELSE --privateKindergarden
	IF (@detailedSchoolTypeID IN (162)) AND (@financialSchoolTypeID IN (1)) SET @instKind = 9 ELSE --publicCsop
	IF (@detailedSchoolTypeID IN (162)) AND (@financialSchoolTypeID IN (2)) SET @instKind = 10 ELSE --municipalityCsop
	SET @instKind = 0

	RETURN (@instKind);
END;
GO

