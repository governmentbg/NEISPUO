namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class InstitutionSchoolYearNomsRepository : Repository, IScopedEntityNomsRepository<InstitutionSchoolYear>
{
    public InstitutionSchoolYearNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<InstitutionSchoolYear>()
            .Where(s =>
                s.SchoolYear == schoolYear &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.InstitutionId == id.Id))
            .Select(i => new NomVO(i.InstitutionId, $"{i.InstitutionId} {i.Name}"))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(int schoolYear, int instId, string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<InstitutionSchoolYear>();

        predicate = predicate.And(s => s.SchoolYear == schoolYear);

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndAnyStringContains(t => t.InstitutionId.ToString(), i => i.Name, word);
            }
        }

        return await this.DbContext.Set<InstitutionSchoolYear>()
            .Where(predicate)
            .OrderBy(i => i.InstitutionId)
            .ThenBy(i => i.Name)
            .Select(i => new NomVO(i.InstitutionId, $"{i.InstitutionId} {i.Name}"))
            .WithOffsetAndLimit(offset ?? 0, limit)
            .ToArrayAsync(ct);
    }
}
