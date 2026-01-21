namespace SB.Data;

using System.Linq;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal static partial class ClassBooksQueryHelper
{
    public static IQueryable<CurriculumClass> CurriculumClassForClassBook(this DbContext dbContext, int schoolYear, int classId, bool classIsLvl2)
    {
        if (!classIsLvl2)
        {
            return (
                from cc in dbContext.Set<CurriculumClass>()

                where cc.ClassId == classId

                select cc
            );
        }
        else
        {
            return (
                from cg in dbContext.Set<ClassGroup>()

                join cc in dbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId

                where cg.SchoolYear == schoolYear && cg.ClassId == classId

                select cc
            );
        }
    }

    public static IQueryable<ClassBooksForStudentsVO> ClassBooksForStudents(this DbContext dbContext, int schoolYear, int[] studentPersonIds)
    {
        var sql = $"""
                SELECT DISTINCT
                    sc.SchoolYear,
                    sc.InstId,
                    sc.ClassId,
                    sc.ClassIsLvl2,
                    sc.ClassBookId,
                    sc.BookType,
                    sc.BookName,
                    sc.FullBookName,
                    sc.BasicClassId,
                    sc.IsValid,
                    sc.PersonId,
                    sc.IsIndividualCurriculum,
                    sc.PersonStatus,
                    sc.StudentSpecialityId
                FROM (
                    SELECT
                        cb.SchoolYear,
                        cb.InstId,
                        cb.ClassId,
                        cb.ClassIsLvl2,
                        cb.ClassBookId,
                        cb.BookType,
                        cb.BookName,
                        cb.FullBookName,
                        cb.BasicClassId,
                        cb.IsValid,
                        sc.PersonId,
                        sc.IsIndividualCurriculum,
                        PersonStatus = sc.Status,
                        sc.StudentSpecialityId
                    FROM [school_books].[ClassBook] cb
                    JOIN [school_books].[ClassBookSchoolYearSettings] cbsy ON cb.SchoolYear = cbsy.SchoolYear AND cb.ClassBookId = cbsy.ClassBookId
                    JOIN [inst_year].[ClassGroup] cg ON cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassID
                    JOIN [student].[StudentClass] sc ON cg.SchoolYear = sc.SchoolYear AND cg.ClassId = sc.ClassId
                    WHERE sc.IsNotPresentForm = 0 AND
                        sc.SchoolYear = @schoolYear AND
                        cb.ClassIsLvl2 = 0 AND
                        sc.PersonId IN (SELECT [Id] FROM OPENJSON(@idsJsonArray) WITH ([Id] INT '$'))
                        AND (sc.Status = 1 OR (sc.Status = 2 AND sc.DischargeDate >= cbsy.SchoolYearStartDate))
                ) AS sc

                UNION ALL

                SELECT DISTINCT
                    sc.SchoolYear,
                    sc.InstId,
                    sc.ClassId,
                    sc.ClassIsLvl2,
                    sc.ClassBookId,
                    sc.BookType,
                    sc.BookName,
                    sc.FullBookName,
                    sc.BasicClassId,
                    sc.IsValid,
                    sc.PersonId,
                    sc.IsIndividualCurriculum,
                    sc.PersonStatus,
                    sc.StudentSpecialityId
                FROM (
                    SELECT
                        cb.SchoolYear,
                        cb.InstId,
                        cb.ClassId,
                        cb.ClassIsLvl2,
                        cb.ClassBookId,
                        cb.BookType,
                        cb.BookName,
                        cb.FullBookName,
                        cb.BasicClassId,
                        cb.IsValid,
                        sc.PersonId,
                        sc.IsIndividualCurriculum,
                        PersonStatus = sc.Status,
                        sc.StudentSpecialityId
                    FROM [school_books].[ClassBook] cb
                    JOIN [school_books].[ClassBookSchoolYearSettings] cbsy ON cb.SchoolYear = cbsy.SchoolYear AND cb.ClassBookId = cbsy.ClassBookId
                    JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND cb.ClassId = sc.ClassId
                    WHERE sc.IsNotPresentForm = 0 AND
                        sc.SchoolYear = @schoolYear AND
                        cb.ClassIsLvl2 = 1 AND
                        sc.PersonId IN (SELECT [Id] FROM OPENJSON(@idsJsonArray) WITH ([Id] INT '$')) AND
                        (sc.Status = 1 OR (sc.Status = 2 AND sc.DischargeDate >= cbsy.SchoolYearStartDate))
                ) AS sc
                """;

        return dbContext.Set<ClassBooksForStudentsVO>()
            .FromSqlRaw(
                sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("idsJsonArray", JsonSerializer.Serialize(studentPersonIds)));
    }

    public static IQueryable<StudentsForClassBookVO> StudentsForClassBook(this DbContext dbContext, int schoolYear, int classId, bool classIsLvl2)
    {
        string sql;
        if (classIsLvl2)
        {
            sql = """
                SELECT
                    sc.SchoolYear,
                    sc.ClassBookClassId,
                    sc.PersonId,
                    IsTransferred =
                        CASE
                            WHEN sc.Status = 1 THEN CAST(0 AS BIT)
                            ELSE CAST(1 AS BIT)
                        END,
                    sc.ClassNumber,
                    sc.StudentSpecialityId,
                    sc.IsIndividualCurriculum,
                    sc.AdmissionDocumentId,
                    sc.RelocationDocumentId,
                    sc.EnrollmentDate,
                    sc.DischargeDate
                FROM (
                    SELECT
                        sc.SchoolYear,
                        ClassBookClassId = sc.ClassId,
                        sc.PersonId,
                        sc.Status,
                        sc.ClassNumber,
                        sc.StudentSpecialityId,
                        sc.IsIndividualCurriculum,
                        sc.AdmissionDocumentId,
                        sc.RelocationDocumentId,
                        sc.EnrollmentDate,
                        sc.DischargeDate,
                        RowNumber =
                            ROW_NUMBER() OVER (
                                PARTITION BY sc.PersonId, sc.ClassId
                                ORDER BY
                                    CASE
                                        WHEN sc.Status = 1 THEN 1
                                        ELSE 2
                                    END
                            )
                    FROM student.StudentClass sc
                    WHERE sc.IsNotPresentForm = 0 AND
                        sc.SchoolYear = @schoolYear AND
                        sc.ClassId = @classId
                ) AS sc
                WHERE sc.RowNumber = 1
                """;
        }
        else
        {
            sql = """
                SELECT
                    sc.SchoolYear,
                    sc.ClassBookClassId,
                    sc.PersonId,
                    IsTransferred =
                        CASE
                            WHEN sc.Status = 1 THEN CAST(0 AS BIT)
                            ELSE CAST(1 AS BIT)
                        END,
                    sc.ClassNumber,
                    sc.StudentSpecialityId,
                    sc.IsIndividualCurriculum,
                    sc.AdmissionDocumentId,
                    sc.RelocationDocumentId,
                    sc.EnrollmentDate,
                    sc.DischargeDate
                FROM (
                    SELECT
                        sc.SchoolYear,
                        ClassBookClassId = cg.ParentClassId,
                        sc.PersonId,
                        sc.Status,
                        sc.ClassNumber,
                        sc.StudentSpecialityId,
                        sc.IsIndividualCurriculum,
                        sc.AdmissionDocumentId,
                        sc.RelocationDocumentId,
                        sc.EnrollmentDate,
                        sc.DischargeDate,
                        RowNumber =
                            ROW_NUMBER() OVER (
                                PARTITION BY sc.PersonId, cg.ParentClassId
                                ORDER BY
                                    CASE
                                        WHEN sc.Status = 1 THEN 1
                                        ELSE 2
                                    END
                            )
                    FROM inst_year.ClassGroup cg
                    JOIN student.StudentClass sc
                        ON cg.SchoolYear = sc.SchoolYear
                        AND cg.ClassId = sc.ClassId
                    WHERE sc.IsNotPresentForm = 0 AND
                        cg.SchoolYear = @schoolYear AND
                        cg.ParentClassId = @classId
                ) AS sc
                WHERE sc.RowNumber = 1
                """;
        }

        return dbContext.Set<StudentsForClassBookVO>()
            .FromSqlRaw(
                sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classId", classId));

    }
}
