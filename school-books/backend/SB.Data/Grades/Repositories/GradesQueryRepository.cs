namespace SB.Data;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IGradesQueryRepository;

internal class GradesQueryRepository : Repository, IGradesQueryRepository
{
    public GradesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetCurriculumStudentsVO[]> GetCurriculumStudentsAsync(int schoolYear, int classBookId, int curriculumId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        var isProfilingCurriculum = Grade.SubjectTypeIsProfilingSubject(
            await (
                from c in this.DbContext.Set<Curriculum>()

                where c.SchoolYear == schoolYear &&
                      c.CurriculumId == curriculumId
                select c.SubjectTypeId)
            .FirstOrDefaultAsync(ct)
        );

        return await (
            from sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            join cs in this.DbContext.Set<CurriculumStudent>().Where(cs => cs.CurriculumId == curriculumId && cs.IsValid)
            on p.PersonId equals cs.PersonId
            into j1 from cs in j1.DefaultIfEmpty()

            join gs in this.DbContext.Set<ClassBookStudentGradeless>().Where(gs => gs.SchoolYear == schoolYear && gs.ClassBookId == classBookId && gs.CurriculumId == curriculumId)
            on sc.PersonId equals gs.PersonId
            into j2 from gs in j2.DefaultIfEmpty()

            join ss in this.DbContext.Set<ClassBookStudentSpecialNeeds>().Where(ss => ss.SchoolYear == schoolYear && ss.ClassBookId == classBookId && ss.CurriculumId == curriculumId)
            on sc.PersonId equals ss.PersonId
            into j3 from ss in j3.DefaultIfEmpty()

            orderby (sc.ClassNumber == null ? 99999 : sc.ClassNumber), p.FirstName, p.MiddleName, p.LastName

            select new GetCurriculumStudentsVO(
                sc.PersonId,
                sc.ClassNumber,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sc.IsTransferred,
                (cs == null || !cs.IsValid) && !isProfilingCurriculum,
                (bool?)gs.WithoutFirstTermGrade ?? false,
                (bool?)gs.WithoutSecondTermGrade ?? false,
                (bool?)gs.WithoutFinalGrade ?? false,
                ss != null)
        ).ToArrayAsync(ct);
    }

