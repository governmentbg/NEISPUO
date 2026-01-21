namespace SB.ExtApi.IntegrationTests;

using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

public static class DataQueries
{
    public static async Task<int> ClassIdWithNoClassBookOrDefaultAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId
            };
        string sql = """
            SELECT
                [ClassId]
            FROM
                [inst_year].[ClassGroup] cg
            WHERE
                cg.SchoolYear = @SchoolYear AND
                cg.InstitutionId = @InstId AND
                cg.IsCombined = 0 AND
                cg.ParentClassId IS NULL AND
                NOT EXISTS (
                    SELECT 1
                    FROM [school_books].[ClassBook] cbi
                    WHERE
                        cbi.InstId = cg.InstitutionId AND
                        cbi.SchoolYear = cg.SchoolYear AND
                        cbi.ClassId = cg.ClassId
                )
            """;
        return (await connection.QueryAsync<int>(sql, parameters)).FirstOrDefault();
    }

    public static async Task<(int, int, bool)> ClassIdOnChildLevelOrDefaultAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId
            };
        string sql = $"""
            SELECT
               cg.ClassId,
               cg.ParentClassId,
               cb.ClassIsLvl2
            FROM
                [inst_year].[ClassGroup] cg
            INNER JOIN
                [school_books].[ClassBook] cb ON 
                cb.InstId = cg.InstitutionId AND
                cb.SchoolYear = cg.SchoolYear AND
                cb.IsValid = 1 AND
                (
                    (cb.ClassIsLvl2 = 1 AND cb.ClassId = cg.ClassId) OR
                    (cb.ClassIsLvl2 = 0 AND cb.ClassId = cg.ParentClassId)
                )
            WHERE
                cg.SchoolYear = @SchoolYear AND
                cg.InstitutionId = @InstId AND
                cg.IsCombined = 1
            """;
        return (await connection.QueryAsync<(int, int, bool)>(sql, parameters)).FirstOrDefault();
    }

    public class StudentDataTestIdsQO
    {
        public int ClassBookId { get; set; }
        public int ClassId { get; set; }
        public int PersonId { get; set; }
        public bool IsTransferred { get; set; }
    }
    public static async Task<StudentDataTestIdsQO[]> GetStudentDataIdsAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId,
        ClassBookType bookType)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId
            };

        // Get data from all levels
        string sql = $"""
            SELECT
                cb.ClassBookId,
                ClassId = CASE WHEN cb.ClassIsLvl2 = 1 THEN sc.ClassId ELSE cg.ClassId END,
                sc.PersonId,
                IsTransferred =
                    CASE
                        WHEN sc.Status = 1 THEN CAST(0 AS BIT)
                        ELSE CAST(1 AS BIT)
                    END
            FROM
                [school_books].[ClassBook] cb
                LEFT JOIN [inst_year].[ClassGroup] cg ON cb.ClassIsLvl2 = 0 AND cb.SchoolYear = cg.SchoolYear AND cb.ClassId = cg.ParentClassId
                INNER JOIN [student].[StudentClass] sc ON cb.SchoolYear = sc.SchoolYear AND (cb.ClassId = sc.ClassId OR cg.ClassId = sc.ClassId)
            WHERE
                cb.SchoolYear = @SchoolYear AND
                cb.InstId = @InstId AND
                cb.IsValid = 1 AND
                cb.BookType = {(int)bookType} AND
                (
                    (cb.ClassIsLvl2 = 1 AND EXISTS (
                        SELECT 1
                        FROM [inst_year].[ClassGroup] cg
                        INNER JOIN [inst_year].[CurriculumClass] cc ON cg.ParentClassId = cc.ClassId
                        WHERE cb.ClassId = cg.ClassId AND cc.IsValid = 1
                    )) OR
                    (cb.ClassIsLvl2 = 0 AND EXISTS (
                        SELECT 1
                        FROM [inst_year].[CurriculumClass] cc
                        WHERE cb.ClassId = cc.ClassId AND cc.IsValid = 1
                    ))
                ) AND NOT EXISTS (
                    SELECT 1
                    FROM [school_books].[Topic] t
                    WHERE
                        t.SchoolYear = cb.SchoolYear AND
                        t.ClassBookId = cb.ClassBookId
                )
            """;
        return (await connection.QueryAsync<StudentDataTestIdsQO>(sql, parameters)).ToArray();
    }

    public class ScheduleDataTestIdsQO
    {
        public int ShiftId { get; set; }
        public int ScheduleLessonId { get; set; }
        public DateTime Date { get; set; }
        public int CurriculumId { get; set; }
    }
    public static async Task<ScheduleDataTestIdsQO[]> GetScheduleDataIdsAsync(
        SqlConnection connection,
        int schoolYear,
        int classBookId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                ClassBookId = classBookId
            };

        string sql = """
            SELECT TOP 2
                s.ShiftId,
                sl.ScheduleLessonId,
                sl.Date,
                sl.CurriculumId
            FROM
                [school_books].[ScheduleLesson] sl
                INNER JOIN [school_books].[Schedule] s ON sl.SchoolYear = s.SchoolYear AND sl.ScheduleId = s.ScheduleId
            WHERE
                s.SchoolYear = @SchoolYear AND
                s.ClassBookId = @ClassBookId AND NOT EXISTS (
                    -- skip schedule lessons with student absences to test absence creation
                    SELECT 1
                    FROM [school_books].[Absence] a
                    WHERE
                        a.SchoolYear = sl.SchoolYear AND
                        a.ScheduleLessonId = sl.ScheduleLessonId
                )
            """;
        return (await connection.QueryAsync<ScheduleDataTestIdsQO>(sql, parameters)).ToArray();
    }

    public class GetUnusedScheduleLessonIdsQO
    {
        public int ScheduleLessonId { get; set; }
        public DateTime Date { get; set; }
    }
    public static async Task<GetUnusedScheduleLessonIdsQO[]> GetUnusedScheduleLessonIdsAsync(
        SqlConnection connection,
        int schoolYear,
        int classBookId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                ClassBookId = classBookId
            };

        string sql = """
            WITH UsedScheduleLessonIds AS (
                SELECT SchoolYear, ScheduleLessonId FROM school_books.Absence
                UNION SELECT SchoolYear, ScheduleLessonId FROM school_books.Grade
                UNION SELECT SchoolYear, ScheduleLessonId FROM school_books.Topic
                UNION SELECT SchoolYear, ScheduleLessonId FROM school_books.TeacherAbsenceHour
            )
            SELECT TOP 2
                sl.ScheduleLessonId,
                sl.Date
            FROM
                [school_books].[ScheduleLesson] sl
                INNER JOIN [school_books].[Schedule] s ON sl.SchoolYear = s.SchoolYear AND sl.ScheduleId = s.ScheduleId
            WHERE
                s.SchoolYear = @SchoolYear AND
                s.ClassBookId = @ClassBookId AND
                NOT EXISTS (
                    SELECT 1
                    FROM UsedScheduleLessonIds u
                    WHERE sl.SchoolYear = u.SchoolYear AND sl.ScheduleLessonId = u.ScheduleLessonId
                )
            """;
        return (await connection.QueryAsync<GetUnusedScheduleLessonIdsQO>(sql, parameters)).ToArray();
    }

    public class GetCurriculumDataIdsQO
    {
        public int CurriculumId { get; set; }
        public int SubjectId { get; set; }
    }
    public static async Task<GetCurriculumDataIdsQO[]> GetCurriculumDataIdsAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId,
        int studentClassId,
        int studentPersonId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ClassId = studentClassId,
                PersonId = studentPersonId
            };

        string sql = """
            SELECT
                cc.CurriculumId,
                c.SubjectId
            FROM
                [inst_year].[ClassGroup] cg
                INNER JOIN [inst_year].[CurriculumClass] cc ON cg.[ParentClassId] = cc.[ClassId]
                INNER JOIN [inst_year].[CurriculumStudent] cs ON cc.[CurriculumId] = cs.[CurriculumId]
                INNER JOIN [inst_year].[Curriculum] c ON cc.[CurriculumId] = c.[CurriculumId]
            WHERE
                cg.SchoolYear = @SchoolYear AND
                cg.InstitutionId = @InstId AND
                cg.ClassId = @ClassId AND
                cs.PersonId = @PersonId AND
                cc.IsValid = 1 AND
                cs.IsValid = 1 AND
                c.IsValid = 1
            """;
        return (await connection.QueryAsync<GetCurriculumDataIdsQO>(sql, parameters)).ToArray();
    }

    public static async Task<int[]> GetCurriculumDataIdsAsync(
        SqlConnection connection,
        int classId)
    {
        var parameters =
            new
            {
                ClassId = classId
            };

        string sql = """
            SELECT
                cc.CurriculumId
            FROM
                [inst_year].[CurriculumClass] cc
            WHERE
                cc.ClassId = @ClassId AND cc.IsValid = 1
            """;
        return (await connection.QueryAsync<int>(sql, parameters)).ToArray();
    }

    public static async Task<int[]> GetSupportTeacherIdsAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId
            };

        string sql = """
            SELECT TOP 2
                sp.PersonID
            FROM
                [inst_basic].[StaffPosition] sp
            WHERE
                sp.InstitutionId = @InstId AND sp.CurrentlyValid = 1
            """;
        return (await connection.QueryAsync<int>(sql, parameters)).ToArray();
    }

    public static async Task<int> GetTeacherAbsenceTeacherIdAsync(
        SqlConnection connection,
        int schoolYear,
        int institutionId,
        int scheduleLessonId)
    {
        var parameters =
            new
            {
                SchoolYear = schoolYear,
                InstId = institutionId,
                ScheduleLessonId = scheduleLessonId
            };

        string sql = """
            SELECT
                sp.PersonID
            FROM
                [school_books].[ScheduleLesson] sl
                INNER JOIN [inst_year].[CurriculumTeacher] ct ON sl.[CurriculumId] = ct.[CurriculumId]
                INNER JOIN [inst_basic].[StaffPosition] sp ON ct.[StaffPositionId] = sp.[StaffPositionId]
            WHERE
                sp.InstitutionId = @InstId AND
                sl.SchoolYear = @SchoolYear AND
                sl.ScheduleLessonId = @ScheduleLessonId AND
                ct.IsValid = 1
            """;
        return (await connection.QueryAsync<int>(sql, parameters)).FirstOrDefault();
    }
}
