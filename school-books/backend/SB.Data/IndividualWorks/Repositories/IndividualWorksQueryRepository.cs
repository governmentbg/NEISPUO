namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.IIndividualWorksQueryRepository;

internal class IndividualWorksQueryRepository : Repository, IIndividualWorksQueryRepository
{
    public IndividualWorksQueryRepository(UnitOfWork unitOfWork)
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

        return await(
            from iw in this.DbContext.Set<IndividualWork>()

            join p in this.DbContext.Set<Person>()
                on iw.PersonId
                equals p.PersonId

            join csu in this.DbContext.Set<SysUser>() on iw.CreatedBySysUserId equals csu.SysUserId

            join cp in this.DbContext.Set<Person>() on csu.PersonId equals cp.PersonId

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on p.PersonId equals sc.PersonId
                into g1
            from sc in g1.DefaultIfEmpty()

            where iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName, iw.Date

            select new GetAllVO(
                iw.IndividualWorkId,
                p.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                iw.Date,
                iw.IndividualWorkActivity,
                StringUtils.JoinNames(cp.FirstName, cp.MiddleName, cp.LastName)))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int individualWorkId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<IndividualWork>()
            .Where(iw =>
                iw.SchoolYear == schoolYear &&
                iw.ClassBookId == classBookId &&
                iw.IndividualWorkId == individualWorkId)
            .Select(iw => new GetVO(
                iw.IndividualWorkId,
                iw.PersonId,
                iw.Date,
                iw.IndividualWorkActivity,
                iw.CreatedBySysUserId))
            .SingleAsync(ct);
    }
}