    public async Task<GetCurriculumGradesVO[]> GetCurriculumGradesAsync(int schoolYear, int classBookId, int curriculumId, CancellationToken ct)
    {
        string sql = """
            SELECT
                g.GradeId,
                g.PersonId,
                g.Date,
                g.Type,
                g.Term,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                g.CreateDate,
                g.CreatedBySysUserId
            FROM
                [school_books].[Grade] g
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId AND
                g.CurriculumId = @curriculumId
            ORDER BY
                g.Date,
                g.CreateDate
            """;
        return await this.DbContext.Set<GetCurriculumGradesVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("curriculumId", curriculumId))
            .ToArrayAsync(ct);
    }

    public async Task<GetCurriculumVO> GetCurriculumAsync(int schoolYear, int classBookId, int curriculumId, CancellationToken ct)
    {
        var classBook = await (
            from cb in this.DbContext.Set<ClassBook>()
            where cb.SchoolYear == schoolYear &&
                cb.ClassBookId == classBookId
            select new
            {
                cb.ClassId,
                cb.ClassIsLvl2
            }
        ).SingleAsync(ct);

        return await (
            from cc in this.DbContext.CurriculumClassForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)

            join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

            join cg in this.DbContext.Set<ClassBookCurriculumGradeless>().Where(cg => cg.SchoolYear == schoolYear && cg.ClassBookId == classBookId)
            on c.CurriculumId equals cg.CurriculumId
            into j1 from cg in j1.DefaultIfEmpty()

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where c.CurriculumId == curriculumId &&
                cg == null &&
                cc.IsValid &&
                c.IsValid

            select new GetCurriculumVO(
                c.CurriculumId,
                s.SubjectName,
                s.SubjectNameShort,
                (c.CurriculumGroupNum ?? 0) == 0 ? null : c.CurriculumGroupNum.ToString(),
                st.SubjectTypeId,
                st.Name)
        ).SingleAsync(ct);
    }

    public async Task<GetVO> GetAsync(int schoolYear, int classBookId, int gradeId, CancellationToken ct)
    {
        string sql = """
            SELECT
                g.GradeId,
                s.SubjectName,
                s.SubjectNameShort,
                st.Name AS SubjectTypeName,
                g.Date,
                g.Type,
                g.Term,
                g.Category,
                g.DecimalGrade,
                g.QualitativeGrade,
                g.SpecialGrade,
                g.CreateDate,
                g.Comment,
                g.CreatedBySysUserId,
                p.FirstName AS CreatedBySysUserFirstName,
                p.MiddleName AS CreatedBySysUserMiddleName,
                p.LastName AS CreatedBySysUserLastName,
                g.IsReadFromParent
            FROM
                [school_books].[Grade] g
                INNER JOIN [inst_year].[Curriculum] c ON g.[CurriculumId] = c.[CurriculumId]
                INNER JOIN [inst_nom].[Subject] s ON c.[SubjectId] = s.[SubjectId]
                INNER JOIN [inst_nom].[SubjectType] st ON c.[SubjectTypeId] = st.[SubjectTypeId]
                INNER JOIN [core].[SysUser] su ON g.[CreatedBySysUserId] = su.[SysUserId]
                INNER JOIN [core].[Person] p ON su.[PersonId] = p.[PersonId]
            WHERE
                g.SchoolYear = @schoolYear AND
                g.ClassBookId = @classBookId AND
                g.GradeId = @gradeId
            """;

        return await this.DbContext.Set<GetVO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("classBookId", classBookId),
                new SqlParameter("gradeId", gradeId))
            .SingleAsync(ct);
    }

    public async Task<GetProfilingSubjectForecastGradesVO[]> GetProfilingSubjectForecastGradesAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        var childCurriculumIds = await this.DbContext.Set<Curriculum>()
            // curriculums are bugged and sometimes the parent has itself as parentCurriculumId
            .Where(c => c.ParentCurriculumID == curriculumId && c.CurriculumId != curriculumId)
            .Select(c => c.CurriculumId)
            .ToArrayAsync(ct);

        var childCurriculumFinalGrades = await (
            from g in this.DbContext.Set<GradeDecimal>()
            where
                g.SchoolYear == schoolYear &&
                g.ClassBookId == classBookId &&
                g.Type == GradeType.Final &&
                this.DbContext.MakeIdsQuery(childCurriculumIds).Any(id => g.CurriculumId == id.Id)
            select g
        ).ToArrayAsync(ct);

        return childCurriculumFinalGrades
            .GroupBy(g => g.PersonId)
            .Select(gr => new GetProfilingSubjectForecastGradesVO(
                gr.Key,
                gr.Any(g => g.DecimalGrade < 3m) ? 2m : gr.Select(g => g.DecimalGrade).Average()))
            .WhereNotNull()
            .ToArray();
    }

    public async Task<bool> ExistsVerifiedScheduleLessonAsync(
        int schoolYear,
        int scheduleLessonId,
        CancellationToken ct)
    {
        return await (
            from sl in this.DbContext.Set<ScheduleLesson>()
            where sl.SchoolYear == schoolYear &&
                sl.ScheduleLessonId == scheduleLessonId &&
                sl.IsVerified
            select sl
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsVerifiedScheduleLessonForGradeAsync(
        int schoolYear,
        int gradeId,
        CancellationToken ct)
    {
        return await (
            from g in this.DbContext.Set<Grade>()

            join sl in this.DbContext.Set<ScheduleLesson>()
            on new { g.SchoolYear, g.ScheduleLessonId } equals new { sl.SchoolYear, ScheduleLessonId = (int?)sl.ScheduleLessonId }

            where g.SchoolYear == schoolYear &&
                g.GradeId == gradeId &&
                sl.IsVerified

            select g
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsFinalGradeForStudentAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        CancellationToken ct)
    {
        return await (
            from g in this.DbContext.Set<Grade>()

            where g.SchoolYear == schoolYear &&
                g.ClassBookId == classBookId &&
                g.PersonId == personId &&
                g.CurriculumId == curriculumId &&
                g.Type == GradeType.Final
            select g
        ).AnyAsync(ct);
    }

    public async Task<bool> ExistsTermGradeForStudentAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        SchoolTerm term,
        CancellationToken ct)
    {
        return await (
            from g in this.DbContext.Set<Grade>()

            where g.SchoolYear == schoolYear &&
                g.ClassBookId == classBookId &&
                g.PersonId == personId &&
                g.CurriculumId == curriculumId &&
                g.Term == term &&
                (g.Type == GradeType.Term || g.Type == GradeType.OtherClassTerm || g.Type == GradeType.OtherSchoolTerm)
            select g
        ).AnyAsync(ct);
    }
}
