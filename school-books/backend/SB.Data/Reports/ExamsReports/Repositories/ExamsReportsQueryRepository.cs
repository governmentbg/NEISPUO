namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IExamsReportsQueryRepository;
using Person = Domain.Person;

internal class ExamsReportsQueryRepository : Repository, IExamsReportsQueryRepository
{
    public ExamsReportsQueryRepository(UnitOfWork unitOfWork)
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
            from r in this.DbContext.Set<ExamsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.CreatedBySysUserId == sysUserId

            orderby r.CreateDate descending

            select new GetAllVO(
                r.ExamsReportId,
                r.CreateDate)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from e in this.DbContext.Set<Exam>()
            join cb in this.DbContext.Set<ClassBook>() on e.ClassBookId equals cb.ClassBookId
            join csu in this.DbContext.Set<SysUser>() on e.CreatedBySysUserId equals csu.SysUserId
            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId
            join c in this.DbContext.Set<Curriculum>() on e.CurriculumId equals c.CurriculumId
            join s in this.DbContext.Set<Subject>() on c.SubjectId equals s.SubjectId
            join st in this.DbContext.Set<SubjectType>() on c.SubjectTypeId equals st.SubjectTypeId

            where cb.InstId == instId && cb.SchoolYear == schoolYear && cb.IsValid

            orderby e.Date, cb.BasicClassId, cb.FullBookName

            select new GetItemsForAddVO(
                e.Date,
                cb.FullBookName,
                e.Type,
                $"{s.SubjectName} / {st.Name}",
                StringUtils.JoinNames(cp.FirstName, cp.LastName))
        ).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int examsReportId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<ExamsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.ExamsReportId == examsReportId

            select new GetVO(
                r.ExamsReportId,
                r.CreateDate)
        ).SingleAsync(ct);
    }

    public async Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int examsReportId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from ri in this.DbContext.Set<ExamsReportItem>()
            join r in this.DbContext.Set<ExamsReport>()
                on new { ri.SchoolYear, ri.ExamsReportId } equals new { r.SchoolYear, r.ExamsReportId }

            where ri.SchoolYear == schoolYear && r.InstId == instId && ri.ExamsReportId == examsReportId

            orderby ri.ExamsReportItemId

            select new GetItemsVO(
                ri.Date,
                ri.ClassBookName,
                ri.BookExamType.GetEnumDescription(),
                ri.CurriculumName,
                ri.CreatedBySysUserName)
        ).ToTableResultAsync(offset, limit, ct);
    }

    public async Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int examsReportId,
        CancellationToken ct)
    {
        return await (
            from srod in this.DbContext.Set<ExamsReport>()
            where srod.SchoolYear == schoolYear &&
                  srod.InstId == instId &&
                  srod.ExamsReportId == examsReportId
            select srod.CreatedBySysUserId
        ).SingleAsync(ct);
    }

    public async Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int examsReportId,
        CancellationToken ct)
    {
        var resultItems = await (
            from ri in this.DbContext.Set<ExamsReportItem>()

            where ri.SchoolYear == schoolYear &&
                  ri.ExamsReportId == examsReportId

            orderby ri.ExamsReportItemId

            select new GetExcelDataVOItem(
                ri.Date,
                ri.ClassBookName,
                ri.BookExamType,
                ri.CurriculumName,
                ri.CreatedBySysUserName)
        ).ToArrayAsync(ct);

        return await (
            from r in this.DbContext.Set<ExamsReport>()

            where r.SchoolYear == schoolYear &&
                  r.InstId == instId &&
                  r.ExamsReportId == examsReportId

            select new GetExcelDataVO(
                r.CreateDate,
                resultItems)
        ).SingleAsync(ct);
    }
}
