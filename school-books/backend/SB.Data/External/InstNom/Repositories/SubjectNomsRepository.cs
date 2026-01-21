namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class SubjectNomsRepository : Repository, ISubjectNomsRepository
{
    public SubjectNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<Subject>()
            .Where(s =>
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.SubjectId == id.Id))
            .Select(bc => new NomVO(bc.SubjectId, bc.SubjectName))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(int schoolYear, int instId, string? term, int? offset, int? limit, CancellationToken ct)
    {
        var predicate = PredicateBuilder.True<Subject>();

        predicate = predicate.And(s => s.IsValid == true);
        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(t => t.SubjectName, word);
            }
        }

        var institutionCurriculums = (
            from c in this.DbContext.Set<Curriculum>()

            join s in this.DbContext.Set<Subject>().Where(predicate) on c.SubjectId equals s.SubjectId

            where c.SchoolYear == schoolYear &&
                c.InstitutionId == instId

            select new
            {
                s.SubjectId,
                s.SubjectName
            }).Distinct();

        var customCurriculums = (
            from c in this.DbContext.Set<CustomVarValue>()

            join s in this.DbContext.Set<Subject>().Where(predicate) on c.CustomVarVal equals s.SubjectId

            where c.InstitutionId == instId && s.IsValid == true && c.IsValid == true

            select new
            {
                s.SubjectId,
                s.SubjectName
            }).Distinct();

        var aloneSubjects =
            from s in this.DbContext.Set<Subject>().Where(predicate)
            where s.SubjectId <= Subject.LastInternalSubjectId
            select new
            {
                s.SubjectId,
                s.SubjectName
            };

        var allSubjects =
            await aloneSubjects
                .Union(institutionCurriculums)
                .Union(customCurriculums)
                .OrderBy(s => s.SubjectName)
                .ToArrayAsync(ct);

        return allSubjects
            .Select(s => new NomVO(s.SubjectId, s.SubjectName))
            .WithOffsetAndLimit(offset ?? 0, limit)
            .ToArray();
    }
}
