import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRTwentyFivePercentageAbsencesView1757074400000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Twenty_Five_Percentage_Absences]
	AS
	WITH InstitutionData AS (
        SELECT 
            r.Code,
            r.RegionID,
            r.Name AS RegionName,
            m.Name AS MunicipalityName,
            t.Name AS TownName,
            i.InstitutionID,
            i.Name AS InstitutionName,
            i.DetailedSchoolTypeID,
            i.BudgetingSchoolTypeID,
            i.FinancialSchoolTypeID
        FROM [core].[Institution] i
        INNER JOIN [location].[Town] t ON i.TownID = t.TownID
        INNER JOIN [location].[Municipality] m ON t.MunicipalityID = m.MunicipalityID
        INNER JOIN [location].[Region] r ON r.RegionID = m.RegionID
    ),
    CurriculumsData AS (
        SELECT
            c.CurriculumPartID,
            s.SubjectName,
            st.Name AS SubjectTypeName,
            c.CurriculumID,
            (c.WeeksFirstTerm * c.HoursWeeklyFirstTerm) AS TotalHours_Term1,
            (c.WeeksSecondTerm * c.HoursWeeklySecondTerm) AS TotalHours_Term2,
            (c.WeeksFirstTerm * c.HoursWeeklyFirstTerm + c.WeeksSecondTerm * c.HoursWeeklySecondTerm) AS TotalHours_WholeYear
        FROM [inst_year].[Curriculum] c
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectID = s.SubjectID
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeID = st.SubjectTypeID
    ),
    StudentAbsences AS (
        SELECT 
            p.PersonID,
            p.FirstName,
            p.MiddleName,
            p.LastName,
            p.PublicEduNumber,
            p.BirthDate,
            p.Gender,
            cb.SchoolYear,
            cb.InstId,
            cb.FullBookName,
            sl.CurriculumId,
            SUM(CASE WHEN a.Term = 1 AND a.Type = 1 THEN 0.5
                     WHEN a.Term = 1 AND a.Type IN (2, 3) THEN 1 ELSE 0 END) AS Absences_Term1,
            SUM(CASE WHEN a.Term = 2 AND a.Type = 1 THEN 0.5
                     WHEN a.Term = 2 AND a.Type IN (2, 3) THEN 1 ELSE 0 END) AS Absences_Term2
        FROM [school_books].[Absence] a
        INNER JOIN [school_books].[ClassBook] cb ON a.SchoolYear = cb.SchoolYear AND a.ClassBookId = cb.ClassBookId
        INNER JOIN [school_books].[ScheduleLesson] sl ON a.SchoolYear = sl.SchoolYear AND a.ScheduleLessonId = sl.ScheduleLessonId
        INNER JOIN [core].[Person] p ON a.PersonId = p.PersonID
        WHERE
    	    cb.IsValid = 1 
            AND cb.BookType IN (2, 3, 4)
        GROUP BY 
            p.PersonID,
            p.FirstName,
            p.MiddleName,
            p.LastName,
            p.PublicEduNumber,
            p.BirthDate, p.Gender,
            cb.SchoolYear,
            cb.InstId,
            cb.FullBookName,
            sl.CurriculumId
    )
    SELECT
        i.RegionID,
        i.RegionName,
        i.MunicipalityName,
        i.TownName,
        i.InstitutionID,
        i.InstitutionName,
        i.DetailedSchoolTypeID,
        i.BudgetingSchoolTypeID,
        i.FinancialSchoolTypeID,
        sa.PersonID,
        sa.FirstName,
        sa.MiddleName,
        sa.LastName,
        sa.PublicEduNumber,
        sa.BirthDate,
        sa.Gender,
        sa.SchoolYear,
        sa.FullBookName AS ClassOrGroupName,
        sa.CurriculumId,
        ch.CurriculumPartID,
        ch.SubjectName,
        ch.SubjectTypeName,
        sa.Absences_Term1,
        ch.TotalHours_Term1,
        CASE 
            WHEN ch.TotalHours_Term1 > 0 
            THEN ROUND(sa.Absences_Term1 / ch.TotalHours_Term1 * 100.0, 0)
            ELSE 0
        END AS AbsencePercent_Term1,
        sa.Absences_Term2,
        ch.TotalHours_Term2,
        CASE 
            WHEN ch.TotalHours_Term2 > 0
            THEN ROUND(sa.Absences_Term2 / ch.TotalHours_Term2 * 100.0, 0)
            ELSE 0
        END AS AbsencePercent_Term2,
        sa.Absences_Term1 + sa.Absences_Term2 AS TotalAbsences,
        ch.TotalHours_WholeYear,
        ROUND((ISNULL(sa.Absences_Term1, 0) + ISNULL(sa.Absences_Term2, 0)) / NULLIF(ch.TotalHours_WholeYear, 0) * 100.0, 0) AS TotalPercent_WholeYear
    FROM StudentAbsences sa
    JOIN CurriculumsData ch ON sa.CurriculumId = ch.CurriculumID
    JOIN InstitutionData i ON sa.InstId = i.InstitutionID
    WHERE
        (ch.TotalHours_Term1 > 0 AND sa.Absences_Term1 / ch.TotalHours_Term1 * 100.0 >= 25) OR
        (ch.TotalHours_Term2 > 0 AND sa.Absences_Term2 / ch.TotalHours_Term2 * 100.0 >= 25);
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_TWENTY_FIVE_PERCENTAGE_ABSENCES}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Twenty_Five_Percentage_Absences]`);
  }
}
