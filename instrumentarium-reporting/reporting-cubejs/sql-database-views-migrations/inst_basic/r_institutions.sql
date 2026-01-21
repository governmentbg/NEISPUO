CREATE VIEW [inst_basic].[R_institutions]
AS

WITH InstAdmDataSchool AS
(
SELECT InstitutionID
		, SchoolYear
		, IsInnovative
		, IsCentral
		, IsProtected
		, [IsStateFunded]
		, [HasMunDecisionFor4]
FROM [inst_nom].[InstAdminData]
WHERE [InstitutionDepartmentID] IS NULL
)
, InstPhone
AS
(
SELECT [InstitutionID]
,STRING_AGG([PhoneNumber],',') AS PhoneNumber
FROM [inst_basic].[InstitutionPhone]
WHERE IsInstitution=1
	AND IsMain=1
GROUP BY InstitutionID
)
SELECT DISTINCT A.InstitutionID AS [InstitutionID]
		, A.Name AS [InstitutionName] --Пълно наименование
		, A.Abbreviation AS [ShortInstitutionName] -- Кратко наименование
		, A.Bulstat AS [Bulstat] -- Булстат/ЕИК
		, location.Country.Name  AS [CountryName] -- Държава
		, location.Region.Name AS [RegionName] -- Област
		, location.Municipality.Name AS [MunicipalityName] -- Община
		, location.Town.Name AS [TownName] -- Населено място
		, location.LocalArea.Name AS [LocalAreaName] -- Район
		, A.TownID as [TownID]
		, location.Municipality.MunicipalityID as [MunicipalityID]
		, location.Region.RegionID as RegionID
		, location.LocalArea.LocalAreaID as LocalAreaID
        , A.BudgetingSchoolTypeID as BudgetingSchoolTypeID
		, instdep.PostCode
		, noms.BaseSchoolType.Name AS [BaseSchoolTypeName] -- Вид по чл.37
		, noms.DetailedSchoolType.Name AS [DetailedSchoolTypeName] -- Вид по чл.38 (детайлен)
		, noms.FinancialSchoolType.Name AS [FinancialSchoolTypeName] -- Вид по чл.35-36 (според собствеността)
		, noms.BudgetingInstitution.Name AS [BudgetingInstitutionName] -- Източник на финансиране
		, CASE WHEN B.IsDelegateBudget=1 THEN 'да' ELSE 'не' END  AS [IsDelegateBudget] -- На делегиран бюджет
		, [YearlyBudget] AS [YearlyBudget] -- Утвърден бюджет за календ.година
		, instdep.Address AS [Address] -- Основен адрес
		, InstPhone.PhoneNumber AS [PhoneNumber] -- Основен телефон
		, B.Email AS [Email] -- Е-мейл
		, B.Website AS [Website] -- Интернет страница
		, B.EstablishedYear AS [EstablishedYear] -- Година на основаване
		, B.ConstitActFirst AS [ConstitActFirst] -- Акт за създаване
		, B.ConstitActLast AS [ConstitActLast] -- Последен акт за преобразуване
		, inst_nom.SchoolShiftType.Name AS [SchoolShiftType] -- Сменност на обучение
		, CASE WHEN InstAdmDataSchool.IsCentral=1 THEN 'да' ELSE 'не' END  AS [IsCentral] -- Средищно/а
		, CASE WHEN InstAdmDataSchool.IsProtected=1 THEN 'да' ELSE 'не' END AS [IsProtected] -- Защитено/а
		, CASE WHEN InstAdmDataSchool.IsInnovative=1 THEN 'да' ELSE 'не' END AS [IsInnovative] -- Иновативно
		, CASE WHEN B.IsNational=1 THEN 'да' ELSE 'не' END  AS [IsNational] -- С национално значение
		, CASE WHEN B.IsProfSchool=1 THEN 'да' ELSE 'не' END  AS [IsProfSchool] -- Училището осигурява професионална подготовка
		, CASE WHEN B.IsNonIndDormitory=1 THEN 'да' ELSE 'не' END  AS [IsNonIndDormitory] -- Към училището има несамостоятелно общежитие
		, CASE WHEN InstAdmDataSchool.HasMunDecisionFor4=1 THEN 'да' ELSE 'не' END  AS [HasMunDecisionFor4] -- Решение на ОС за задълж.предуч.обуч. на 4 год.
		, CASE WHEN B.IsAppInnovSystem=1 THEN 'да' ELSE 'не' END  AS [IsAppInnovSystem] -- ДГ прилага иновативна програмна система
		, CASE WHEN B.IsODZ=1 THEN 'да' ELSE 'не' END AS [IsODZ] -- Към ДГ има яслена група
		, CASE WHEN B.IsProvideEduServ=1 THEN 'да' ELSE 'не' END AS [IsProvideEduServ] -- ДГ организира доп.услуга по отглеждане на децата
		, [reginst_basic].[Directors].FirstName + ' ' + [reginst_basic].[Directors].LastName AS [Director] -- Директор
		, CASE WHEN inst_basic.InstitutionPublicCouncil.InstitutionPublicCouncilID IS NOT NULL THEN 'да'
				ELSE 'не' END [InstitutionPublicCouncil] -- Към училището/ДГ има обществен съвет
		, [StaffCountAll] AS [StaffCountAll] -- Определена численост - персонал общо
		, [PedagogStaffCount] AS [PedagogStaffCount] -- Определена численост - педаг.персонал
		, [NonpedagogStaffCount] AS [NonpedagogStaffCount] -- Определена численост - непедаг.персонал

FROM core.Institution AS A 
		INNER JOIN inst_basic.InstitutionDetail AS B 
			ON A.InstitutionID = B.InstitutionID 
		INNER JOIN inst_basic.InstitutionDepartment AS instdep 
			ON instdep.InstitutionID = B.InstitutionID 
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
		LEFT OUTER JOIN inst_basic.InstitutionPublicCouncil
			ON instdep.InstitutionID=inst_basic.InstitutionPublicCouncil.InstitutionID
		LEFT OUTER JOIN InstAdmDataSchool
			ON instdep.InstitutionID=InstAdmDataSchool.InstitutionID
				AND InstAdmDataSchool.SchoolYear=inst_basic.getCurrYear()      --inst_basic.getCurrYear()
		LEFT OUTER JOIN [reginst_basic].[Directors]
			ON A.InstitutionID=[reginst_basic].[Directors].InstitutionID
				AND [reginst_basic].[Directors].CurrentYearID=inst_basic.getCurrYear()     --[inst_basic].getCurrYear()
		LEFT OUTER JOIN [inst_year].[InstitutionOtherData]
			ON B.InstitutionID=[inst_year].[InstitutionOtherData].InstitutionID
				AND [inst_year].[InstitutionOtherData].SchoolYear=inst_basic.getCurrYear()    ---inst_basic.getCurrYear()
		LEFT OUTER JOIN inst_nom.SchoolShiftType
			ON [inst_year].[InstitutionOtherData].SchoolShiftTypeID=inst_nom.SchoolShiftType.SchoolShiftTypeID
		LEFT OUTER JOIN InstPhone
			ON InstPhone.InstitutionID=instdep.InstitutionID


WHERE  (instdep.IsMain = 1) 

GO

