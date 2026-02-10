import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRPGFamilyReasonAbsencesView1757074403000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_PG_Family_Reason_Absences]
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
            AND cb.BookType  = 1
            AND a.[Type] = 3 
            AND a.ExcusedReasonId = 2
        GROUP BY 
            cb.InstId,
            cb.SchoolYear,
            cb.FullBookName,
            CAST(a.[Date] AS DATE),
            DATEPART(MONTH, a.[Date])
    ),
    MonthlyYearlyTotals AS (
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
        mt.SchoolYear,
        mt.ClassOrGroupName,
        mt.[Day],
        mt.[Month],
        mt.AbsencesPerDay,
        mt.AbsencesPerMonth,
        mt.AbsencesPerYear
    FROM MonthlyYearlyTotals mt
    INNER JOIN [core].[Institution] i WITH (NOLOCK) ON mt.InstId = i.InstitutionID
    INNER JOIN [location].[Town] t WITH (NOLOCK) ON i.TownID = t.TownID
    INNER JOIN [location].[Municipality] m WITH (NOLOCK) ON t.MunicipalityID = m.MunicipalityID
    INNER JOIN [location].[Region] r WITH (NOLOCK) ON m.RegionID = r.RegionID;
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_PG_FAMILY_REASON_ABSENCES}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_PG_Family_Reason_Absences]`);
  }
}
