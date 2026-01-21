namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class SubjectTypeNomsRepository : Repository, IEntityNomsRepository<SubjectType>
{
    public SubjectTypeNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<SubjectType>()
            .Where(s =>
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.SubjectTypeId == id.Id))
            .Select(bc => new NomVO(bc.SubjectTypeId, bc.Name))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<SubjectType>();
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(t => t.Name, word);
            }
        }

        return await this.DbContext.Set<SubjectType>()
            .Where(st => st.IsValid)
            .Where(predicate)
            .OrderBy(st => st.Name)
            .Select(s => new NomVO(s.SubjectTypeId, s.Name))
            .WithOffsetAndLimit(offset ?? 0, limit)
            .ToArrayAsync(ct);
    }
}
