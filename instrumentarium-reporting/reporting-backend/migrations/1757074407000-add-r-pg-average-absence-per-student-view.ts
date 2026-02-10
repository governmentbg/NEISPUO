import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRPGAverageAbsencePerStudentView1757074407000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_PG_Average_Absence_Per_Student]
		AS
    WITH AbsencesCTE AS (
        SELECT
            r.[RegionID],
            r.[Name] AS RegionName,
            m.[Name] AS MunicipalityName,
            t.[Name] AS TownName,
            cb.SchoolYear,
            i.InstitutionID,
            i.[Name] AS InstitutionName,
            i.DetailedSchoolTypeID,
            i.BudgetingSchoolTypeID,
            i.FinancialSchoolTypeID,
            p.PersonID,
            p.FirstName,
            p.MiddleName,
            p.LastName,
            cb.ClassBookId,
            cb.FullBookName AS ClassOrGroupName,
            SUM(CASE WHEN a.Type = 2 THEN 1 ELSE 0 END) AS TotalUnexcusedAbsences,
            SUM(CASE WHEN a.Type = 3 AND a.ExcusedReasonId = 1 AND a.HisMedicalNoticeId IS NULL THEN 1 ELSE 0 END) AS ExcusedByMedicalNoticeAbsence,
            SUM(CASE WHEN a.Type = 3 AND a.ExcusedReasonId = 1 AND a.HisMedicalNoticeId IS NOT NULL THEN 1 ELSE 0 END) AS ExcusedByHealthReasonAbsence,
            SUM(CASE WHEN a.Type = 3 AND a.ExcusedReasonId = 2 THEN 1 ELSE 0 END) AS ExcusedByFamilyReasonAbsence,
            SUM(CASE WHEN a.Type = 3 AND (a.ExcusedReasonId = 3 OR a.ExcusedReasonId IS NULL) AND a.HisMedicalNoticeId IS NULL THEN 1 ELSE 0 END) AS ExcusedByOtherReasonAbsence,
            SUM(CASE WHEN a.Type = 3 THEN 1 ELSE 0 END) AS TotalExcusedAbsences
        FROM [school_books].[Attendance] a
        INNER JOIN [school_books].[ClassBook] cb ON a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId
        INNER JOIN [school_books].[ClassBookSchoolYearSettings] cbsys ON cbsys.SchoolYear = cb.SchoolYear AND cbsys.ClassBookId = cb.ClassBookId
        INNER JOIN [inst_year].[ClassGroup] cg 
            ON (CASE WHEN cb.ClassIsLvl2 = 1 THEN cg.ClassId ELSE cg.ParentClassId END) = cb.ClassId 
           AND cg.SchoolYear = cb.SchoolYear 
           AND cg.InstitutionID = cb.InstId
        INNER JOIN [student].[StudentClass] sc 
            ON sc.SchoolYear = cb.SchoolYear 
           AND sc.InstitutionID = cb.InstId 
           AND sc.ClassId = cg.ClassID 
           AND sc.PersonId = a.PersonId
           AND sc.IsNotPresentForm = 0
           AND sc.Status = 1
        INNER JOIN [core].[Person] p ON p.PersonID = a.PersonId
        INNER JOIN [core].[Institution] i ON cb.InstId = i.InstitutionID
        INNER JOIN [location].[Town] t ON i.TownID = t.TownID
        INNER JOIN [location].[Municipality] m ON t.MunicipalityID = m.MunicipalityID
        INNER JOIN [location].[Region] r ON m.RegionID = r.RegionID
        
        WHERE 
            cb.IsValid = 1
            AND cb.BookType = 1
            AND a.Date >= cbsys.SchoolYearStartDate
            AND a.Date <= cbsys.SchoolYearEndDate
        
        GROUP BY 
            r.RegionID, r.[Name], m.[Name], t.[Name],
            cb.SchoolYear, i.InstitutionID, i.[Name],
            i.DetailedSchoolTypeID, i.BudgetingSchoolTypeID, i.FinancialSchoolTypeID,
            p.PersonID, p.FirstName, p.MiddleName, p.LastName,
            cb.ClassBookId, cb.FullBookName
    )
        
    SELECT * FROM AbsencesCTE
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_PG_AVERAGE_ABSENCE_PER_STUDENT}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_PG_Average_Absence_Per_Student]`);
  }
}
