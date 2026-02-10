namespace SB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IRegularGradePointAverageByStudentsReportsQueryRepository;

class RegularGradePointAverageByStudentsReportsQueryRepository : Repository, IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public RegularGradePointAverageByStudentsReportsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<RegularGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.RegularGradePointAverageByStudentsReportId,
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int regularGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<RegularGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsReportId

            select new GetVO(
                r.SchoolYear,
                r.RegularGradePointAverageByStudentsReportId,
                r.Period,
                r.ClassBookNames,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int regularGradePointAverageByStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<RegularGradePointAverageByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsReportId

            orderby ri.RegularGradePointAverageByStudentsReportItemId

            select new GetItemsVO(
                ri.ClassBookName,
                ri.StudentNames,
                ri.IsTransferred,
                ri.CurriculumInfo,
                ri.GradePointAverage,
                ri.TotalGradesCount,
                ri.PoorGradesCount,
                ri.FairGradesCount,
                ri.GoodGradesCount,
                ri.VeryGoodGradesCount,
                ri.ExcellentGradesCount,
                ri.IsTotal)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        ReportPeriod period,
        int[] classBookIds,
        CancellationToken ct)
    {
        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        classBookPredicate =
            classBookPredicate.And(cb =>
                cb.SchoolYear == schoolYear &&
                cb.InstId == instId &&
                (cb.BookType == ClassBookType.Book_I_III ||
                 cb.BookType == ClassBookType.Book_IV ||
                 cb.BookType == ClassBookType.Book_V_XII));

        if (classBookIds.Any())
        {
            classBookPredicate = classBookPredicate.And(cb => this.DbContext.MakeIdsQuery(classBookIds).Any(id => cb.ClassBookId == id.Id));
        }

        var classBooks = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)

            select new
            {
                cb.ClassId,
                cb.ClassBookId,
                cb.ClassIsLvl2
            }
        ).ToListAsync(ct);

        var level1ClassIds = classBooks.Where(x => !x.ClassIsLvl2).Select(cb => cb.ClassId).ToArray();
        var level2ClassIds = classBooks.Where(x => x.ClassIsLvl2).Select(cb => cb.ClassId).ToArray();

        var curriculums = await (
                from cc in (
                    from cc in this.DbContext.Set<CurriculumClass>()

                    where level1ClassIds != null && level1ClassIds.Contains(cc.ClassId)

                    select new
                    {
                        cc.CurriculumId,
                        cc.ClassId,
                        cc.IsValid
                    })
                .Union(
                    from cg in this.DbContext.Set<ClassGroup>()

                    join cc in this.DbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId

                    where cg.SchoolYear == schoolYear && (level2ClassIds != null && level2ClassIds.Contains(cg.ClassId))

                    select new
                    {
                        cc.CurriculumId,
                        cg.ClassId,
                        cc.IsValid
                    })

                join cb in this.DbContext.Set<ClassBook>() on cc.ClassId equals cb.ClassId

                join c in this.DbContext.Set<Curriculum>() on cc.CurriculumId equals c.CurriculumId

                join cg in this.DbContext.Set<ClassBookCurriculumGradeless>()
                    on c.CurriculumId equals cg.CurriculumId
                    into j1
                from cg in j1.DefaultIfEmpty()

                join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

                join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

                where
                      cb.SchoolYear == schoolYear &&
                      cg == null &&
                      cc.IsValid &&
                      c.IsValid

                orderby cb.BasicClassId, cb.ClassBookId, cb.FullBookName ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

                select new
                {
                    cb.ClassId,
                    cb.BasicClassId,
                    cb.ClassBookId,
                    cb.FullBookName,
                    c.CurriculumId,
                    s.SubjectName,
                    SubjectTypeName = st.Name
                }
        ).ToArrayAsync(ct);

        var curriculumIds = curriculums.Select(c => c.CurriculumId).ToArray();

        var students = await (
            from sc in (
                from sc in this.DbContext.Set<StudentClass>()
                where sc.IsNotPresentForm == false &&
                      sc.SchoolYear == schoolYear &&
                      this.DbContext.MakeIdsQuery(level2ClassIds).Any(id => sc.ClassId == id.Id)
                select new
                {
                    ClassId = sc.ClassId,
                    sc.PersonId,
                    IsTransferred = sc.Status == StudentClassStatus.Transferred
                })
            .Union(
                from cg in this.DbContext.Set<ClassGroup>()
                join sc in this.DbContext.Set<StudentClass>()
                    on new { cg.SchoolYear, cg.ClassId } equals new { sc.SchoolYear, sc.ClassId }
                where sc.IsNotPresentForm == false &&
                      cg.SchoolYear == schoolYear &&
                      this.DbContext.MakeIdsQuery(level1ClassIds).Any(id => cg.ParentClassId == id.Id)
                select new
                {
                    ClassId = cg.ParentClassId ?? cg.ClassId,
                    sc.PersonId,
                    IsTransferred = sc.Status == StudentClassStatus.Transferred
                })

            join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId

            join cb in this.DbContext.Set<ClassBook>() on sc.ClassId equals cb.ClassId

            where (
                    from cs in this.DbContext.Set<CurriculumStudent>()
                    join c in this.DbContext.Set<Curriculum>()
                        on cs.CurriculumId equals c.CurriculumId

                    where c.SchoolYear == schoolYear &&
                          c.InstitutionId == instId &&
                          this.DbContext.MakeIdsQuery(curriculumIds).Any(id => cs.CurriculumId == id.Id)

                    select cs.PersonId
                  )
                  .Distinct()
                  .Contains(sc.PersonId)

            orderby p.FirstName, p.MiddleName, p.LastName

            select new
            {
                cb.ClassBookId,
                sc.PersonId,
                p.FirstName,
                p.MiddleName,
                p.LastName,
                sc.IsTransferred
            }
        ).ToArrayAsync(ct);

        classBookIds = classBooks.Select(cb => cb.ClassBookId).ToArray();

        var curriculumStudentsWithoutGrade = await (
                 from sg in this.DbContext.Set<ClassBookStudentGradeless>()

                 where
                     this.DbContext.MakeIdsQuery(classBookIds).Any(id => sg.ClassBookId == id.Id && sg.SchoolYear == schoolYear) &&
                     ((period == ReportPeriod.TermOne && sg.WithoutFirstTermGrade) ||
                      (period == ReportPeriod.TermTwo && sg.WithoutSecondTermGrade) ||
                      (period == ReportPeriod.WholeYear && sg.WithoutFirstTermGrade && sg.WithoutSecondTermGrade))

                 select new
                 {
                     sg.ClassBookId,
                     sg.CurriculumId,
                     sg.PersonId
                 })
            .Union(
                from sn in this.DbContext.Set<ClassBookStudentSpecialNeeds>()

                where
                    this.DbContext.MakeIdsQuery(classBookIds).Any(id => sn.ClassBookId == id.Id && sn.SchoolYear == schoolYear)

                select new
                {
                    sn.ClassBookId,
                    sn.CurriculumId,
                    sn.PersonId
                })
            .ToListAsync(ct);

        var curriculumTeachers = await (
                from t in this.DbContext.Set<CurriculumTeacher>()
                join c in this.DbContext.Set<Curriculum>() on t.CurriculumId equals c.CurriculumId
                join sp in this.DbContext.Set<StaffPosition>() on t.StaffPositionId equals sp.StaffPositionId
                join p in this.DbContext.Set<Person>() on sp.PersonId equals p.PersonId

                where c.SchoolYear == schoolYear &&
                      c.InstitutionId == instId

                select new
                {
                    c.CurriculumId,
                    TeacherName = StringUtils.JoinNames(p.FirstName, p.LastName)
                })
            .ToArrayAsync(ct);

        var groupedCurriculumTeacher = curriculumTeachers
            .GroupBy(x => x.CurriculumId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.TeacherName).Distinct());

        var sql = $"""
SELECT
    o.ClassBookId,
    o.BasicClassId,
    o.FullBookName AS ClassBookName,
    o.PersonID AS StudentPersonId,
    o.CurriculumID,
    o.Grade
FROM (
    SELECT
        g.PersonID,
        cb.ClassBookId,
        cb.BasicClassId,
        cb.FullBookName,
        c.CurriculumID,
        COALESCE(g.QualitativeGrade, CASE WHEN g.DecimalGrade < 3.00 THEN 2.00 ELSE ROUND(g.DecimalGrade, 0) END) AS Grade
    FROM
        [school_books].[Grade] g
        INNER JOIN [school_books].[ClassBook] cb ON cb.SchoolYear = g.SchoolYear AND cb.ClassBookId = g.ClassBookId
        INNER JOIN [core].[Person] p ON p.PersonID = g.PersonId
        INNER JOIN [inst_year].[Curriculum] c ON c.SchoolYear = g.SchoolYear AND c.CurriculumID = g.CurriculumId
        INNER JOIN [inst_year].[CurriculumStudent] cs ON cs.CurriculumID = g.CurriculumId AND cs.PersonID = g.PersonId
        LEFT JOIN [school_books].[ClassBookStudentGradeless] csg ON csg.SchoolYear = g.SchoolYear AND csg.CurriculumId = g.CurriculumId AND csg.PersonId = g.PersonId
    WHERE
        (g.DecimalGrade IS NOT NULL OR
         g.QualitativeGrade IS NOT NULL) AND
        g.Type <> 21 AND
        g.Type <> 22 AND
        c.IsValid = 1 AND
        ((@period = g.Term AND (csg.WithoutFirstTermGrade IS NULL OR csg.WithoutFirstTermGrade = 0)) OR
         (@period = g.Term AND (csg.WithoutSecondTermGrade IS NULL OR csg.WithoutSecondTermGrade = 0)) OR
         (@period = {(int)ReportPeriod.WholeYear} AND g.Term IN ({(int)ReportPeriod.TermOne}, {(int)ReportPeriod.TermTwo}) AND
          ((g.Term = 1 AND (csg.WithoutFirstTermGrade IS NULL OR csg.WithoutFirstTermGrade = 0)) OR
           (g.Term = 2 AND (csg.WithoutSecondTermGrade IS NULL OR csg.WithoutSecondTermGrade = 0))))) AND
        cb.InstId = @InstId AND
        cb.SchoolYear = @SchoolYear AND
        {"g.ClassBookId IN (" + string.Join(',', classBookIds) + ")"}
) o
ORDER BY
    o.BasicClassId,
    o.ClassBookId,
    o.FullBookName
""";

        var grades = await this.DbContext.Set<RegularGradesByStudentsQO>()
            .FromSqlRaw(sql,
                new SqlParameter("schoolYear", schoolYear),
                new SqlParameter("InstId", instId),
                new SqlParameter("period", period))
            .ToArrayAsync(ct);

        grades = grades
            .Where(g =>
                !curriculumStudentsWithoutGrade.Contains(new
                {
                    g.ClassBookId,
                    g.CurriculumId,
                    PersonId = g.StudentPersonId
                }))
            .ToArray();

        var result = Enumerable.Concat(
            new[]
            {
                new GetItemsForAddVO(
                     "Всички",
                        "Всички",
                        false,
                        "Всички",
                        grades.Length > 0 ? grades.Average(i => i.Grade) : 0M,
                        grades.Length,
                        grades.Count(i => i.Grade is 2.0M),
                        grades.Count(i => i.Grade is 3.0M),
                        grades.Count(i => i.Grade is 4.0M),
                        grades.Count(i => i.Grade is 5.0M),
                        grades.Count(i => i.Grade is 6.0M),
                        true)
                },
                curriculums
                    .GroupBy(g1 => new { g1.ClassId, g1.ClassBookId, g1.BasicClassId, g1.FullBookName })
                    .Select(g1 =>
                    {
                        var studentsPerClassBook = students.Where(s => s.ClassBookId == g1.Key.ClassBookId).ToArray();
                        var gradesForClassBook = grades.Where(g => g.ClassBookId == g1.Key.ClassBookId).ToArray();

                        return Enumerable.Concat(
                                new[]
                                {
                                    new GetItemsForAddVO(
                                        g1.Key.FullBookName,
                                        "Всички",
                                        false,
                                        "Всички",
                                        gradesForClassBook.Length > 0 ? gradesForClassBook.Average(i => i.Grade) : 0M,
                                        gradesForClassBook.Length,
                                        gradesForClassBook.Count(i => i.Grade is 2.0M),
                                        gradesForClassBook.Count(i => i.Grade is 3.0M),
                                        gradesForClassBook.Count(i => i.Grade is 4.0M),
                                        gradesForClassBook.Count(i => i.Grade is 5.0M),
                                        gradesForClassBook.Count(i => i.Grade is 6.0M),
                                        true)
                                },
                                studentsPerClassBook
                                .Select(s =>
                                {
                                    var gradesForStudent = grades.Where(g => g.StudentPersonId == s.PersonId).ToArray();

                                    return Enumerable.Concat(
                                        new[]
                                        {
                                            new GetItemsForAddVO(
                                                g1.Key.FullBookName,
                                                $"{s.FirstName} {s.LastName}",
                                                s.IsTransferred,
                                                "Всички",
                                                gradesForStudent.Length > 0 ? gradesForStudent.Average(i => i.Grade) : 0M,
                                                gradesForStudent.Length,
                                                gradesForStudent.Count(i => i.Grade is 2.0M),
                                                gradesForStudent.Count(i => i.Grade is 3.0M),
                                                gradesForStudent.Count(i => i.Grade is 4.0M),
                                                gradesForStudent.Count(i => i.Grade is 5.0M),
                                                gradesForStudent.Count(i => i.Grade is 6.0M),
                                                true)
                                        },
                                        g1
                                            .Where(g => !curriculumStudentsWithoutGrade.Contains(new
                                                {
                                                    g.ClassBookId,
                                                    g.CurriculumId,
                                                    s.PersonId
                                                }))
                                            .GroupBy(g2 => new { g2.CurriculumId, g2.SubjectName, g2.SubjectTypeName })
                                            .Select(g2 =>
                                                {
                                                    var gradesForCurriculum =
                                                        gradesForStudent.Where(g => g.CurriculumId == g2.Key.CurriculumId).ToArray();
                                                    var teachers = groupedCurriculumTeacher
                                                        .FirstOrDefault(t => t.Key == g2.Key.CurriculumId).Value;
                                                    var teachersNames = teachers != null ? string.Join(", ", teachers) : string.Empty;

                                                    return new GetItemsForAddVO(
                                                        g1.Key.FullBookName,
                                                        $"{s.FirstName} {s.LastName}",
                                                        s.IsTransferred,
                                                        $"{g2.Key.SubjectName}/{g2.Key.SubjectTypeName} - {teachersNames}",
                                                        gradesForCurriculum.Length > 0 ? gradesForCurriculum.Average(i => i.Grade) : 0M,
                                                        gradesForCurriculum.Length,
                                                        gradesForCurriculum.Count(i => i.Grade is 2.0M),
                                                        gradesForCurriculum.Count(i => i.Grade is 3.0M),
                                                        gradesForCurriculum.Count(i => i.Grade is 4.0M),
                                                        gradesForCurriculum.Count(i => i.Grade is 5.0M),
                                                        gradesForCurriculum.Count(i => i.Grade is 6.0M),
                                                        false
                                                    );
                                                }
                                            )
                                        ).ToArray();
                                }).SelectMany(i => i)
                            ).ToArray();
                    }).SelectMany(i => i)
            ).ToArray();


        return result;
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int regularGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from abc in this.DbContext.Set<RegularGradePointAverageByStudentsReport>()
            where abc.SchoolYear == schoolYear &&
                abc.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsReportId
            select abc.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int regularGradePointAverageByStudentsReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<RegularGradePointAverageByStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsReportId

            orderby ri.RegularGradePointAverageByStudentsReportItemId

            select new GetExcelDataVOItem
            (
                ri.ClassBookName,
                ri.StudentNames,
                ri.IsTransferred,
                ri.CurriculumInfo,
                ri.GradePointAverage,
                ri.TotalGradesCount,
                ri.PoorGradesCount,
                ri.FairGradesCount,
                ri.GoodGradesCount,
                ri.VeryGoodGradesCount,
                ri.ExcellentGradesCount,
                ri.IsTotal
            )
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<RegularGradePointAverageByStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.RegularGradePointAverageByStudentsReportId == regularGradePointAverageByStudentsReportId

            select new GetExcelDataVO
            (
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate,
                items
            )
        ).SingleAsync(ct);
    }
}
