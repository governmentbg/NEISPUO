namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IPerformancesQueryRepository;

internal class PerformancesQueryRepository : Repository, IPerformancesQueryRepository
{
    public PerformancesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await(
            from p in this.DbContext.Set<Performance>()
            join pt in this.DbContext.Set<PerformanceType>() on p.PerformanceTypeId equals pt.Id

            where p.SchoolYear == schoolYear &&
                p.ClassBookId == classBookId

            orderby p.StartDate, p.EndDate

            select new GetAllVO(
                p.PerformanceId,
                p.Name,
                pt.Name,
                p.StartDate,
                p.EndDate))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int performanceId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Performance>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.PerformanceId == performanceId)
            .Select(s => new GetVO(
                s.PerformanceId,
                s.PerformanceTypeId,
                s.Name,
                s.Description,
                s.StartDate,
                s.EndDate,
                s.Location,
                s.StudentAwards))
            .SingleAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetExcelDataAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Performance>()
            join pt in this.DbContext.Set<PerformanceType>() on p.PerformanceTypeId equals pt.Id

            where p.SchoolYear == schoolYear && p.ClassBookId == classBookId

            orderby p.StartDate, p.EndDate

            select new GetExcelDataVO(
                null,
                pt.Name,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Location,
                p.StudentAwards))
            .ToArrayAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetAllExcelDataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from p in this.DbContext.Set<Performance>()
            join pt in this.DbContext.Set<PerformanceType>() on p.PerformanceTypeId equals pt.Id
            join cb in this.DbContext.Set<ClassBook>() on new { p.SchoolYear, p.ClassBookId } equals new { cb.SchoolYear, cb.ClassBookId }
            join bc in this.DbContext.Set<BasicClass>() on cb.BasicClassId equals bc.BasicClassId
            into j1
            from bc in j1.DefaultIfEmpty()

            where p.SchoolYear == schoolYear && cb.InstId == instId

            orderby cb.BasicClassId, cb.BookName, p.StartDate, p.EndDate

            select new GetExcelDataVO(
                string.IsNullOrEmpty(bc.Name) || string.IsNullOrEmpty(cb.BookName)
                    ? (bc.Name ?? cb.BookName)
                    : (bc.Name + " - " + cb.BookName),
                pt.Name,
                p.Name,
                p.Description,
                p.StartDate,
                p.EndDate,
                p.Location,
                p.StudentAwards))
            .ToArrayAsync(ct);
    }
}
