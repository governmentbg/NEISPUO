import { MigrationInterface, QueryRunner } from 'typeorm';

export class addIsNotMeetReqColumnINRPersonalView1691078483358
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP VIEW IF EXISTS [inst_basic].[R_Personal];`);

    await queryRunner.query(
      `	CREATE VIEW [inst_basic].[R_Personal] AS SELECT
	person.FirstName --Име
,
	person.MiddleName --Презиме
,
	person.LastName --Фамилия
,
	person.PermanentAddress --Постоянен адрес
,
	PermanentTown.Name AS [PermanentTownName] --Постоянен адрес - населено място
,
	person.CurrentAddress --Настоящ адрес
,
	CurrentTown.Name AS [CurrentTownName] --Настоящ адрес - населено място
,
	person.PublicEduNumber AS PublicEduNumber --[Персонален образователен номер]
,
	Nationality.Name AS [NationalityName] --Гражданство
,
	person.BirthDate --Дата на раждане
,
	BirthPlaceTown.Name AS [BirthPlaceTownName] --Месторождение - населено място
,
	BirthPlaceCountry.Name AS [BirthPlaceCountryName] --Месторождение - държава
,
	noms.Gender.Name AS [GenderName] --Пол
,
	inst_basic.PersonDetail.Title AS [PersonDetailTitle] --Титла
,
	core.Institution.InstitutionID AS [InstitutionID],
	core.Institution.Name AS [InstitutionName] --Институция - Пълно наименование
,
	core.Institution.Abbreviation AS [InstitutionAbbreviation] --Институция - Кратко наименование
,
	location.Region.Name AS [RegionName] --Институция - Област
,
	location.Municipality.Name AS [MunicipalityName] --Институция - Община
,
	location.Town.Name AS [TownName] --Институция - Населено място
,
	location.LocalArea.Name AS [LocalAreaName] --Институция - Район
,
	core.Institution.TownID as [TownID],
	location.Municipality.MunicipalityID as [MunicipalityID],
	location.Region.RegionID as RegionID,
	location.LocalArea.LocalAreaID as LocalAreaID,
	core.Institution.BudgetingSchoolTypeID as BudgetingSchoolTypeID,
	noms.BaseSchoolType.Name AS [BaseSchoolTypeName] --Институция - Вид по чл.37
,
	noms.DetailedSchoolType.Name AS [DetailedSchoolTypeName] --Институция - Вид по чл.38 (детайлен)
,
	noms.FinancialSchoolType.Name AS [FinancialSchoolTypeName] --Институция - Вид по чл.35-36 (според собствеността)
,
	noms.BudgetingInstitution.Name AS [BudgetingInstitutionName] --Институция - Източник на финансиране
,
	[WorkStartYear] --Година на постъпване
,
	[WorkExpTotalYears] --Трудов стаж - общ
,
	[WorkExpSpecYears] --Трудов стаж - по специалността
,
	[WorkExpTeachYears] --Трудов стаж - учителски
	--, [ExperienceYear] AS [Професионален опит]
,
	[StaffOrd] --Пореден №
,
	[StaffPositionNo] --Длъжност №
,
	PositionKind.Name AS [PositionKindName] --Титуляр/заместник
,
	inst_nom.StaffType.Name AS [StaffTypeName] --Вид персонал
,
	inst_nom.CategoryStaffType.Name AS [CategoryStaffTypeName] --Категория персонал
,
	[PositionCount] --Щат
,   CASE
		WHEN [inst_basic].[StaffPosition].IsNotMeetReq = 1 THEN 'да'
		ELSE 'не'
	END AS [IsNotMeetReq] -- --Не отговаря на изискванията (щатна бройка)
,   ContractWith.Name AS [ContractWithName] --Назначен към
,
	NKPDPosition.Code + ' ' + NKPDPosition.Name AS [NKPDPositionName] --Длъжност
,
	SubjectGroup.Name AS [SubjectGroupName] --Назначен на щатно място по:
,
	CASE
		WHEN [CurrentlyValid] = 1 THEN 'да'
		ELSE 'не'
	END AS [CurrentlyValid] --Активна за текущата учебна година
,
	PositionNotes --Бележки по длъжността
