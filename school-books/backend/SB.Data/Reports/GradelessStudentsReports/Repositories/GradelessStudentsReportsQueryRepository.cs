namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IGradelessStudentsReportsQueryRepository;

internal class GradelessStudentsReportsQueryRepository : Repository, IGradelessStudentsReportsQueryRepository
{
    public GradelessStudentsReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<GradelessStudentsReport>()

            where r.SchoolYear == schoolYear &&
                r.InstId == instId &&
                r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SchoolYear,
                r.GradelessStudentsReportId,
                r.OnlyFinalGrades,
                EnumUtils.GetEnumDescription(r.Period),
                r.ClassBookNames,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<GradelessStudentsReport>()

            where r.SchoolYear == schoolYear && r.InstId == instId && r.GradelessStudentsReportId == gradelessStudentsReportId

            select new GetVO(
                r.SchoolYear,
                r.GradelessStudentsReportId,
                r.OnlyFinalGrades,
                r.Period,
                r.ClassBookNames,
                r.CreateDate)
         ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<GradelessStudentsReportItem>()
            join r in this.DbContext.Set<GradelessStudentsReport>() on new { ri.SchoolYear, ri.GradelessStudentsReportId } equals new { r.SchoolYear, r.GradelessStudentsReportId }
            where ri.SchoolYear == schoolYear && r.InstId == instId && ri.GradelessStudentsReportId == gradelessStudentsReportId
            orderby ri.GradelessStudentsReportItemId

