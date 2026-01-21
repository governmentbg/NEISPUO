import { MigrationInterface, QueryRunner } from 'typeorm';

export class updateRStudentsViewRevertSepacialtyName1683627250527
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP VIEW [reporting].[R_Students];`);

    await queryRunner.query(
      `
	CREATE VIEW [reporting].[R_Students]
			AS -- WITH SCHEMABINDING
		SELECT
		cg.ParentClassID AS ParentClassID,
		-- код на класа
		sc.ClassId AS ClassId,
		-- код на група
		sc.InstitutionID AS 'InstitutionID',
		-- Код по НЕИСПУО
		isy.Name AS 'InstitutionName',
		-- Институция
		isy.Abbreviation AS 'InstitutionAbbreviation',
		r.RegionID,
		-- Кратко наименование
		r.Name AS 'RegionName',
		-- Област
		m.MunicipalityID,
		m.Name AS 'MunicipalityName',
		-- Община
		t.TownID,
		t.Name AS 'TownName',
		-- Населено място
		id.InstitutionDepartmentID as 'InstitutionDepartmentID',
		CASE
		WHEN id.Name IS NULL THEN 'Основна сграда'
		ELSE id.Name
		END AS 'InstitutionDepartmentName',
		-- Филиал
		dst.InstType,
		bst.Name AS 'BasicSchoolTypeName',
		-- по чл. 37
		dst.Name AS 'DetailedSchoolTypeName',
		-- по чл. 38
		fst.Name AS 'FinancialSchoolTypeName',
		-- Финансиране
		isy.BudgetingSchoolTypeID AS BudgetingSchoolTypeID,
		CASE
		WHEN iad.IsInnovative = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsInnovative',
		-- Иновативно
		CASE
		WHEN iad.IsCentral = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsCentral',
		-- Средищно
		CASE
		WHEN iad.IsProtected = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsProtected',
		-- Защитено
		CASE
		WHEN iad.IsStateFunded = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsStateFunded',
		-- С национално значение
		CASE
		WHEN iad.HasMunDecisionFor4 = 1 THEN 'да'
		ELSE 'не'
		END AS 'HasMunDecisionFor4',
		-- Решение за задължително обучение на 4 годишните 
			-- if HasMunDecisionFor4 = 0, basiclass -1, -6 -- предучилищна 5-6годишни,
		-- if HasMunDecisionFor4 = 1, basiclass -1,-6,-3 -- предучилишни групи предучилищна 5-6годишни + 4 годишни
		bc.RomeName AS 'RomeClassName',
		-- Випуск
		cg.ClassName AS 'ClassName',
		-- Паралелка/Група
		ef.Name AS 'EduFormName',
		-- Форма на обучение
		ck.Name AS 'ClassKind',
		-- Категория 
		-- филтрираме тук за предефиниераните справки
		ct.Name AS 'ClassType',
		-- Профил/вид на паралелка/група
		prof.Name AS 'ProfName',
			-- Професия
			spec.Name AS 'SpecialtyName',
			-- Специалност
		sc.PositionId,
		COUNT(sc.ID) AS 'StudentsCount',
		-- Общ брой ученици
		-- Марги - долното отпада
			--COUNT(sc.ID) - COUNT(sny.PersonId) AS 'StudentsNO_CSOPCount',
		-- Брой без СОП
			-- добавено от Марги
			SUM (
		CASE
		WHEN sc.PositionId = 10 THEN 1
		ELSE 0
		END
		) AS 'StudentCSOPCount',
			-- Брой ученици, обучаващи се в ЦСОП
		COUNT(sny.PersonId) AS 'StudentSOPCount',
			-- Брой ученици със СОП
		SUM (
		CASE
		WHEN p.Gender = 2 THEN 1
		ELSE 0
		END
		) AS 'CountMale',
		-- Брой момчета
		SUM (
		CASE
		WHEN p.Gender = 1 THEN 1
		ELSE 0
		END
		) AS 'CountFemale',
		-- Брой момичета
		SUM (
		CASE
		WHEN sc.RepeaterId > 1 THEN 1
		ELSE 0
		END
		) AS 'RepeaterCount',
		-- Брой второгодници
		SUM (
		CASE
		WHEN sc.CommuterTypeId > 1 THEN 1
		ELSE 0
		END
		) AS 'CommuterCount',
		-- Брой пътуващи
		CASE
		WHEN cg.IsSpecNeed = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsCSOP',
		-- Паралелка/група за деца със СОП
		CASE
		WHEN ct.ClassKind = 2 THEN 'да'
		ELSE 'не'
		END AS 'IsCDO',
		-- ЦДО
		-- коригирано от Марги
			SUM
				(CASE
		WHEN sc.IsHourlyOrganization = 1 THEN 1
		ELSE 0
				END)
			AS 'IsHourlyOrganization',
		-- Брой деца на почасова организация
		CASE
		WHEN cg.IsNotPresentForm = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsNotPresentForm',
		-- cg.isnotpresentform = 1 (самостоятелна форма), за всички други е 0
			-- СФО
		CASE
		WHEN cg.IsCombined = 1 THEN 'да'
		ELSE 'не'
		END AS 'IsCombined',
		-- cg.IsCombined = 1 - слята паралелка
			-- Слята паралелка/група
		sc.SchoolYear AS 'SchoolYear' -- година
		FROM
		inst_basic.CurrentYear cy -- указва текуща учебна година за НЕИСПУО
		--- лятото се преминава в нова уч.година -> на първия ден модул справки няма да показва данни;
		--- да се сложат данни
		JOIN inst_nom.InstAdminData iad ON iad.SchoolYear = cy.CurrentYearID
		JOIN student.StudentClass sc ON sc.InstitutionId = iad.InstitutionID
		AND sc.SchoolYear = cy.CurrentYearID
		AND sc.IsCurrent = 1
		JOIN inst_nom.BasicClass bc ON bc.BasicClassID = sc.BasicClassId
		AND bc.IsValid = 1
		JOIN inst_year.ClassGroup cg ON cg.ClassID = sc.ClassID -- връща 3 записа за 1 клас , ако той е разбит на 3 групи
		JOIN inst_nom.EduForm ef ON ef.ClassEduFormID = sc.StudentEduFormId
		JOIN inst_nom.ClassType ct ON ct.ClassTypeID = sc.ClassTypeId
		JOIN inst_nom.ClassKind ck ON ck.ClassKindID = ct.ClassKind
		JOIN core.Person p ON p.PersonID = sc.PersonID
		-- Мaрги - не е необходимо, имаме си го в StudentClass
			--JOIN student.RepeaterReason repeaterReason ON sc.RepeaterId = repeaterReason.Id
		-- Мaрги - не е необходимо, имаме си го в StudentClass
			--JOIN student.CommuterType commuterType ON sc.CommuterTypeID = commuterType.ID
		JOIN core.InstitutionSchoolYear isy ON isy.InstitutionId = sc.InstitutionId
		AND isy.SchoolYear = cy.CurrentYearID -- текуща година за институция
		LEFT JOIN inst_basic.InstitutionDepartment id ON id.InstitutionID = isy.InstitutionId
		AND id.InstitutionDepartmentID = iad.InstitutionDepartmentID
		JOIN location.Town t ON isy.TownID = t.TownID
		JOIN location.Municipality m ON t.MunicipalityID = m.MunicipalityID
		JOIN location.Region r ON m.RegionID = r.RegionID
		JOIN noms.BaseSchoolType bst ON bst.BaseSchoolTypeID = isy.BaseSchoolTypeID
		JOIN noms.DetailedSchoolType dst ON dst.DetailedSchoolTypeID = isy.DetailedSchoolTypeID
		JOIN noms.FinancialSchoolType fst ON fst.FinancialSchoolTypeID = isy.FinancialSchoolTypeID
		LEFT JOIN inst_nom.SPPOOSpeciality spec ON spec.SPPOOSpecialityID = sc.StudentSpecialityId
			--добавено от Марги
			LEFT JOIN inst_nom.SPPOOProfession prof ON prof.SPPOOProfessionID = spec.ProfessionID
			--
		LEFT JOIN student.SpecialNeedsYear sny ON p.PersonID = sny.PersonId
		AND isy.SchoolYear = sny.SchoolYear
		WHERE
			cy.IsValid = 1 -- AND (
			-- ct.ClassKind = 1 -- 1 - всичко видове учебни паралелки, учебни паралелки;
			-- or ct.ClassKind = 2 -- 2 - групи в целодневни организации ЦДО;
			-- -- 3 - всички останали;
			-- )
		GROUP BY
		sc.ClassId,
		sc.SchoolYear,
		sc.InstitutionId,
		sc.PositionId,
		-- Марги - май не е необходимо да се групира по тези
			--sc.IsHourlyOrganization,
		--bc.BasicClassID,
		--bc.Name,
		bc.RomeName,
		cg.ParentClassID,
		cg.ClassName,
		cg.IsSpecNeed,
		cg.IsCombined,
		cg.IsNotPresentForm,
		ck.Name,
		-- Марги - май не е необходимо да се групира по това
			--ct.ClassTypeID,
		ct.ClassKind,
		ct.Name,
		ef.Name,
		spec.Name,
		isy.Name,
		isy.Abbreviation,
		isy.BudgetingSchoolTypeID,
		r.RegionID,
		r.Name,
		m.MunicipalityID,
		m.Name,
		t.TownID,
		t.Name,
		dst.InstType,
		bst.Name,
		dst.Name,
		fst.Name,
		id.InstitutionDepartmentID,
		id.Name,
		iad.IsInnovative,
		iad.IsCentral,
		iad.IsProtected,
		iad.IsStateFunded,
		iad.HasMunDecisionFor4,
			prof.Name;
		`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
