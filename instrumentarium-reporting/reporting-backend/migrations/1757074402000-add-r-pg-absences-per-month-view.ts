import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRPGAbsencesPerMonthView1757074402000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_PG_Absences_Per_Month]
		AS
    WITH DailyAbsences AS (
        SELECT 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName AS ClassOrGroupName,
            CAST(a.[Date] AS DATE) AS [Day],
            DATEPART(MONTH, a.[Date]) AS [Month],
            COUNT(*) AS AbsencesPerDay
        FROM [school_books].[Attendance] a WITH (NOLOCK)
        INNER JOIN [school_books].[ClassBook] cb WITH (NOLOCK)
            ON a.SchoolYear = cb.SchoolYear 
            AND a.ClassBookId = cb.ClassBookId
        WHERE cb.IsValid = 1
            AND cb.BookType = 1
            AND a.[Type] <> 1
        GROUP BY 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName,
            CAST(a.[Date] AS DATE),
            DATEPART(MONTH, a.[Date])
    ),
    AggregatedTotals AS (
        SELECT 
            InstId,
            SchoolYear,
            ClassOrGroupName,
            [Day],
            [Month],
            AbsencesPerDay,
            SUM(AbsencesPerDay) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName, [Month]
            ) AS AbsencesPerMonth,
            SUM(AbsencesPerDay) OVER (
                PARTITION BY InstId, SchoolYear, ClassOrGroupName
            ) AS AbsencesPerYear
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
        ag.AbsencesPerDay,
        ag.AbsencesPerMonth,
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
       WHERE SchemaName='${SysSchemaEnum.R_PG_ABSENCES_PER_MONTH}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_PG_Absences_Per_Month]`);
  }
}
