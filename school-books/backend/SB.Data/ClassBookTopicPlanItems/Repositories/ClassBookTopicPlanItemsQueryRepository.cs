namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IClassBookTopicPlanItemsQueryRepository;

internal class ClassBookTopicPlanItemsQueryRepository : Repository, IClassBookTopicPlanItemsQueryRepository
{
    public ClassBookTopicPlanItemsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<ClassBookTopicPlanItem>()

            where tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId && tpi.CurriculumId == curriculumId

            orderby tpi.Number, tpi.CreateDate

            select new GetAllVO(
                tpi.ClassBookTopicPlanItemId,
                tpi.Number,
                tpi.Title,
                tpi.Taken,
                tpi.Note))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookTopicPlanItemId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<ClassBookTopicPlanItem>()

            where tpi.SchoolYear == schoolYear && tpi.ClassBookTopicPlanItemId == classBookTopicPlanItemId

            select new GetVO(
                tpi.Number,
                tpi.Title,
                tpi.Taken,
                tpi.Note))
            .SingleAsync(ct);
    }

    public async Task<GetExcelDataVO[]> GetExcelDataAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<ClassBookTopicPlanItem>()

            where tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId && tpi.CurriculumId == curriculumId

            orderby tpi.Number, tpi.CreateDate

            select new GetExcelDataVO(
                tpi.Number,
                tpi.Title,
                tpi.Note)
        ).ToArrayAsync(ct);
    }

    public async Task<bool> HasAnyAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        return await (
            from tpi in this.DbContext.Set<ClassBookTopicPlanItem>()

            where tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId && tpi.CurriculumId == curriculumId

            select tpi.ClassBookTopicPlanItemId
        ).AnyAsync(ct);
    }
}