,
	ContractType.Name AS [ContractTypeName] --Вид на договора
,
	ContractReason.Name AS [ContractReasonName] --Основание по КТ
,
	[ContractNo] --№ на договора
,
	[ContractYear] --Година на договора
,
	[ContractNotes] --Бележки
,
	CASE
		WHEN [IsAccountablePerson] = 1 THEN 'да'
		ELSE 'не'
	END AS [IsAccountablePerson] --МОЛ
,
	CASE
		WHEN [IsTravel] = 1 THEN 'да'
		ELSE 'не'
	END AS [IsTravel] --Пътуващ от друго населено място
,
	CASE
		WHEN inst_basic.PersonDetail.IsExtendStudent = 1 THEN 'да'
		ELSE 'не'
	END AS [IsExtendStudent] --Продължава образованието си
,
	CASE
		WHEN inst_basic.PersonDetail.IsPensioneer = 1 THEN 'да'
		ELSE 'не'
	END AS [IsPensioneer] --Работещ пенсионер
,
	CASE
		WHEN [IsMentor] = 1 THEN 'да'
		ELSE 'не'
	END AS [IsMentor] --Учител - наставник
,
	CASE
		WHEN [IsTrainee] = 1 THEN 'да'
		ELSE 'не'
	END AS [IsTrainee] --Учител - стажант
,
	CASE
		WHEN [IsHospital] = 1 THEN 'да'
		ELSE 'не'
	END AS [IsHospital] --Болничен учител
,
	Norma --Минимална ЗНПР
,
	NormaT --Индивидуална ЗНПР
,
	[ReductionHours] --Всичко възложени часове (с редукция)
,
	LectYear --Лекторски / недостиг - инд.ЗНПР (год.)
,
	PedStaffData.SchoolYear
,
	inst_basic.PersonDetail.PhoneNumber --телефонен номер
,
	inst_basic.PersonDetail.Email --мейл
  
--	STRING_AGG(CAST([inst_basic].[PersonOKS].AcquiredPK  as nvarchar(MAX)),', ') as AcquiredPK--Придобита Професионална Квалификация
,  
	[inst_basic].[PersonOKS].AcquiredPK  as AcquiredPK--Придобита Професионална Квалификация
,
	[inst_nom].[EducationGradeType].Name as EducationGradeType--образователно степен

--	STRING_AGG(CAST([inst_basic].[PersonOKS].Speciality as nvarchar(MAX)),', ') as SpecialityOKS--специалност 
,
	[inst_basic].[PersonOKS].Speciality as SpecialityOKS--специалност
	
,	[inst_nom].[PKSType].Name as PKSType -- професионална квалификационна степен

--	[inst_nom].[QCourseType].Name as QCourseType--завършен квалификационнен курс

--	[inst_basic].[PersonQCourse].QCourseTopic as QCourseTopic --тема на квалификационния курс

--,	[inst_nom].[QCourseDurationType].Name as QCourseDurationType--тип продължителност на квалификационния курс
,
	SUM([inst_basic].[PersonQCourse].QCourseDurationCredits) as QCourseDurationCredits --кредити от квалификационния курс
FROM
	core.Person as person
	INNER JOIN inst_basic.PersonDetail ON person.PersonID = inst_basic.PersonDetail.PersonID
	LEFT JOIN [inst_basic].[PersonOKS] ON [inst_basic].[PersonOKS].PersonID=person.PersonID
	LEFT JOIN [inst_nom].[EducationGradeType] ON [inst_nom].[EducationGradeType].EducationGradeTypeID=[inst_basic].[PersonOKS].EducationGradeTypeID
    LEFT JOIN [inst_basic].[PersonPKS] ON [inst_basic].[PersonPKS].PersonID=person.PersonID
	LEFT JOIN [inst_nom].[PKSType] ON [inst_nom].[PKSType].PKSTypeID=[inst_basic].[PersonPKS].PKSTypeID
	LEFT JOIN [inst_basic].[PersonQCourse] ON [inst_basic].[PersonQCourse].PersonID=person.PersonID
