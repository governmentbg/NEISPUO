import { MigrationInterface, QueryRunner } from 'typeorm';

export class updateRStudentsView1680164460506 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP VIEW [reporting].[R_Students];`);

    await queryRunner.query(
      `CREATE VIEW [reporting].[R_Students] AS --- WITH SCHEMABINDING
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
    -- кратко наименование
    r.Name AS 'RegionName',
    -- Област
    m.MunicipalityID,
    m.Name AS 'MunicipalityName',
    -- Община
    t.TownID,
    t.Name AS 'TownName',
    -- Населено място
    id.InstitutionDepartmentID,
    CASE
        WHEN id.Name IS NULL THEN 'Основна институция'
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
    -- Иновативна институция
    CASE
        WHEN iad.IsCentral = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsCentral',
    --  Средищна институция
    CASE
        WHEN iad.IsProtected = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsProtected',
    --  Средищна институция
    CASE
        WHEN iad.IsStateFunded = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsStateFunded',
    -- Национална институция
    CASE
        WHEN iad.HasMunDecisionFor4 = 1 THEN 'да'
        ELSE 'не'
    END AS 'HasMunDecisionFor4',
    --- if HasMunDecisionFor4 = 0, basiclass -1, -6 -- предучилищна 5-6годишни, 
    --- if HasMunDecisionFor4 = 1, basiclass -1,-6,-3 -- предучилишни групи предучилищна 5-6годишни + 4 годишни    
    bc.RomeName AS 'RomeClassName',
    -- Клас 
    cg.ClassName AS 'ClassName',
    -- Паралелка/Група
    ef.Name AS 'EduFormName',
    -- Форма на обучение
    ck.Name AS 'ClassKind',
    --Вид
    -- филтрираме тук за предефиниераните справки
    ct.Name AS 'ClassType',
    --Вид на паралелка/група
    spec.Name AS 'SpecialtyName',
    sc.PositionId,
    COUNT(sc.ID) AS 'StudentsCount',
    -- Брой ученици
    COUNT(sc.ID) - COUNT(sny.PersonId) AS 'StudentsNO_CSOPCount',
    --- Брой без СОП
    COUNT(sny.PersonId) AS 'StudentCSOPCount',
    SUM (
        CASE
            WHEN p.Gender = 2 THEN 1
            ELSE 0
        END
    ) AS 'CountMale',
    --- Брой Момчета
    SUM (
        CASE
            WHEN p.Gender = 1 THEN 1
            ELSE 0
        END
    ) AS 'CountFemale',
    --- Брой Момичета
    SUM (
        CASE
            WHEN repeaterReason.Id > 1 THEN 1
            ELSE 0
        END
    ) AS 'RepeaterCount',
    --- Брой второгодници
    SUM (
        CASE
            WHEN commuterType.ID > 1 THEN 1
            ELSE 0
        END
    ) AS 'CommuterCount',
    --- Брой пъруващи
    CASE
        WHEN cg.IsSpecNeed = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsCSOP',
    -- Група за деца със СОП
    CASE
        WHEN ct.ClassKind = 2 THEN 'да'
        ELSE 'не'
    END AS 'IsCDO',
    -- ЦДО
    CASE
        WHEN sc.IsHourlyOrganization = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsHourlyOrganization',
    --Почасова организация
    CASE
        WHEN cg.IsNotPresentForm = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsNotPresentForm',
    -- cg.isnotpresentform = 1 (самостоятелна форма), за всички други е 0
    CASE
        WHEN cg.IsCombined = 1 THEN 'да'
        ELSE 'не'
    END AS 'IsCombined',
    -- cg.IsCombined = 1 - слята паралелка
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
    JOIN student.RepeaterReason repeaterReason ON sc.RepeaterId = repeaterReason.Id
    JOIN student.CommuterType commuterType ON sc.CommuterTypeID = commuterType.ID
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
    LEFT JOIN student.SpecialNeedsYear sny ON p.PersonID = sny.PersonId
    AND isy.SchoolYear = sny.SchoolYear
WHERE
    cy.IsValid = 1
-- AND (
    --     ct.ClassKind = 1 -- 1 - всичко видове учебни паралелки, учебни паралелки; 
    --     or ct.ClassKind = 2 -- 2 - групи в целодневни организации ЦДО; 
    --     -- 3 - всички останали; 
    -- )
GROUP BY
    sc.ClassId,
    sc.SchoolYear,
    sc.InstitutionId,
    sc.PositionId,
    sc.IsHourlyOrganization,
    bc.BasicClassID,
    bc.Name,
    bc.RomeName,
    cg.ParentClassID,
    cg.ClassName,
    cg.IsSpecNeed,
    cg.IsCombined,
    cg.IsNotPresentForm,
    ck.Name,
    ct.ClassTypeID,
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
    iad.HasMunDecisionFor4;`,
    );
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
