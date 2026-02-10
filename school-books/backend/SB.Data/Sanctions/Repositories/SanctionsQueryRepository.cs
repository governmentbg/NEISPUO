namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ISanctionsQueryRepository;

internal class SanctionsQueryRepository : Repository, ISanctionsQueryRepository
{
    public SanctionsQueryRepository(UnitOfWork unitOfWork)
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
            from s in this.DbContext.Set<Sanction>()
            join st in this.DbContext.Set<SanctionType>() on s.SanctionTypeId equals st.Id

            join p in this.DbContext.Set<Person>()
                on s.PersonId
                equals p.PersonId

            join sc in this.DbContext.StudentsForClassBook(schoolYear, classBook.ClassId, classBook.ClassIsLvl2)
                on p.PersonId equals sc.PersonId
                into g1
            from sc in g1.DefaultIfEmpty()

            where s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId

            orderby p.FirstName, p.MiddleName, p.LastName

            select new GetAllVO(
                s.SanctionId,
                p.PersonId,
                StringUtils.JoinNames(p.FirstName, p.MiddleName, p.LastName),
                (bool?)sc.IsTransferred ?? true,
                st.Name,
                s.StartDate,
                s.EndDate))
            .ToTableResultAsync(offset, limit, ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int sanctionId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<Sanction>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.ClassBookId == classBookId &&
                s.SanctionId == sanctionId)
            .Select(s => new GetVO(
                s.SanctionId,
                s.PersonId,
                s.SanctionTypeId,
                s.OrderNumber,
                s.OrderDate,
                s.StartDate,
                s.EndDate,
                s.Description,
                s.CancelOrderNumber,
                s.CancelOrderDate,
                s.CancelReason,
                s.RuoOrderNumber,
                s.RuoOrderDate))
            .SingleAsync(ct);
    }
}
