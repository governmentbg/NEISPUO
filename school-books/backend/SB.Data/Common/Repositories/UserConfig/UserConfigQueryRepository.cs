namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IUserConfigQueryRepository;

internal class UserConfigQueryRepository : Repository, IUserConfigQueryRepository
{
    public UserConfigQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetVO> GetAsync(
        int? tokenInstId,
        CancellationToken ct)
    {
        var systemSchoolYear = await (
            from cy in this.DbContext.Set<CurrentYear>()

            where cy.IsValid

            select cy.CurrentYearId
        ).SingleAsync(ct);

        int? tokenInstSchoolYear = null;
        if (tokenInstId != null)
        {
            tokenInstSchoolYear = await (
                from isy in this.DbContext.Set<InstitutionSchoolYear>()

                where isy.InstitutionId == tokenInstId &&
                    isy.IsCurrent

                select isy.SchoolYear
            ).SingleAsync(ct);
        }

        return new GetVO(
            systemSchoolYear,
            tokenInstId,
            tokenInstSchoolYear);
    }
}
