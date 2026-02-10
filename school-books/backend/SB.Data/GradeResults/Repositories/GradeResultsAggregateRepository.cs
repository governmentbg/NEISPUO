namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

internal class GradeResultsAggregateRepository : ScopedAggregateRepository<GradeResult>, IGradeResultsAggregateRepository
{
    public GradeResultsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<GradeResult>, IQueryable<GradeResult>>[] Includes =>
        new Func<IQueryable<GradeResult>, IQueryable<GradeResult>>[]
        {
            (q) => q.Include(gr => gr.GradeResultSubjects),
        };

    public async Task<GradeResult[]> FindAllByClassBookIdAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            gr =>
                gr.SchoolYear == schoolYear &&
                gr.ClassBookId == classBookId,
            ct)
            ).ToArray();
    }
}
