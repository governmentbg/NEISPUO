namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.ISessionStudentsReportsQueryRepository;
using Person = Domain.Person;

internal class SessionStudentsReportsQueryRepository : Repository, ISessionStudentsReportsQueryRepository
{
    private const int MinGradeToPassExam = 3;

    public SessionStudentsReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<SessionStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.SessionStudentsReportId,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var nonCombinedClassBookStudents =
            from cb in this.DbContext.Set<ClassBook>()
            join cg in this.DbContext.Set<ClassGroup>() on new { cb.SchoolYear, ClassId = (int?)cb.ClassId } equals new { cg.SchoolYear, ClassId = cg.ParentClassId }
            join sc in this.DbContext.Set<StudentClass>() on new { cg.SchoolYear, cg.InstitutionId, cg.ClassId } equals new { sc.SchoolYear, sc.InstitutionId, sc.ClassId }
            where cb.InstId == instId && cb.SchoolYear == schoolYear && cb.IsValid

            select new
            {
                cb.BookType,
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                sc.PersonId,
                IsTransferred = sc.Status == StudentClassStatus.Transferred
            };

        var combinedClassBookStudents =
            from cb in this.DbContext.Set<ClassBook>()
            join sc in this.DbContext.Set<StudentClass>() on new { cb.SchoolYear, cb.ClassId } equals new { sc.SchoolYear, sc.ClassId }
            where cb.InstId == instId && cb.SchoolYear == schoolYear && cb.IsValid

            select new
            {
                cb.BookType,
                cb.ClassBookId,
                ClassBookName = cb.FullBookName,
                cb.BasicClassId,
                sc.PersonId,
                IsTransferred = sc.Status == StudentClassStatus.Transferred
            };

        var allClassBooksStudents = nonCombinedClassBookStudents.Union(combinedClassBookStudents);

        var result = await (
                from gr in this.DbContext.Set<GradeResult>()
                join sc in allClassBooksStudents on new { gr.ClassBookId, gr.PersonId } equals new { sc.ClassBookId, sc.PersonId }
                join p in this.DbContext.Set<Person>() on sc.PersonId equals p.PersonId
                join grc in this.DbContext.Set<GradeResultSubject>() on gr.GradeResultId equals grc.GradeResultId
                join c in this.DbContext.Set<Curriculum>() on grc.CurriculumId equals c.CurriculumId
                join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
                join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

                orderby sc.BasicClassId, sc.ClassBookId, sc.ClassBookName, p.FirstName, p.MiddleName, p.LastName ascending, c.CurriculumPartID, c.IsIndividualLesson, c.TotalTermHours descending

                select new
                {
                    IsSession1Failed = grc.Session1NoShow == true || grc.Session1Grade < MinGradeToPassExam,
                    IsSession2Failed = grc.Session2NoShow == true || grc.Session2Grade < MinGradeToPassExam,
                    ClassBookName = sc.ClassBookName,
                    sc.BasicClassId,
                    StudentNames = StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                    IsTransferred = sc.IsTransferred,
                    CurriculumName = $"{s.SubjectName} / {st.Name}"
                })
                .ToArrayAsync(ct);

        return result.GroupBy(s => new { s.StudentNames, s.ClassBookName, s.BasicClassId, s.IsTransferred })
                .Select(g =>
                    new GetItemsForAddVO
                    {
                        StudentNames = g.Key.StudentNames,
                        ClassBookName = g.Key.ClassBookName,
                        IsTransferred = g.Key.IsTransferred,
                        Session1CurriculumNames = string.Join(", ", g.Select(gi => gi.CurriculumName)),
                        Session2CurriculumNames = string.Join(", ", g.Where(x => x.IsSession1Failed).Select(gi => gi.CurriculumName)),
                        Session3CurriculumNames = string.Join(", ", g.Where(x => x.IsSession2Failed).Select(gi => gi.CurriculumName))
                    }).ToArray();
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int sessionStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<SessionStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.SessionStudentsReportId == sessionStudentsReportId

            select new GetVO(
                r.SessionStudentsReportId,
                r.CreateDate)
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int sessionStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (from ri in this.DbContext.Set<SessionStudentsReportItem>()
                      join r in this.DbContext.Set<SessionStudentsReport>()
                          on new { ri.SchoolYear, ri.SessionStudentsReportId } equals new { r.SchoolYear, r.SessionStudentsReportId }

                      where ri.SchoolYear == schoolYear && r.InstId == instId && ri.SessionStudentsReportId == sessionStudentsReportId

                      orderby ri.SessionStudentsReportItemId

                      select new GetItemsVO(
                          ri.StudentNames,
                          ri.ClassBookName,
                          ri.IsTransferred,
                          ri.Session1CurriculumNames,
                          ri.Session2CurriculumNames,
                          ri.Session3CurriculumNames))
                        .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int sessionStudentsReportId,
        CancellationToken ct)
    {
        return await (
            from srod in this.DbContext.Set<SessionStudentsReport>()
            where srod.SchoolYear == schoolYear &&
                  srod.InstId == instId &&
                  srod.SessionStudentsReportId == sessionStudentsReportId
            select srod.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int sessionStudentsReportId,
        CancellationToken ct)
    {
        var resultItems = await (
            from ri in this.DbContext.Set<SessionStudentsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.SessionStudentsReportId == sessionStudentsReportId

            orderby ri.SessionStudentsReportItemId

            select new GetExcelDataVOItem(
                ri.StudentNames,
                ri.IsTransferred,
                ri.ClassBookName,
                ri.Session1CurriculumNames,
                ri.Session2CurriculumNames,
                ri.Session3CurriculumNames)
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<SessionStudentsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.SessionStudentsReportId == sessionStudentsReportId

            select new GetExcelDataVO(
                r.CreateDate,
                resultItems)
        ).SingleAsync(ct);
    }
}
