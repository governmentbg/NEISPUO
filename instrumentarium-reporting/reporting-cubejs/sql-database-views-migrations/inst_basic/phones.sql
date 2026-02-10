CREATE VIEW [inst_basic].[Phones]
AS

SELECT DISTINCT A.InstitutionID AS [InstitutionID]
		, A.Name AS InstitutionName -- AS [Пълно наименование]
		, A.Abbreviation AS InstitutionAbbreviation -- AS [Кратко наименование]
		, location.Country.Name AS CountryName -- AS [Държава]
		, location.Region.Name AS RegionName-- AS [Област]
		, location.Municipality.Name AS MunicipalityName -- AS [Община]
		, location.Town.Name AS TownName -- AS [Населено място]
		, location.LocalArea.Name AS LocalAreaName -- AS [Район]
		, A.TownID as [TownID]
		, location.Municipality.MunicipalityID as [MunicipalityID]
		, location.Region.RegionID as RegionID
		, location.LocalArea.LocalAreaID as LocalAreaID
		, inst_basic.InstitutionDetail.Email AS InstitutionEmail
        , A.BudgetingSchoolTypeID as BudgetingSchoolTypeID
		, noms.BaseSchoolType.Name  AS BaseSchoolTypeName -- AS [Вид по чл.37]
		, noms.DetailedSchoolType.Name AS DetailedSchoolTypeName -- AS [Вид по чл.38 (детайлен)]
		, noms.FinancialSchoolType.Name AS FinancialSchoolTypeName -- AS [Вид по чл.35-36 (според собствеността)]
		, noms.BudgetingInstitution.Name AS BudgetingInstitutionName -- AS [Източник на финансиране]
		, PhoneType.Name AS PhoneType -- AS [Тип телефон]
		, [PhoneCode] AS PhoneCode --AS [Код за АТМ]
		, [PhoneNumber]  AS PhoneNumber --AS [Телефон]
		, [ContactKind] AS ContactKind -- [Вид телефон]
		, CASE WHEN InstitutionPhone.IsMain=1 THEN 'да' ELSE 'не' END AS IsMain -- [Основен]
FROM core.Institution AS A 
		INNER JOIN inst_basic.InstitutionDepartment AS instdep 
			ON instdep.InstitutionID = A.InstitutionID 
		INNER JOIN inst_basic.InstitutionDetail
			ON inst_basic.InstitutionDetail.InstitutionID = A.InstitutionID 
		INNER JOIN location.Town
			ON instdep.TownID = location.Town.TownID 
		INNER JOIN location.Municipality
			ON location.Town.MunicipalityID=location.Municipality.MunicipalityID
		INNER JOIN location.Region
			ON location.Municipality.RegionID=location.Region.RegionID
		LEFT OUTER JOIN location.LocalArea 
			ON instdep.LocalAreaID = location.LocalArea.LocalAreaID 
		LEFT OUTER JOIN location.Country
			ON instdep.CountryID=location.Country.CountryID
		INNER JOIN noms.BaseSchoolType
			ON noms.BaseSchoolType.BaseSchoolTypeID=A.BaseSchoolTypeID
		INNER JOIN noms.DetailedSchoolType
			ON noms.DetailedSchoolType.DetailedSchoolTypeID=A.DetailedSchoolTypeID
		INNER JOIN noms.BudgetingInstitution
			ON noms.BudgetingInstitution.BudgetingInstitutionID=A.BudgetingSchoolTypeID
		INNER JOIN noms.FinancialSchoolType
			ON noms.FinancialSchoolType.FinancialSchoolTypeID=A.FinancialSchoolTypeID
		LEFT OUTER JOIN inst_basic.InstitutionPhone
			ON instdep.InstitutionID = InstitutionPhone.InstitutionID 
		LEFT OUTER JOIN inst_nom.PhoneType
			ON InstitutionPhone.PhoneTypeID=PhoneType.PhoneTypeID
WHERE  (instdep.IsMain = 1) --AND A.InstitutionID=1301329
GO
