USE [neispuo]
GO

/****** Object:  StoredProcedure [reginst_basic].[neispuoInstitutionCreateUpdate]    Script Date: 6.10.2021 г. 8:49:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE  [reginst_basic].[neispuoInstitutionCreateUpdate]
	-- Add the parameters for the stored procedure here
	@InstitutionID int,
	@OperationType int
AS

IF (@OperationType=1) --CREATE Institution
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	INSERT INTO core.Institution(InstitutionID,Name,Abbreviation,Bulstat,CountryID,LocalAreaID,FinancialSchoolTypeID,DetailedSchoolTypeID,BudgetingSchoolTypeID,TownID,BaseSchoolTypeID)
	SELECT InstitutionID,Name,Abbreviation,Bulstat,CountryID,LocalAreaID,FinancialSchoolTypeID,DetailedSchoolTypeID,BudgetingSchoolTypeID,TownID,BaseSchoolTypeID
	FROM reginst_basic.RINEISPUOTransfer
	WHERE @InstitutionID=InstitutionID
END
IF (@OperationType=2) --UPDATE Institution
BEGIN

UPDATE core.Institution 
SET core.Institution.InstitutionID=reginst_basic.RINEISPUOTransfer.InstitutionID,
	core.Institution.Name=reginst_basic.RINEISPUOTransfer.Name,
	core.Institution.Abbreviation=reginst_basic.RINEISPUOTransfer.Abbreviation,
	core.Institution.Bulstat=reginst_basic.RINEISPUOTransfer.Bulstat,
	core.Institution.CountryID=reginst_basic.RINEISPUOTransfer.CountryID,
	core.Institution.LocalAreaID=reginst_basic.RINEISPUOTransfer.LocalAreaID,
	core.Institution.FinancialSchoolTypeID=reginst_basic.RINEISPUOTransfer.FinancialSchoolTypeID,
	core.Institution.DetailedSchoolTypeID=reginst_basic.RINEISPUOTransfer.DetailedSchoolTypeID,
	core.Institution.BudgetingSchoolTypeID=reginst_basic.RINEISPUOTransfer.BudgetingSchoolTypeID,
	core.Institution.TownID=reginst_basic.RINEISPUOTransfer.TownID,
	core.Institution.BaseSchoolTypeID=reginst_basic.RINEISPUOTransfer.BaseSchoolTypeID

FROM reginst_basic.RINEISPUOTransfer 
	JOIN core.Institution 
	ON core.Institution.InstitutionID=reginst_basic.RINEISPUOTransfer.InstitutionID 
WHERE @InstitutionID=core.Institution.InstitutionID

END
GO