--	LEFT JOIN [inst_nom].[QCourseType] ON [inst_nom].[QCourseType].QCourseTypeID=[inst_basic].[PersonQCourse].QCourseTypeID
--	LEFT JOIN [inst_nom].[QCourseDurationType] ON [inst_nom].[QCourseDurationType].QCourseDurationTypeID=[inst_basic].[PersonQCourse].QCourseDurationTypeID
	LEFT OUTER JOIN inst_basic.StaffPosition ON person.PersonID = inst_basic.StaffPosition.PersonID
	LEFT OUTER JOIN inst_year.PedStaffData ON inst_basic.StaffPosition.StaffPositionID = PedStaffData.StaffPositionID
	AND PedStaffData.IsValid = 1
	INNER JOIN core.Institution ON inst_basic.StaffPosition.InstitutionID = core.Institution.InstitutionID
	INNER JOIN inst_basic.CurrentYear cy on pedStaffData.SchoolYear = cy.CurrentYearID
	AND cy.IsValid = 1
	INNER JOIN location.Town ON core.Institution.TownID = location.Town.TownID
	INNER JOIN location.Municipality ON location.Town.MunicipalityID = location.Municipality.MunicipalityID
	INNER JOIN location.Region ON location.Municipality.RegionID = location.Region.RegionID
	LEFT OUTER JOIN location.LocalArea ON core.Institution.LocalAreaID = location.LocalArea.LocalAreaID
	INNER JOIN noms.BaseSchoolType ON noms.BaseSchoolType.BaseSchoolTypeID = core.Institution.BaseSchoolTypeID
	INNER JOIN noms.DetailedSchoolType ON noms.DetailedSchoolType.DetailedSchoolTypeID = core.Institution.DetailedSchoolTypeID
	INNER JOIN noms.BudgetingInstitution ON noms.BudgetingInstitution.BudgetingInstitutionID = core.Institution.BudgetingSchoolTypeID
	INNER JOIN noms.FinancialSchoolType ON noms.FinancialSchoolType.FinancialSchoolTypeID = core.Institution.FinancialSchoolTypeID
	LEFT OUTER JOIN location.Town AS PermanentTown ON person.PermanentTownID = PermanentTown.TownID
	LEFT OUTER JOIN location.Town AS CurrentTown ON person.CurrentTownID = CurrentTown.TownID
	LEFT OUTER JOIN location.Town AS BirthPlaceTown ON person.BirthPlaceTownID = BirthPlaceTown.TownID
	LEFT OUTER JOIN location.Country AS Nationality ON person.NationalityID = Nationality.CountryID
	LEFT OUTER JOIN location.Country AS BirthPlaceCountry ON person.BirthPlaceCountry = BirthPlaceCountry.CountryID
	LEFT OUTER JOIN noms.PersonalIDType ON person.PersonalIDType = noms.PersonalIDType.PersonalIDTypeID
	LEFT OUTER JOIN noms.Gender ON person.Gender = noms.Gender.GenderID
	LEFT OUTER JOIN inst_nom.StaffType ON inst_basic.StaffPosition.StaffTypeID = inst_nom.StaffType.StaffTypeID
	LEFT OUTER JOIN inst_nom.CategoryStaffType ON inst_basic.StaffPosition.CategoryStaffTypeID = inst_nom.CategoryStaffType.CategoryStaffTypeID
	LEFT OUTER JOIN inst_nom.PositionKind ON inst_basic.StaffPosition.PositionKindID = inst_nom.PositionKind.PositionKindID
	LEFT OUTER JOIN inst_nom.ContractWith ON inst_basic.StaffPosition.ContractWithID = inst_nom.ContractWith.ContractWithID
	LEFT OUTER JOIN inst_nom.NKPDPosition ON inst_basic.StaffPosition.NKPDPositionID = inst_nom.NKPDPosition.NKPDPositionID
	LEFT OUTER JOIN inst_nom.ContractType ON inst_basic.StaffPosition.ContractTypeID = inst_nom.ContractType.ContractTypeID
	LEFT OUTER JOIN inst_nom.ContractReason ON inst_basic.StaffPosition.ContractReasonID = inst_nom.ContractReason.ContractReasonID
	LEFT OUTER JOIN inst_nom.SubjectGroup ON inst_basic.StaffPosition.PositionSubjectGroupID = inst_nom.SubjectGroup.SubjectGroupID
