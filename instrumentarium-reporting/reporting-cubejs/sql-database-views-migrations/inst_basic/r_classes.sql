/*ORDER BY InstitutionID, [ClassGroup].BasicClassID*/
CREATE VIEW [inst_basic].[R_Classes]
AS

WITH MaxOfGroup AS
(
SELECT COUNT([inst_year].ClassGroup.ClassGroupNum) AS MaxOfGroup
		, ParentClassID
		, InstitutionID
		, SchoolYear
FROM [inst_year].ClassGroup
WHERE inst_year.ClassGroup.IsNotPresentForm<>1
GROUP BY InstitutionID
		, SchoolYear
		, ParentClassID
HAVING inst_year.ClassGroup.ParentClassID IS NOT NULL 
)


SELECT inst_year.ClassGroup.ClassID 
		, inst_year.ClassGroup.ClassName AS 'ClassName' -- Паралелка/група - наименование
		, inst_year.ClassGroup.ClassGroupNum AS 'ClassGroupNum' -- Група/Подгрупа
		, inst_nom.BasicClass.Description AS 'BasicClass' -- Випуск/Възрастова група
		, inst_nom.ClassType.Name AS 'ClassTypeName' -- Вид
		, inst_nom.EduForm.Name AS 'EduFormName' -- Форма на обучение
		, inst_nom.ClassEduDuration.Name AS 'ClassEduDurationName' -- Срок на обучение
		, inst_nom.ClassShift.Name AS 'ClassShiftName' -- Организация на учебния процес
		--, inst_nom.SPPOOProfession.SPPOOProfessionCode + ' ' + inst_nom.SPPOOProfession.Name + ', ' + inst_nom.SPPOOSpeciality.SPPOOSpecialityCode + ' ' + inst_nom.SPPOOSpeciality.Name + ' - ' + CONVERT(NVARCHAR(4), 
  --                inst_nom.SPPOOSpeciality.VETLevel) + ' СПК' + (CASE WHEN inst_nom.SPPOOSpecialityDetails.[IsProtected] = 1 THEN ' (Защитена) ' WHEN inst_nom.SPPOOSpecialityDetails.[IsPriority] = 1 THEN ' (Приоритетна за пазара на труда) ' ELSE '' END)
		--				AS 'Професия/Специалност'
		, inst_nom.FLStudyType.Name AS 'FLStudyTypeName' -- Начин на изучаване на ЧЕ
		, inst_nom.FL.Name AS 'FLName' -- Чужд език
		, CASE WHEN [inst_basic].InstitutionDepartment.IsMain = 1 THEN 'Основна сграда - ' + [location].Town.[Name] + ', ' + [inst_basic].InstitutionDepartment.[Address] ELSE [inst_basic].InstitutionDepartment.[Name] + ', ' + [location].Town.[Name] +
                   ', ' + [inst_basic].InstitutionDepartment.[Address] END AS 'Address' --Адрес на обучение

		, noms.BudgetingInstitution.Name AS 'BudgetingInstitutionName' -- Финансира се от
		, inst_nom.EntranceLevel.Name AS 'EntranceLevel' -- Прием след:
		, inst_nom.[SPPOOProfArea].SPPOOProfAreaCode + ' ' + inst_nom.SPPOOProfArea.Name AS 'SPPOOProfAreaName' -- Професионално направление
		, inst_nom.SPPOOProfession.SPPOOProfessionCode + ' ' + inst_nom.SPPOOProfession.Name AS 'SPPOOProfessionName' -- Професия
		, inst_nom.SPPOOSpeciality.SPPOOSpecialityCode + ' ' + inst_nom.SPPOOSpeciality.Name AS 'SPPOOSpeciality' -- Специалност
		, inst_nom.SPPOOSpeciality.VETLevel AS 'VETLevel' -- СПК
		, CASE WHEN inst_nom.SPPOOSpecialityDetails.[IsProtected] = 1 THEN 'да' ELSE '' END AS 'IsProtected' -- Защитена специалност
		, CASE WHEN inst_nom.SPPOOSpecialityDetails.[IsPriority] = 1 THEN 'да' ELSE '' END AS 'IsPriority' -- Приоритетна специалност
		, CASE WHEN [IsProfModule]=1 THEN 'да' ELSE 'не' END AS 'IsProfModule' -- Обучение по модули
		, StudentCountPlaces AS 'StudentCountPlaces' -- Брой места в групата
		, [Notes] AS 'Notes' -- Бележки
		, CASE WHEN [IsCombined]  = 1 THEN 'да' ELSE '' END AS 'IsCombined' -- Слята
		, CASE WHEN [IsSpecNeed]  = 1 THEN 'да' ELSE '' END AS 'IsSpecNeed' -- Специална/за деца със СОП
		, ROUND(CAST(1 AS FLOAT) / CAST(MaxOfGroup.MaxOfGroup AS FLOAT),2) AS 'ClassWeigth' -- Тежест на паралелката/групата
		, inst_year.ClassGroup.SchoolYear
		, inst_year.ClassGroup.InstitutionID
		, A.TownID as [TownID]
		, location.Municipality.MunicipalityID as [MunicipalityID]
		, location.Region.RegionID as RegionID
        , A.BudgetingSchoolTypeID as BudgetingSchoolTypeID

