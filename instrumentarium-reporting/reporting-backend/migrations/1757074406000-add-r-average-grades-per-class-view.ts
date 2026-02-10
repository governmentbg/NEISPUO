import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRAverageGradesPerClassView1757074406000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Average_Grades_Per_Class]
	AS
    WITH CombinedClassBooks AS (
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
            s.SubjectName,
            st.[Name] AS SubjectTypeName,
            p.PersonId,
            cb.ClassBookId,
            cb.FullBookName AS ClassOrGroupName,
            CASE WHEN g.Type = 21 AND g.Term = 1 THEN g.DecimalGrade END AS Term1Grade,
            CASE WHEN g.Type = 21 AND g.Term = 2 THEN g.DecimalGrade END AS Term2Grade,
            CASE WHEN g.Type = 22 THEN g.DecimalGrade END AS YearGrade
        FROM [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[CurriculumClass] cc ON cb.SchoolYear = cc.SchoolYear AND cb.ClassId = cc.ClassId AND cc.IsValid = 1
        INNER JOIN [inst_year].[CurriculumStudent] cs ON cs.SchoolYear = cc.SchoolYear AND cs.CurriculumID = cc.CurriculumID AND cs.IsValid = 1
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassID
        INNER JOIN [student].[StudentClass] sc ON cg.ClassID = sc.ClassId AND cs.PersonID = sc.PersonId AND sc.Status = 1 AND sc.IsNotPresentForm = 0
        INNER JOIN [core].[Person] p ON cs.PersonID = p.PersonID
        INNER JOIN [inst_year].[Curriculum] c ON cc.SchoolYear = c.SchoolYear AND cc.CurriculumId = c.CurriculumId AND c.IsValid = 1 AND c.SubjectTypeID != 40
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectId = s.SubjectId
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeId = st.SubjectTypeId
        LEFT JOIN [school_books].[Grade] g ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId AND cs.PersonID = g.PersonId AND cs.CurriculumID = g.CurriculumId AND g.Category = 1
        INNER JOIN [core].[Institution] i ON cb.InstId = i.InstitutionID
        INNER JOIN [location].[Town] t ON i.TownID = t.TownID
        INNER JOIN [location].[Municipality] m ON t.MunicipalityID = m.MunicipalityID
        INNER JOIN [location].[Region] r ON m.RegionID = r.RegionID
        WHERE  
            cb.IsValid = 1
            AND cb.BookType IN (3,4)
            AND cb.ClassIsLvl2 = 0
        
        UNION ALL
        
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
            s.SubjectName,
            st.[Name] AS SubjectTypeName,
            p.PersonId,
            cb.ClassBookId,
            cb.FullBookName AS ClassOrGroupName,
            CASE WHEN g.Type = 21 AND g.Term = 1 THEN g.DecimalGrade END AS Term1Grade,
            CASE WHEN g.Type = 21 AND g.Term = 2 THEN g.DecimalGrade END AS Term2Grade,
            CASE WHEN g.Type = 22 THEN g.DecimalGrade END AS YearGrade
        FROM [school_books].[ClassBook] cb
        INNER JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ClassId
        INNER JOIN [inst_year].[CurriculumClass] cc ON cc.SchoolYear = cg.SchoolYear AND cc.ClassId = cg.ParentClassId AND cc.IsValid = 1
        INNER JOIN [inst_year].[CurriculumStudent] cs ON cs.SchoolYear = cc.SchoolYear AND cs.CurriculumID = cc.CurriculumID AND cs.IsValid = 1
        INNER JOIN [student].[StudentClass] sc ON sc.ClassId = cg.ClassID AND cs.PersonID = sc.PersonId AND sc.Status = 1 AND sc.IsNotPresentForm = 0
        INNER JOIN [core].[Person] p ON cs.PersonID = p.PersonID
        INNER JOIN [inst_year].[Curriculum] c ON cc.SchoolYear = c.SchoolYear AND cc.CurriculumId = c.CurriculumId AND c.IsValid = 1 AND c.SubjectTypeID != 40
        INNER JOIN [inst_nom].[Subject] s ON c.SubjectId = s.SubjectId
        INNER JOIN [inst_nom].[SubjectType] st ON c.SubjectTypeId = st.SubjectTypeId
        LEFT JOIN [school_books].[Grade] g ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId AND cs.PersonID = g.PersonId AND cs.CurriculumID = g.CurriculumId AND g.Category = 1
        INNER JOIN [core].[Institution] i ON cb.InstId = i.InstitutionID
        INNER JOIN [location].[Town] t ON i.TownID = t.TownID
        INNER JOIN [location].[Municipality] m ON t.MunicipalityID = m.MunicipalityID
        INNER JOIN [location].[Region] r ON m.RegionID = r.RegionID
        WHERE 
            cb.IsValid = 1
            AND cb.BookType IN (3,4)
            AND cb.ClassIsLvl2 = 1
    )
    SELECT
        RegionID,
        RegionName,
        MunicipalityName,
        TownName,
        SchoolYear,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        SubjectName,
        SubjectTypeName,
        COUNT(DISTINCT PersonId) AS StudentsStudyingSubject,
        COUNT(DISTINCT CASE WHEN Term1Grade IS NOT NULL THEN PersonId END) AS StudentsWithTerm1Grades,
        COUNT(DISTINCT CASE WHEN Term2Grade IS NOT NULL THEN PersonId END) AS StudentsWithTerm2Grades,
        COUNT(DISTINCT CASE WHEN YearGrade IS NOT NULL THEN PersonId END) AS StudentsWithYearGrades,
        CASE 
            WHEN COUNT(DISTINCT PersonId) > 0 THEN 
                ROUND(CAST(COUNT(DISTINCT CASE WHEN Term1Grade IS NOT NULL THEN PersonId END) AS FLOAT) * 100.0 / COUNT(DISTINCT PersonId), 2)
            ELSE 0 
        END AS PercentStudentsWithTerm1Grades,
        CASE 
            WHEN COUNT(DISTINCT PersonId) > 0 THEN 
                ROUND(CAST(COUNT(DISTINCT CASE WHEN Term2Grade IS NOT NULL THEN PersonId END) AS FLOAT) * 100.0 / COUNT(DISTINCT PersonId), 2)
            ELSE 0 
        END AS PercentStudentsWithTerm2Grades,
        CASE 
            WHEN COUNT(DISTINCT PersonId) > 0 THEN 
                ROUND(CAST(COUNT(DISTINCT CASE WHEN YearGrade IS NOT NULL THEN PersonId END) AS FLOAT) * 100.0 / COUNT(DISTINCT PersonId), 2)
            ELSE 0 
        END AS PercentStudentsWithYearGrades,
        ROUND(AVG(Term1Grade), 2) AS AverageTerm1Grade,
        ROUND(AVG(Term2Grade), 2) AS AverageTerm2Grade,
        ROUND(AVG(YearGrade), 2) AS AverageYearGrade
        
    FROM CombinedClassBooks
        
    GROUP BY
        RegionID,
        RegionName,
        MunicipalityName,
        TownName,
        SchoolYear,
        InstitutionID,
        InstitutionName,
        ClassBookId,
        ClassOrGroupName,
        SubjectName,
        SubjectTypeName
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_AVERAGE_GRADES_PER_CLASS}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Average_Grades_Per_Class]`);
  }
}
