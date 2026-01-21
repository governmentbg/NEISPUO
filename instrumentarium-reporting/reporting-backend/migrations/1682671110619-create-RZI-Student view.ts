import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class createRZIStudentsView1682671110619 implements MigrationInterface {
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`DROP VIEW [reporting].[RZI_Students];`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.MON_ADMIN});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.CIOO});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.RUO});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.RUO_EXPERT});`);

    await queryRunner.query(`
        INSERT INTO "reporting"."SchemaRoleAccess"("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_RZI_STUDENTS}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE view [reporting].[R_RZI_Students]
as

WITH cte
AS
(SELECT
	cg.InstitutionID,
	COUNT(cg.ClassID) AS classesCounted
 FROM
	inst_year.ClassGroup cg
	INNER JOIN core.InstitutionSchoolYear isy ON isy.InstitutionId = cg.InstitutionID
											AND isy.SchoolYear = cg.SchoolYear
											AND isy.IsCurrent = 1
	INNER JOIN inst_nom.ClassType ct ON ct.ClassTypeID = cg.ClassTypeID
 WHERE
 	ct.ClassKind = 1
	AND cg.ParentClassID IS NULL
	AND cg.IsValid = 1
 GROUP BY
	cg.InstitutionID
) 

, stdsByAge
AS
(
 SELECT
 	sc.InstitutionID AS InstitutionID
   ,COUNT(*) AS allStds
   ,SUM(CASE WHEN age.val >= 1 AND age.val <= 3 AND p.Gender = 2 THEN 1 ELSE 0 END) AS a1_3male
   ,SUM(CASE WHEN age.val >= 1 AND age.val <= 3 AND p.Gender = 1 THEN 1 ELSE 0 END) AS a1_3female
   ,SUM(CASE WHEN age.val >= 4 AND age.val <= 7 AND sc.BasicClassId < 0 AND p.Gender = 2 THEN 1 ELSE 0 END) AS a4_7malePG
   ,SUM(CASE WHEN age.val >= 4 AND age.val <= 7 AND sc.BasicClassId < 0 AND p.Gender = 1 THEN 1 ELSE 0 END) AS a4_7femalePG
   ,SUM(CASE WHEN age.val >= 7 AND age.val <= 13 AND sc.BasicClassId > 0 AND p.Gender = 2 THEN 1 ELSE 0 END) AS a7_13maleSch
   ,SUM(CASE WHEN age.val >= 7 AND age.val <= 13 AND sc.BasicClassId > 0 AND p.Gender = 1 THEN 1 ELSE 0 END) AS a7_13femaleSch
   ,SUM(CASE WHEN age.val >= 14 AND age.val <= 18 AND p.Gender = 2 THEN 1 ELSE 0 END) AS a14_18male
   ,SUM(CASE WHEN age.val >= 14 AND age.val <= 18 AND p.Gender = 1 THEN 1 ELSE 0 END) AS a14_18female
 FROM
	student.StudentClass sc
	INNER JOIN core.Person p ON p.PersonID = sc.PersonId
	INNER JOIN core.InstitutionSchoolYear isy ON isy.InstitutionId = sc.InstitutionID
                                  				 AND isy.SchoolYear = sc.SchoolYear
                                                 AND isy.IsCurrent = 1
	INNER JOIN noms.DetailedSchoolType dst ON isy.DetailedSchoolTypeID = dst.DetailedSchoolTypeID
	INNER JOIN inst_nom.ClassType ct ON ct.ClassTypeID = sc.ClassTypeID
	OUTER APPLY (SELECT val = cioo.fn_CalcAgeByYear(p.BirthDate,sc.SchoolYear)) AS age
 WHERE
	sc.IsCurrent = 1
	AND sc.IsNotPresentForm = 0
	AND sc.PositionId NOT IN (7,10)
	AND dst.InstType IN (1,2) 
	AND ct.ClassKind = 1
 GROUP BY
	sc.InstitutionId
)

SELECT
	rc.InstRegId AS RegionId, --да не се показва в справката
	rc.InstRegName AS RegionName, ---'Област'
	rc.InstMunId AS MunicipalityId, --да не се показва в справката
	rc.InstMunName AS MunicipalityName, --'Община'
    rc.InstTownId AS TownID,
	rc.InstTownName AS TownName, --'Населено място'
	rc.InstId AS InstitutionID, --'Код по НЕИСПУО'
	rc.InstName AS InstitutionName, --'Наименование на институция'
	CASE rc.InstType
		 WHEN 1 THEN 'Училище'
		 WHEN 2 THEN 'ДГ'
	END AS InstitutionKind, -- 'Вид институция',
	(SELECT	cte.classesCounted
	 FROM	cte
	 WHERE	cte.InstitutionID = rc.InstId
	) AS GroupsCount, ---'Общ брой групи/паралелки',
	stdsByAge.allStds AS AllStds, ---'Общ брой деца/ученици',
	stdsByAge.a1_3male AS A1_3male, ---'Брой момчета 1-3 години',
	stdsByAge.a1_3female AS A1_3female, --- 'Брой момичета 1-3 години',
	stdsByAge.a4_7malePG AS A4_7malePG, ---'Брой момчета 4-7 години в ПГ',
	stdsByAge.a4_7femalePG AS A4_7femalePG, ---'Брой момичета 4-7 години в ПГ',
	stdsByAge.a7_13maleSch AS A7_13maleSch, ---'Брой момчета 7-13 години в училище',
	stdsByAge.a7_13femaleSch AS A7_13femaleSch, ---'Брой момичета 7-13 години в училище',
	stdsByAge.a14_18male AS A14_18male, ---'Брой момчета 14-18 години',
	stdsByAge.a14_18female AS A14_18female ---'Брой момичета 14-18 години'
FROM
	cioo.CIOOReportCommon_CurrData rc
	INNER JOIN stdsByAge ON stdsByAge.InstitutionID = rc.InstId
WHERE
	rc.InstType IN (1,2)
GROUP BY
	rc.InstRegId,
	rc.InstRegName,
	rc.InstMunId,
	rc.InstMunName,
    rc.InstTownId,
	rc.InstTownName,
	rc.InstId,
	rc.InstName,
	rc.InstType,
	allStds,
	a1_3male,
	a1_3female,
	a4_7malePG,
	a4_7femalePG,
	a7_13maleSch,
	a7_13femaleSch,
	a14_18male,
	a14_18female
`);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {}
}
