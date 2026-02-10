namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class TopicPlanNomsRepository : Repository, ITopicPlanNomsRepository
{
    public TopicPlanNomsRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<NomVO[]> GetNomsByIdAsync(int sysUserId, int[] ids, CancellationToken ct)
    {
        if (!ids.Any())
        {
            throw new ArgumentException($"'{nameof(ids)}' should be non-empty.");
        }

        return await this.DbContext.Set<TopicPlan>()
            .Where(s =>
                s.CreatedBySysUserId == sysUserId &&
                this.DbContext
                    .MakeIdsQuery(ids)
                    .Any(id => s.TopicPlanId == id.Id))
            .Select(s => new NomVO(s.TopicPlanId, s.Name))
            .ToArrayAsync(ct);
    }

    public async Task<NomVO[]> GetNomsByTermAsync(
        int sysUserId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct)
    {
        // TODO use basicClassId and curriculumId

        var predicate = PredicateBuilder.True<TopicPlan>();
        predicate = predicate.And(tp => tp.CreatedBySysUserId == sysUserId);

        if (!string.IsNullOrWhiteSpace(term))
        {
            string[] words = term.Split(null); // split by whitespace
            foreach (var word in words)
            {
                predicate = predicate.AndStringContains(p => p.Name, word);
            }
        }

        return await (
            from tp in this.DbContext.Set<TopicPlan>().Where(predicate)

            orderby tp.BasicClassId, tp.Name

            select new NomVO(
                tp.TopicPlanId,
                tp.Name
            ))
            .WithOffsetAndLimit(offset, limit)
            .ToArrayAsync(ct);
    }
}