FROM  inst_year.ClassGroup 
	LEFT OUTER JOIN inst_nom.SPPOOSpeciality 
		ON inst_year.ClassGroup.ClassSpecialityID = inst_nom.SPPOOSpeciality.SPPOOSpecialityID 
	LEFT OUTER JOIN inst_nom.SPPOOProfession 
		ON inst_nom.SPPOOSpeciality.ProfessionID = inst_nom.SPPOOProfession.SPPOOProfessionID 
	LEFT JOIN [inst_nom].[SPPOOProfArea]
		ON inst_nom.SPPOOProfession.ProfAreaID=[inst_nom].[SPPOOProfArea].SPPOOProfAreaID
	LEFT OUTER JOIN inst_nom.BasicClass 
		ON inst_year.ClassGroup.BasicClassID = inst_nom.BasicClass.BasicClassID 
	LEFT OUTER JOIN inst_nom.ClassType 
		ON inst_year.ClassGroup.ClassTypeID = inst_nom.ClassType.ClassTypeID 
	LEFT OUTER JOIN inst_nom.EduForm 
		ON inst_year.ClassGroup.ClassEduFormID = inst_nom.EduForm.ClassEduFormID 
	LEFT OUTER JOIN inst_nom.ClassShift 
		ON inst_year.ClassGroup.ClassShiftID = inst_nom.ClassShift.ClassShiftID 
	LEFT OUTER JOIN inst_nom.ClassEduDuration 
		ON inst_year.ClassGroup.ClassEduDurationID = inst_nom.ClassEduDuration.ClassEduDurationID
	LEFT OUTER JOIN inst_nom.EntranceLevel
		ON inst_year.ClassGroup.EntranceLevelID = inst_nom.EntranceLevel.EntranceLevelID
	LEFT OUTER JOIN noms.BudgetingInstitution
		ON inst_year.ClassGroup.BudgetingClassTypeID = noms.BudgetingInstitution.BudgetingInstitutionID
	LEFT OUTER JOIN inst_nom.FL 
		ON inst_year.ClassGroup.FLID = inst_nom.FL.FLID 
	LEFT OUTER JOIN inst_nom.FLStudyType 
		ON inst_year.ClassGroup.FLTypeID = inst_nom.FLStudyType.FLStudyTypeID 
	LEFT OUTER JOIN inst_basic.InstitutionDepartment 
		ON inst_year.ClassGroup.InstitutionDepartmentID = inst_basic.InstitutionDepartment.InstitutionDepartmentID 
			AND  inst_year.ClassGroup.InstitutionID = inst_basic.InstitutionDepartment.InstitutionID 
	LEFT OUTER JOIN location.Town 
		ON inst_basic.InstitutionDepartment.TownID = location.Town.TownID
	LEFT JOIN inst_nom.SPPOOSpecialityDetails
		ON inst_year.ClassGroup.ClassSpecialityID = inst_nom.SPPOOSpecialityDetails.SpecialityID
			AND inst_nom.SPPOOSpecialityDetails.SchoolYear=(CASE 
														WHEN inst_year.ClassGroup.BasicClassID = 8 THEN inst_basic.getCurrYear()
														WHEN inst_year.ClassGroup.BasicClassID = 9 THEN inst_basic.getCurrYear()-1
														WHEN inst_year.ClassGroup.BasicClassID = 10 THEN inst_basic.getCurrYear()-2
														WHEN inst_year.ClassGroup.BasicClassID = 11 THEN inst_basic.getCurrYear()-3
														WHEN inst_year.ClassGroup.BasicClassID = 12 THEN inst_basic.getCurrYear()-4
														ELSE 0
														END)

	INNER JOIN core.Institution AS A 
		ON inst_year.ClassGroup.InstitutionID=A.InstitutionID
	INNER JOIN MaxOfGroup
		ON ClassGroup.InstitutionID=MaxOfGroup.InstitutionID
			AND ClassGroup.SchoolYear=MaxOfGroup.SchoolYear
			AND ClassGroup.ParentClassID=MaxOfGroup.ParentClassID
	INNER JOIN location.Municipality
		ON location.Town.MunicipalityID=location.Municipality.MunicipalityID
	INNER JOIN location.Region
		ON location.Municipality.RegionID=location.Region.RegionID

WHERE inst_year.ClassGroup.ParentClassID IS NOT NULL 
		AND (inst_year.ClassGroup.IsNotPresentForm =0 OR inst_year.ClassGroup.IsNotPresentForm IS NULL) 
		AND A.BaseSchoolTypeID IN (11,12,13,14,15)
GO

