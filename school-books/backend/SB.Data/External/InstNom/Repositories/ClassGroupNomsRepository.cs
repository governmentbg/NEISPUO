namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ClassGroupNomsRepository : Repository, IScopedEntityNomsRepository<ClassGroup>
{
    public ClassGroupNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<ClassGroup>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.InstitutionId == instId &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.ClassId == id.Id))
            .Select(bc => new NomVO(bc.ClassId, bc.ClassName))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(int schoolYear, int instId, string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<ClassGroup>();

        predicate = predicate
            .And(cg =>
                cg.SchoolYear == schoolYear &&
                cg.InstitutionId == instId);

        predicate = predicate.And(cg => cg.ParentClassId == null);

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(t => t.ClassName, word);
            }
        }

        return await (
            from cg in this.DbContext.Set<ClassGroup>().Where(predicate)
            orderby cg.BasicClassId, cg.ClassName
            select new NomVO(cg.ClassId, cg.ClassName)
            ).WithOffsetAndLimit(offset ?? 0, limit)
            .ToArrayAsync(ct);
    }
}