WHERE
	StaffPosition.IsValid = 1 AND [inst_basic].[PersonQCourse].QCourseDurationTypeID in (20,21,22)

GROUP BY person.FirstName --Име
,
	person.MiddleName --Презиме
,
	person.LastName --Фамилия
,
	person.PermanentAddress --Постоянен адрес
,
	PermanentTown.Name  --Постоянен адрес - населено място
,
	person.CurrentAddress --Настоящ адрес
,
	CurrentTown.Name  --Настоящ адрес - населено място
,
	person.PublicEduNumber  --[Персонален образователен номер]
,
	Nationality.Name  --Гражданство
,
	person.BirthDate --Дата на раждане
,
	BirthPlaceTown.Name  --Месторождение - населено място
,
	BirthPlaceCountry.Name  --Месторождение - държава
,
	noms.Gender.Name  --Пол
,
	inst_basic.PersonDetail.Title  --Титла
,
	core.Institution.InstitutionID ,
	core.Institution.Name  --Институция - Пълно наименование
,
	core.Institution.Abbreviation  --Институция - Кратко наименование
,
	location.Region.Name --Институция - Област
,
	location.Municipality.Name  --Институция - Община
,
	location.Town.Name  --Институция - Населено място
,
	location.LocalArea.Name  --Институция - Район
,
	core.Institution.TownID,
	location.Municipality.MunicipalityID ,
	location.Region.RegionID ,
	location.LocalArea.LocalAreaID ,
	core.Institution.BudgetingSchoolTypeID ,
	noms.BaseSchoolType.Name  --Институция - Вид по чл.37
,
	noms.DetailedSchoolType.Name  --Институция - Вид по чл.38 (детайлен)
,
	noms.FinancialSchoolType.Name  --Институция - Вид по чл.35-36 (според собствеността)
,
	noms.BudgetingInstitution.Name --Институция - Източник на финансиране
,
	[WorkStartYear] --Година на постъпване
,
	[WorkExpTotalYears] --Трудов стаж - общ
,
	[WorkExpSpecYears] --Трудов стаж - по специалността
,
	[WorkExpTeachYears] --Трудов стаж - учителски
	--, [ExperienceYear] AS [Професионален опит]
,
	[StaffOrd] --Пореден №
,
	[StaffPositionNo] --Длъжност №
,
[IsNotMeetReq] --Не изпълнява изискванията (щатна бройка)

,
	PositionKind.Name  --Титуляр/заместник
,
	inst_nom.StaffType.Name  --Вид персонал
,
	inst_nom.CategoryStaffType.Name  --Категория персонал
,
	[PositionCount] --Щат
,
	ContractWith.Name --Назначен към
,
	NKPDPosition.Code , NKPDPosition.Name  --Длъжност
,
	SubjectGroup.Name  --Назначен на щатно място по:
,
	 [CurrentlyValid] --Активна за текущата учебна година
,
	PositionNotes --Бележки по длъжността
,
	ContractType.Name  --Вид на договора
,
	ContractReason.Name  --Основание по КТ
,
	[ContractNo] --№ на договора
,
	[ContractYear] --Година на договора
,
	[ContractNotes] --Бележки
,
	[IsAccountablePerson] --МОЛ
,
	[IsTravel] --Пътуващ от друго населено място
,
	 [IsExtendStudent] --Продължава образованието си
,
	 [IsPensioneer] --Работещ пенсионер
,
	[IsMentor] --Учител - наставник
,
	[IsTrainee] --Учител - стажант
,
	 [IsHospital] --Болничен учител
,
	Norma --Минимална ЗНПР
,
	NormaT --Индивидуална ЗНПР
,
	[ReductionHours] --Всичко възложени часове (с редукция)
,
	LectYear --Лекторски / недостиг - инд.ЗНПР (год.)
,
	PedStaffData.SchoolYear
,
	inst_basic.PersonDetail.PhoneNumber --телефонен номер
,
	inst_basic.PersonDetail.Email --мейл
,
	[inst_nom].[EducationGradeType].Name --образователно степен
,	[inst_basic].[PersonOKS].AcquiredPK
,   [inst_basic].[PersonOKS].Speciality
,   [inst_nom].[PKSType].Name
--,	[inst_nom].[QCourseDurationType].Name

`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
