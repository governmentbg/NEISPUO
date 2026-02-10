import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRRegularFamilyReasonAbsencesView1757074404000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Regular_Family_Reason_Absences]
		AS
    WITH DailyAbsences AS (
        SELECT 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName AS ClassOrGroupName,
            CAST(a.[Date] AS DATE) AS [Day],
            DATEPART(MONTH, a.[Date]) AS [Month],
            a.Term,
            COUNT(*) AS AbsencesPerDay
        FROM [school_books].[Absence] a WITH (NOLOCK)
        INNER JOIN [school_books].[ClassBook] cb WITH (NOLOCK)
            ON a.SchoolYear = cb.SchoolYear 
            AND a.ClassBookId = cb.ClassBookId
        WHERE cb.IsValid = 1
            AND a.ExcusedReasonId = 2
        GROUP BY 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName,
            CAST(a.[Date] AS DATE),
            DATEPART(MONTH, a.[Date]),
            a.Term
    ),
    AggregatedTotals AS (
        SELECT 
            InstId,
            SchoolYear,
            ClassOrGroupName,
            [Day],
            [Month],
            Term,
            AbsencesPerDay,
            SUM(AbsencesPerDay) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName, [Month]
            ) AS AbsencesPerMonth,
            SUM(AbsencesPerDay) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesPerYear,
            SUM(CASE WHEN Term = 1 THEN AbsencesPerDay ELSE 0 END) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesTerm1,
            SUM(CASE WHEN Term = 2 THEN AbsencesPerDay ELSE 0 END) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesTerm2
        FROM DailyAbsences
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
        ag.[Day],
        ag.[Month],
        ag.Term,
        ag.AbsencesPerDay,
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
       WHERE SchemaName='${SysSchemaEnum.R_REGULAR_FAMILY_REASON_ABSENCES}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Regular_Family_Reason_Absences]`);
  }
}
