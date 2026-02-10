namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ShiftNomsRepository : Repository, IScopedEntityNomsRepository<Shift>
{
    public ShiftNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<Shift>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                s.InstId == instId &&
                !s.IsAdhoc &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.ShiftId == id.Id))
            .Select(s => new NomVO(s.ShiftId, s.Name))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(int schoolYear, int instId, string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Shift>();
        predicate = predicate
            .And(s =>
                s.SchoolYear == schoolYear &&
                s.InstId == instId &&
                !s.IsAdhoc);

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(t => t.Name, word);
            }
        }

        return await this.DbContext.Set<Shift>()
            .Where(predicate)
            .OrderBy(s => s.Name)
            .Select(s => new NomVO(s.ShiftId, s.Name))
            .WithOffsetAndLimit(offset ?? 0, limit)
            .ToArrayAsync(ct);
    }
}