            select new GetItemsVO(
                ri.ClassBookName,
                ri.StudentName,
                ri.CurriculumName)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        bool onlyFinalGrades,
        ReportPeriod period,
        int[] classBookIds,
        CancellationToken ct)
    {
        var classBookPredicate = PredicateBuilder.True<ClassBook>();
        var gradePredicate = PredicateBuilder.True<Grade>();

        classBookPredicate = classBookPredicate.And(c => c.SchoolYear == schoolYear && c.InstId == instId && c.IsValid);

        classBookPredicate = classBookPredicate.And(
            cb => cb.BookType == ClassBookType.Book_I_III ||
                cb.BookType == ClassBookType.Book_IV ||
                cb.BookType == ClassBookType.Book_V_XII ||
                cb.BookType == ClassBookType.Book_CSOP);

        gradePredicate = gradePredicate.And(g => g.SchoolYear == schoolYear);

        if (classBookIds.Any())
        {
            classBookPredicate = classBookPredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
            gradePredicate = gradePredicate.And(g => this.DbContext.MakeIdsQuery(classBookIds).Any(cid => g.ClassBookId == cid.Id));
        }

        if (period != ReportPeriod.WholeYear)
        {
            gradePredicate = gradePredicate.And(g => g.Term == (SchoolTerm)period);
        }

        if (onlyFinalGrades)
        {
            if (period == ReportPeriod.WholeYear)
            {
                gradePredicate = gradePredicate.And(g => g.Type == GradeType.Final);
            }
            else
            {
                gradePredicate = gradePredicate.And(g => g.Type == GradeType.Term || g.Type == GradeType.OtherClassTerm || g.Type == GradeType.OtherSchoolTerm);
            }
        }
        else
        {
            gradePredicate = gradePredicate.And(g => g.Type != GradeType.Final && g.Type != GradeType.Term && g.Type != GradeType.OtherClassTerm && g.Type != GradeType.OtherSchoolTerm);
        }

        var studentPersonIdsWithGrades = await (
            from g in this.DbContext.Set<Grade>().Where(gradePredicate)
            join cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate) on new { g.SchoolYear, g.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            select new
            {
                g.ClassBookId,
                g.PersonId,
                g.CurriculumId,
            }
        ).Distinct().ToArrayAsync(ct);

        var gradelessStudentIds = await (
            from gcc in this.DbContext.Set<ClassBookStudentGradeless>()
            join cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate) on new { gcc.SchoolYear, gcc.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            where
                ((gcc.WithoutFirstTermGrade && period == ReportPeriod.TermOne) ||
                (gcc.WithoutSecondTermGrade && period == ReportPeriod.TermTwo) ||
                gcc.WithoutFinalGrade)
            select new
            {
                gcc.ClassBookId,
                gcc.PersonId,
                gcc.CurriculumId
            }
        ).ToArrayAsync(ct);

        var excludedStudentIds = studentPersonIdsWithGrades
            .Union(gradelessStudentIds)
            .ToDictionary(x => new { x.ClassBookId, x.PersonId, x.CurriculumId });

        var nonCombinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)

            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals new { cg.SchoolYear, ClassId = cg.ParentClassId }

            join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.InstitutionId, cg.ClassId } equals new { sc.SchoolYear, sc.InstitutionId, sc.ClassId }

            join cc in this.DbContext.Set<CurriculumClass>() on cg.ParentClassId equals cc.ClassId

            join cs in this.DbContext.Set<CurriculumStudent>() on new { cc.CurriculumId, sc.PersonId } equals new { cs.CurriculumId, cs.PersonId }

            join c in this.DbContext.Set<Curriculum>() on cs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on cs.PersonId equals p.PersonId

            join cbcg in this.DbContext.Set<ClassBookCurriculumGradeless>()
            on new { cb.SchoolYear, cb.ClassBookId, c.CurriculumId } equals new { cbcg.SchoolYear, cbcg.ClassBookId, cbcg.CurriculumId }
            into j1
            from cbcg in j1.DefaultIfEmpty()

            orderby cb.BasicClassId, cb.FullBookName, p.FirstName, p.MiddleName, p.LastName ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            where
                cs.IsValid &&
                sc.Status == StudentClassStatus.Enrolled &&
                cbcg == null

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                c.CurriculumId,
                p.PersonId,
                StudentName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                CurriculumName = $"{s.SubjectName} / {st.Name}"
            }).ToArrayAsync(ct);

        var combinedClassBooksItems = await (
            from cb in this.DbContext.Set<ClassBook>().Where(classBookPredicate)

            join sc in this.DbContext.Set<StudentClass>() on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }

            join cc in this.DbContext.Set<CurriculumClass>() on sc.ClassId equals cc.ClassId

            join cs in this.DbContext.Set<CurriculumStudent>() on new { cc.CurriculumId, sc.PersonId } equals new { cs.CurriculumId, cs.PersonId }

            join c in this.DbContext.Set<Curriculum>() on cs.CurriculumId equals c.CurriculumId

            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId

            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            join p in this.DbContext.Set<Person>() on cs.PersonId equals p.PersonId

            join cbcg in this.DbContext.Set<ClassBookCurriculumGradeless>()
            on new { cb.SchoolYear, cb.ClassBookId, c.CurriculumId } equals new { cbcg.SchoolYear, cbcg.ClassBookId, cbcg.CurriculumId }
            into j1
            from cbcg in j1.DefaultIfEmpty()

            orderby cb.BasicClassId, cb.FullBookName, p.FirstName, p.MiddleName, p.LastName ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

            where
                cs.IsValid &&
                sc.Status == StudentClassStatus.Enrolled &&
                cbcg == null

            select new
            {
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                c.CurriculumId,
                p.PersonId,
                StudentName = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                CurriculumName = $"{s.SubjectName} / {st.Name}"
            }).ToArrayAsync(ct);

        return
            nonCombinedClassBooksItems
            .Union(combinedClassBooksItems)
            .Distinct()
            .Where(r => !excludedStudentIds.ContainsKey(new { r.ClassBookId, r.PersonId, r.CurriculumId }))
            .Select(r => new GetItemsForAddVO(r.ClassBookName, r.StudentName, r.CurriculumName))
            .ToArray();
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from mtr in this.DbContext.Set<GradelessStudentsReport>()
            where mtr.SchoolYear == schoolYear &&
                mtr.GradelessStudentsReportId == gradelessStudentsReportId
            select mtr.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        CancellationToken ct)
    {
        var items = await (
            from ri in this.DbContext.Set<GradelessStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.GradelessStudentsReportId == gradelessStudentsReportId

            orderby ri.GradelessStudentsReportItemId

            select new GetExcelDataVOItem
            (
                ri.ClassBookName,
                ri.StudentName,
                ri.CurriculumName
            )
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<GradelessStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.GradelessStudentsReportId == gradelessStudentsReportId

            select new GetExcelDataVO
            (
                r.OnlyFinalGrades,
                r.Period,
                r.ClassBookNames,
                r.CreateDate,
                items
            )
        ).SingleAsync(ct);
    }
}
