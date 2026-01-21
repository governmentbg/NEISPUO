import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRRegularAbsencesPerMonthView1757074401000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Regular_Absences_Per_Month]
		AS
    WITH MonthlyAbsences AS (
        SELECT 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName AS ClassOrGroupName,
            DATEPART(MONTH, a.[Date]) AS [Month],
            a.Term,
            SUM(CASE 
                WHEN a.Type = 1 THEN 0.5 
                WHEN a.Type = 2 THEN 1 
                ELSE 0 
            END) AS AbsencesPerMonth
        FROM [school_books].[Absence] a WITH (NOLOCK)
        INNER JOIN [school_books].[ClassBook] cb WITH (NOLOCK)
            ON a.SchoolYear = cb.SchoolYear 
            AND a.ClassBookId = cb.ClassBookId
        WHERE cb.IsValid = 1
            AND a.Type IN (1, 2)
        GROUP BY 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName,
            DATEPART(MONTH, a.[Date]),
            a.Term
    ),
    AggregatedTotals AS (
        SELECT 
            InstId,
            SchoolYear,
            ClassOrGroupName,
            [Month],
            Term,
            AbsencesPerMonth,
            SUM(CASE WHEN Term = 1 THEN AbsencesPerMonth ELSE 0 END) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesTerm1,
            SUM(CASE WHEN Term = 2 THEN AbsencesPerMonth ELSE 0 END) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesTerm2,
            SUM(AbsencesPerMonth) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesPerYear
        FROM MonthlyAbsences
    )
    SELECT 
        r.[RegionID],
        r.[Name] AS RegionName,
        m.[Name] AS MunicipalityName,
        t.[Name] AS TownName,
        i.InstitutionID,
        i.[Name] AS InstitutionName,
        i.DetailedSchoolTypeID,
        i.BudgetingSchoolTypeID,
        i.FinancialSchoolTypeID,
        ag.SchoolYear,
        ag.ClassOrGroupName,
        ag.[Month],
        ag.Term,
        ag.AbsencesPerMonth,
        ag.AbsencesTerm1,
        ag.AbsencesTerm2,
        ag.AbsencesPerYear
    FROM AggregatedTotals ag
    INNER JOIN [core].[Institution] i WITH (NOLOCK) ON ag.InstId = i.InstitutionID
    INNER JOIN [location].[Town] t WITH (NOLOCK) ON i.TownID = t.TownID
    INNER JOIN [location].[Municipality] m WITH (NOLOCK) ON t.MunicipalityID = m.MunicipalityID
    INNER JOIN [location].[Region] r WITH (NOLOCK) ON m.RegionID = r.RegionID;
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_REGULAR_ABSENCES_PER_MONTH}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Regular_Absences_Per_Month]`);
  }
}
