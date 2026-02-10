namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class BasicClassNomsRepository : Repository, IEntityNomsRepository<BasicClass>
{
    public BasicClassNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<BasicClass>()
            .Where(s =>
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.BasicClassId == id.Id))
            .Select(bc => new NomVO(bc.BasicClassId, bc.Name))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<BasicClass>();
        predicate = predicate.And(s => s.IsValid == true);
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(t => t.Name, word);
            }
        }

        return await this.DbContext.Set<BasicClass>()
            .Where(predicate)
            .OrderBy(s => s.BasicClassId)
            .ThenBy(s => s.Name)
            .Select(s => new NomVO(s.BasicClassId, s.Name))
            .WithOffsetAndLimit(offset ?? 0, limit)
            .ToArrayAsync(ct);
    }
}
